using Windows.Networking.Sockets;

namespace SirenOfShame.Uwp.Background
{
    public sealed class HttpRouter
    {
        public void ProcessRequest(HttpContext context)
        {
            const string rootNs = "SirenOfShame.Uwp.Background.wwwroot";
            if (context.RequestPart.StartsWith("/css/"))
            {
                context.WriteResource(rootNs + context.RequestPart.Replace("/", "."), "text/css");
                return;
            }
            if (context.RequestPart.StartsWith("/js/"))
            {
                context.WriteResource(rootNs + context.RequestPart.Replace("/", "."), "text/javascript");
                return;
            }
            if (context.RequestPart == "/")
            {
                context.WriteResource(rootNs + ".index.html", "text/html");
                return;
            }

            context.Write404("Unknown resource" + context.RequestPart);
        }
    }
}
