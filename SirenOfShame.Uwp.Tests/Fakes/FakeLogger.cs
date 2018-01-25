using System;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Watcher;

namespace SirenOfShame.Uwp.Tests.Fakes
{
    public class FakeLogger : ILog
    {
        public FakeLogger(Type type) { }

        public Task Error(string message)
        {
            return Task.FromResult(true);
        }

        public Task Error(string message, Exception ex)
        {
            return Task.FromResult(true);
        }

        public Task Warn(string message)
        {
            return Task.FromResult(true);
        }

        public Task Warn(string message, Exception ex)
        {
            return Task.FromResult(true);
        }

        public Task Info(string message)
        {
            return Task.FromResult(true);
        }

        public Task Debug(string message)
        {
            return Task.FromResult(true);
        }
    }
}