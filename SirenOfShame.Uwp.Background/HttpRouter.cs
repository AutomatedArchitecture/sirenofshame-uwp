using System;
using System.IO;
using System.Linq;
using Windows.Networking.Sockets;

namespace SirenOfShame.Uwp.Background
{
    public sealed class HttpRouter
    {
        public void ProcessRequest(HttpContext context)
        {
            const string rootNs = "SirenOfShame.Uwp.Background.wwwroot";
            if (context.RequestPart == "/")
            {
                context.WriteResource(rootNs + ".index.html", "text/html");
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
