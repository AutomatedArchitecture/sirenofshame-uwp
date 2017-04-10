using System;
using SirenOfShame.Uwp.Watcher;

namespace SirenOfShame.Uwp.Tests.Fakes
{
    public class FakeLogger : ILog
    {
        public FakeLogger(Type type) { }

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
}