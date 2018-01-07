using System;

namespace SirenOfShame.Uwp.Shared.Dtos
{
    internal enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error
    }

    internal class LogMessage
    {
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}
