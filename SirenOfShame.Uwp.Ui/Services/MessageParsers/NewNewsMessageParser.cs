using Newtonsoft.Json;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Ui.Services.MessageParsers
{
    class NewNewsMessageParser : MessageParserBase
    {
        public override string Key => "NewNewsItem";

        public NewNewsItemEventArgs Parse(string value)
        {
            return JsonConvert.DeserializeObject<NewNewsItemEventArgs>(value);
        }
    }
}
