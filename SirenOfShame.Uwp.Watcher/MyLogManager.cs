using System;
using MetroLog;

namespace SirenOfShame.Uwp.Watcher
{
    public static class MyLogManager
    {
        public static ILog GetLog(Type type)
        {
            return new ConsoleLogger(type);
        }
    }

    public class ConsoleLogger : ILog
    {
        private readonly ILogger _log;

        public ConsoleLogger(Type type)
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
        void Error(string message, Exception ex);
        void Warn(string message);
        void Info(string message);
        void Debug(string message);
    }
}
