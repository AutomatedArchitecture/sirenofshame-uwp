using System;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Shared.Dtos;

namespace SirenOfShame.Uwp.Core.Services
{
    public class MyLogManager
    {
        public static Func<Type, ILog> GetLog = type => new ConsoleLogger(type);
        public static readonly Func<Type, ILog> GetLogger = GetLog;
    }

    public class ConsoleLogger : ILog
    {
        private readonly Type _type;

        public ConsoleLogger(Type type)
        {
            _type = type;
        }

        private void Write(LogLevel logLevel, string message, Exception ex = null)
        {
            System.Diagnostics.Debug.WriteLine($"{logLevel.ToString().ToUpper()}: {_type} - {message} {ex}");
        }

        public Task Error(string message)
        {
            Write(LogLevel.Error, message);
            return Task.FromResult(true);
        }

        public Task Error(string message, Exception exception)
        {
            Write(LogLevel.Error, message, exception);
            return Task.FromResult(true);
        }

        public Task Warn(string message)
        {
            Write(LogLevel.Warn, message);
            return Task.FromResult(true);
        }

        public Task Warn(string message, Exception ex)
        {
            Write(LogLevel.Warn, message, ex);
            return Task.FromResult(true);
        }

        public Task Info(string message)
        {
            Write(LogLevel.Info, message);
            return Task.FromResult(true);
        }

        public Task Debug(string message)
        {
            Write(LogLevel.Debug, message);
            return Task.FromResult(true);
        }
    }
}
