using System;

namespace SirenOfShame.Uwp.Maintenance.Log
{
    internal static class MyLogManager
    {
        public static Func<Type, ILog> GetLog = (type) => new ConsoleLogger(type);
    }

    public sealed class ConsoleLogger : ILog
    {
        private Type type;

        public ConsoleLogger(Type type)
        {
            this.type = type;
        }

        public void Error(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Error(string message, Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(message + " " + exception);
        }

        public void Warn(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Info(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
