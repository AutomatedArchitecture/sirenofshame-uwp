using System;

namespace SirenOfShame.Uwp.Watcher.Exceptions
{
    public class ServerUnavailableException : Exception
    {
        public ServerUnavailableException(string message, Exception innerException) : base(message, innerException) { }
        public ServerUnavailableException(string message) : base(message) { }
        public ServerUnavailableException() { }
    }
}
