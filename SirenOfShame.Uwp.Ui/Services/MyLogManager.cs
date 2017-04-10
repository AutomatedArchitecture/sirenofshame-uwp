using System;
using System.Threading.Tasks;
using MetroLog;
using MetroLog.Targets;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class MyLogManager
    {
        public static ILog GetLog(Type type)
        {
            return new MetroLogger(type);
        }
    }

    public class MetroLogger : ILog
    {
        private readonly ILogger _log;
        private static SQLiteTarget _sqLiteTarget;

        static MetroLogger()
        {
            // set more verbose logging to the file system (default is only warn and above)
            var minLogLevel = LogLevel.Trace;
            _sqLiteTarget = new SQLiteTarget();
            LogManagerFactory.DefaultConfiguration.AddTarget(minLogLevel, LogLevel.Fatal, _sqLiteTarget);
        }

        public static async Task<ReadLogEntriesResult> ReadLogEntriesAsync(bool showAll)
        {
            var logReadQuery = new LogReadQuery
            {
                IsDebugEnabled = showAll
            };
            var result = await _sqLiteTarget.ReadLogEntriesAsync(logReadQuery);
            return result;
        }

        public MetroLogger(Type type)
        {
            _log = LogManagerFactory.DefaultLogManager.GetLogger(type);
        }

        public void Error(string message)
        {
            _log.Error(message);
        }

        public void Error(string message, Exception ex)
        {
            _log.Error(message, ex);
        }

        public void Warn(string message)
        {
            _log.Warn(message);
        }

        public void Info(string message)
        {
            _log.Info(message);
        }

        public void Debug(string message)
        {
            _log.Debug(message);
        }
    }

    public interface ILog
    {
        void Error(string message);
        void Error(string message, Exception webException);
        void Warn(string message);
        void Info(string message);
        void Debug(string message);
    }
}
