using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Background.Controllers;

namespace SirenOfShame.Uwp.Background
{
    internal sealed class HttpRouter
    {
        private Dictionary<string, Func<ApiController>> _controllers = new Dictionary<string, Func<ApiController>>
        {
            { "ledPatterns", () => new LedPatternsController() },
            { "audioPatterns", () => new AudioPatternsController() }
        };

        public async Task ProcessRequest(HttpContext context)
        {
            const string rootNs = "SirenOfShame.Uwp.Background.wwwroot";
            if (context.RequestPart == "/")
            {
                context.WriteResource(rootNs + ".index.html", "text/html");
                return;
            }
            if (context.RequestPart.StartsWith("/api/"))
            {
                var last = context.RequestPart.Split('/').Last();
                Func<ApiController> func;
                if (_controllers.TryGetValue(last, out func))
                {
                    var apiController = func();
                    var result = await apiController.Get(context);
                    context.WriteString(result);
                    return;
                }
                context.Write404("API not found: " + context.RequestPart);
                return;
            }
            var contentType = context.GetRequestContentType();
            var resourceNs = RequestToNamespace(context.RequestPart);

            context.WriteResource(rootNs + resourceNs, contentType);
        }

        private static string RequestToNamespace(string request)
        {
            var urlParts = request.Split('/');
            var fileName = urlParts.Last();
            var location = string.Join(".", urlParts.Take(urlParts.Length - 1));
            var locationNs = location.Replace("@", "_").Replace("-", "_");

            var resourceNs = locationNs + "." + fileName;
            return resourceNs;
        }
    }
}
