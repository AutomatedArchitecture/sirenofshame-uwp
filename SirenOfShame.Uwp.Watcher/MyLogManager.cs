using System;
using MetroLog;
using MetroLog.Targets;

namespace SirenOfShame.Uwp.Watcher
{
    public static class MyLogManager
    {
        public static ILog GetLog(Type type)
        {
            return new MetroLogger(type);
        }
    }

    public class MetroLogger : ILog
    {
        static MetroLogger()
        {
            // set more verbose logging to the file system (default is only warn and above)
            var minFileSystemLogLevel = LogLevel.Debug;
            LogManagerFactory.DefaultConfiguration.AddTarget(minFileSystemLogLevel, LogLevel.Fatal, new StreamingFileTarget());
        }


        private readonly ILogger _log;

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
        void Error(string message, Exception ex);
        void Warn(string message);
        void Info(string message);
        void Debug(string message);
    }
}
