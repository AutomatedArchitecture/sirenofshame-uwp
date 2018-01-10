using System;
using System.Collections.Generic;

namespace SirenOfShame.Uwp.Shared.Dtos
{
    public enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal,
    }

    public class ReadLogEntriesResult
    {
        public List<LogEventInfoItem> Events { get; set; }
    }

    public class LogEventInfoItem
    {
        //[PrimaryKey, AutoIncrement]
        public int ItemId { get; set; }
        public int SessionId { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public long SequenceId { get; set; }
        public LogLevel Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }

        public bool HasException { get; set; }
        public string Exception { get; set; }
        public string ExceptionTypeName { get; set; }
        public int ExceptionHresult { get; set; }
    }

    public class LogMessage
    {
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}
