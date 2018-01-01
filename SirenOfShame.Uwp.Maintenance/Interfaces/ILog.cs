using System;

namespace SirenOfShame.Uwp.Maintenance
{
    public interface ILog
    {
        void Error(string message);
        void Error(string message, Exception exception);
        void Warn(string message);
        void Info(string message);
        void Debug(string message);
    }
}