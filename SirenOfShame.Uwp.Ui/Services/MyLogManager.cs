using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Models;

namespace SirenOfShame.Uwp.Ui.Services
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
        public static Task<ReadLogEntriesResult> ReadLogEntriesAsync(bool showAll)
        {
            return Task.FromResult(new ReadLogEntriesResult { Events = new List<ILogEntry>() });
        }

        public MetroLogger(Type type)
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
