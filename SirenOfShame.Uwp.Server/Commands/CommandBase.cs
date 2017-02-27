using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SirenOfShame.Uwp.Server.Models;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal abstract class CommandBase<T> : CommandBase
    {
        protected abstract Task<SocketResult> Invoke(T frame);

        public override Task<SocketResult> Invoke(string frame)
        {
            var obj = Deserialize(frame);
            return Invoke(obj);
        }

        private static T Deserialize(string frame)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var request = JsonConvert.DeserializeObject<T>(frame, jsonSerializerSettings);
            return request;
        }
    }

    internal abstract class CommandBase
    {
        public abstract string CommandName { get; }

        public abstract Task<SocketResult> Invoke(string frame);

        internal static readonly CommandBase[] Commands = {
            new EchoCommand(),
            new SirenInfoCommand(),
            new PlayLedPatternCommand(),
            new PlayAudioPatternCommand(),
            new GetBuildDefinitionsCommand(),
            new GetCiEntryPointSettingCommand(),
            new GetCiEntryPointSettingsCommand(),
            new AddCiEntryPointSettingCommand(),
            new DeleteSettingsCommand(),
            new GetCiEntryPointsCommand(),
            new UpdateMockBuildCommand(),
            new SendLatestCommand(),
            new DeleteBuildDefinitionCommand()
        };
    }
}