using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SirenOfShame.Uwp.Server.Models;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal abstract class CommandBase
    {
        public abstract string CommandName { get; }

        public abstract Task<SocketResult> Invoke(string frame);

        protected static T Deserialize<T>(string frame)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var request = JsonConvert.DeserializeObject<T>(frame, jsonSerializerSettings);
            return request;
        }
    }
}