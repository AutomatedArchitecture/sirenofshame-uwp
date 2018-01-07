using System;
using System.Threading.Tasks;

namespace SirenOfShame.Uwp.Maintenance
{
    internal interface ILog
    {
        Task Error(string message);
        Task Error(string message, Exception exception);
        Task Warn(string message);
        Task Info(string message);
        Task Debug(string message);
    }
}