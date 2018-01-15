using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Models;
using SirenOfShame.Uwp.Shared.Dtos;
using SQLite;

namespace SirenOfShame.Uwp.Watcher
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

    public static class MyLogManager
    {
        private static SQLiteAsyncConnection _conn;
        public static Func<Type, ILog> GetLog = (type) => new SqlLogger(type, _conn);

        public static async Task Initialize()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SirenOfShameLogs.db");

            _conn = new SQLiteAsyncConnection(databasePath);
            await _conn.CreateTableAsync<LogEntry>();
        }

        public static async Task<ReadLogEntriesResult> ReadLogEntriesAsync(bool showAll)
        {
            var allRows = await _conn.Table<LogEntry>().ToListAsync();
            var logEntries = allRows.Cast<ILogEntry>().ToList();
            
            return new ReadLogEntriesResult
            {
                Events = logEntries
            };
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

        public async Task Info(string message)
        {
            await Write(message, LogLevel.Info);
        }

        public async Task Debug(string message)
        {
            await Write(message, LogLevel.Debug);
        }
    }

    public interface ILog
    {
        Task Error(string message);
        Task Error(string message, Exception webException);
        Task Warn(string message);
        Task Info(string message);
        Task Debug(string message);
    }
}
