using System;

namespace SirenOfShame.Uwp.Core.Services
{
    public abstract class MessageCommandProcessorBase
    {
        protected void ParseMessage(string dkey, object value, out MessageDestination messageDestination, out string key, out string messageBody)
        {
            messageBody = value as string;
            var strings = dkey.Split(',');
            if (strings.Length <= 1)
            {
                key = dkey;
                messageDestination = MessageDestination.All;
            }
            else
            {
                var destStr = strings[0];
                messageDestination = (MessageDestination)Enum.Parse(typeof(MessageDestination), destStr);
                key = strings[1];
            }
        }
    }
}
