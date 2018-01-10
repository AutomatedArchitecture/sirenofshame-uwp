using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Shared.Dtos;

namespace SirenOfShame.Uwp.Watcher
{
    public static class MyLogManager
    {
        public static Func<Type, ILog> GetLog = (type) => new SqlLogger(type);

        public static void Initialize()
        {

        }

        public static Task<ReadLogEntriesResult> ReadLogEntriesAsync(bool showAll)
        {
            return Task.FromResult(new ReadLogEntriesResult {Events = new List<LogEventInfoItem>()});
        }
    }

    public class SqlLogger : ILog
    {
        public SqlLogger(Type type)
        {
        }

        public void Error(string message)
        {
        }

        public void Error(string message, Exception ex)
        {
        }

        public void Warn(string message)
        {
        }

        public void Info(string message)
        {
        }

        public void Debug(string message)
        {
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
