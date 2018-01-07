using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Shared.Commands;
using SirenOfShame.Uwp.Shared.Dtos;
using SirenOfShame.Uwp.Watcher;
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
                    _log.Debug(frame.Message);
                    break;
                case LogLevel.Info:
                    _log.Info(frame.Message);
                    break;
                case LogLevel.Warn:
                    _log.Warn(frame.Message);
                    break;
                case LogLevel.Error:
                    if (frame.Exception == null)
                        _log.Error(frame.Message);
                    else
                        _log.Error(frame.Message, frame.Exception);
                    break;
            }

            await Task.Yield();
            return new OkSocketResult();
        }
    }
}