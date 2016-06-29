using System;

namespace SirenOfShame.Uwp.Watcher.Exceptions
{
    public class SosException : Exception
    {
        public SosException()
        {
        }

        public SosException(string message) : base(message)
        {
        }

        public SosException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
