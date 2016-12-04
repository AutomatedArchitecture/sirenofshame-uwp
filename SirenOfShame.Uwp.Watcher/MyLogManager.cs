using System;

namespace SirenOfShame.Uwp.Watcher
{
    public class MyLogManager
    {
        public static ILog GetLog(Type type)
        {
            return new ConsoleLogger(type);
        }
    }

    public class ConsoleLogger : ILog
    {
        private readonly Type _type;

        public ConsoleLogger(Type type)
        {
            _type = type;
        }

        public void Error(string message)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR {_type} - {message}");
        }

        public void Error(string message, Exception webException)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR {_type} - {message} - {webException}");
        }

        public void Warn(string message)
        {
            System.Diagnostics.Debug.WriteLine($"WARN {_type} - {message}");
        }

        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine($"DEBUG {_type} - {message}");
        }
    }

    public interface ILog
    {
        void Error(string message);
        void Error(string message, Exception webException);
        void Warn(string message);
        void Debug(string message);
    }
}
