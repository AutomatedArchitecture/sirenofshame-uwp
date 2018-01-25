using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Services;
using SirenOfShame.Uwp.Maintenance.Services;
using SirenOfShame.Uwp.Shared.Commands;
using SirenOfShame.Uwp.Shared.Dtos;

namespace SirenOfShame.Uwp.Maintenance.Log
{
    /// <summary>
    /// A logger who simply forwards messages to the MessageRelay for Background to log for us
    /// </summary>
    internal sealed class MessageRelayLogger : ILog
    {
        private readonly MaintenanceMessageRelayService _messageRelayService;

        public MessageRelayLogger(MaintenanceMessageRelayService messageRelayService)
        {
            _messageRelayService = messageRelayService;
        }

        private async Task Send(LogMessage logMessage)
        {
            Console.WriteLine($"{logMessage.LogLevel}: {logMessage.Message} {logMessage.Exception}");
            
            // todo: runtime customizable log levels
            if (logMessage.LogLevel == LogLevel.Debug) return;

            var str = JsonConvert.SerializeObject(logMessage);
            await _messageRelayService.SendMessageAsync(MessageDestination.Server, CommandNames.LOG, str);
        }

        public async Task Error(string message)
        {
            await Send(new LogMessage { LogLevel = LogLevel.Error, Message = message });
        }

        public async Task Error(string message, Exception exception)
        {
            await Send(new LogMessage { LogLevel = LogLevel.Error, Message = message, Exception = exception.ToString() });
        }

        public async Task Warn(string message, Exception ex)
        {
            await Send(new LogMessage { LogLevel = LogLevel.Warn, Message = message, Exception = ex.ToString() });
        }

        public async Task Info(string message)
        {
            await Send(new LogMessage { LogLevel = LogLevel.Info, Message = message });
        }

        public async Task Debug(string message)
        {
            await Send(new LogMessage { LogLevel = LogLevel.Debug, Message = message });
        }
    }
}
