using System;
using System.Threading.Tasks;

namespace SirenOfShame.Uwp.Core.Interfaces
{
    public interface ILog
    {
        Task Error(string message);
        Task Error(string message, Exception webException);
        Task Warn(string message);
        Task Warn(string message, Exception httpRequestException);
        Task Info(string message);
        Task Debug(string message);
    }
}