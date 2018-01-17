using System;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Services;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Shared.Commands;
using SirenOfShame.Uwp.Shared.Dtos;
using LogLevel = SirenOfShame.Uwp.Shared.Dtos.LogLevel;

namespace SirenOfShame.Uwp.Server.Commands.CiEntryPointSettings
{
    internal class LogCommand : CommandBase<LogMessage>
    {
        private readonly ILog _log = MyLogManager.GetLog(typeof(LogCommand));

        public override string CommandName => CommandNames.LOG;

        protected override async Task<SocketResult> Invoke(LogMessage frame)
        {
            switch (frame.LogLevel)
            {
                case LogLevel.Debug:
                    await _log.Debug(frame.Message);
                    break;
                case LogLevel.Info:
                    await _log.Info(frame.Message);
                    break;
                case LogLevel.Warn:
                    await _log.Warn(frame.Message);
                    break;
                case LogLevel.Error:
                    if (frame.Exception == null)
                        await _log.Error(frame.Message);
                    else
                        await _log.Error(frame.Message + Environment.NewLine + Environment.NewLine + frame.Exception);
                    break;
            }

            await Task.Yield();
            return new OkSocketResult();
        }
    }
}