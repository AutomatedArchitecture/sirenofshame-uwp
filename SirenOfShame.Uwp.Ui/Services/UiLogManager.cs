using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Models;
using SirenOfShame.Uwp.Core.Services;
using SirenOfShame.Uwp.Shared.Dtos;
using SQLite;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class LogEntry : ILogEntry
    {
        [PrimaryKey, AutoIncrement]
        public int ItemId { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public LogLevel Level { get; set; }
        public string CallerType { get; set; }
        public string Message { get; set; }

        public bool HasException { get; set; }
        public string Exception { get; set; }
        public string ExceptionTypeName { get; set; }
        public int? ExceptionHresult { get; set; }
    }

    public class UiLogManager : MyLogManager
    {
        private SQLiteAsyncConnection _conn;

        public async Task Initialize()
        {
            var localAppData = Environment.SpecialFolder.LocalApplicationData;
            var databasePath = Path.Combine(Environment.GetFolderPath(localAppData), "SirenOfShameLogs.sosdb");

            _conn = new SQLiteAsyncConnection(databasePath);
            await _conn.CreateTableAsync<LogEntry>();

            GetLog = (type) => new SqlLogger(type, _conn);
        }

        public async Task<ReadLogEntriesResult> ReadLogEntriesAsync(bool showAll)
        {
            var allRows = await _conn.Table<LogEntry>()
                .Where(i => showAll || (i.Level != LogLevel.Debug))
                .OrderByDescending(i => i.DateTimeUtc)
                .Take(50)
                .ToListAsync();
            var logEntries = allRows
                .Select(i => (ILogEntry)new LogEntry
                {
                    ItemId = i.ItemId,
                    Level = i.Level,
                    Message = MakeFullMessage(i),
                    DateTimeUtc = i.DateTimeUtc.ToLocalTime()
                })
                .ToList();
            
            return new ReadLogEntriesResult
            {
                Events = logEntries
            };
        }

        private static string MakeFullMessage(LogEntry i)
        {
            if (i.Exception == null) return i.Message;
            return $"{i.CallerType}{Environment.NewLine}{i.Message}{Environment.NewLine}{Environment.NewLine}{i.ExceptionTypeName} ({i.ExceptionHresult}): {i.Exception}";
        }
    }

    public class SqlLogger : ILog
    {
        private readonly Type _type;
        private readonly SQLiteAsyncConnection _conn;

        public SqlLogger(Type type, SQLiteAsyncConnection conn)
        {
            _type = type;
            _conn = conn;
        }

        private async Task Write(string message, LogLevel level, Exception ex = null)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"{level.ToString().ToUpper()}: {message} {ex}");
#endif
            
            var logEntry = new LogEntry
            {
                Message = message,
                CallerType = _type.ToString(),
                Level = level,
                HasException = ex != null,
                Exception = ex?.ToString(),
                DateTimeUtc = DateTime.UtcNow,
                ExceptionHresult = ex?.HResult,
                ExceptionTypeName = ex?.GetType().Name,
                
            };
            await Write(logEntry);
        }

        private async Task Write(LogEntry logEntry)
        {
            await _conn.InsertAsync(logEntry);
        }

        public async Task Error(string message)
        {
            await Write(message, LogLevel.Error);
        }

        public async Task Error(string message, Exception ex)
        {
            await Write(message, LogLevel.Error, ex);
        }

        public async Task Warn(string message)
        {
            await Write(message, LogLevel.Warn);
        }

        public async Task Warn(string message, Exception ex)
        {
            await Write(message, LogLevel.Warn, ex);
        }

        public async Task Info(string message)
        {
            await Write(message, LogLevel.Info);
        }

        public async Task Debug(string message)
        {
            await Write(message, LogLevel.Debug);
        }
    }
}
