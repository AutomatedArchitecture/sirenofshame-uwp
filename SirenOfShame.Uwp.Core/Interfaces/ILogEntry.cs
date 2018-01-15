using System;
using SirenOfShame.Uwp.Shared.Dtos;

namespace SirenOfShame.Uwp.Core.Interfaces
{
    public interface ILogEntry
    {
        int ItemId { get; set; }
        DateTime DateTimeUtc { get; set; }
        LogLevel Level { get; set; }
        string CallerType { get; set; }
        string Message { get; set; }
        bool HasException { get; set; }
        string Exception { get; set; }
        string ExceptionTypeName { get; set; }
        int? ExceptionHresult { get; set; }
    }
}