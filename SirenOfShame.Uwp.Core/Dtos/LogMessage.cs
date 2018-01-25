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

    public class LogMessage
    {
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }
}
