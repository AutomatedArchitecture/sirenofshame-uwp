using System.Collections.Generic;
using System.Linq;

namespace SirenOfShame.Uwp.Watcher.Helpers
{
    /// <summary>
    /// Technically this class has no business being in this project, but it makes unit testing easier
    /// </summary>
    public class HttpHelper
    {
        public static Dictionary<string, string> ParseQueryStringParams(string requestPart)
        {
            if (!requestPart.Contains("?")) return new Dictionary<string, string>();
            var paramStr = requestPart.Split('?').Last();
            var paramsList = paramStr.Split('&');
            var dictionary = new Dictionary<string, string>();
            foreach (var parameter in paramsList)
            {
                var strings = parameter.Split('=');
                var key = strings.FirstOrDefault();
                var value = strings.LastOrDefault();
                dictionary.Add(key, value);
            }

            return dictionary;
        }
    }
}
