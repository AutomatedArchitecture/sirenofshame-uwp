using Windows.Networking.Sockets;

namespace SirenOfShame.Uwp.Background
{
    public sealed class HttpRouter
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.RequestPart.StartsWith("/css/"))
            {
                context.WriteResource("SirenOfShame.Uwp.Background" + context.RequestPart.Replace("/", "."), "text/css");
                return;
            }
            if (context.RequestPart.StartsWith("/js/"))
            {
                context.WriteResource("SirenOfShame.Uwp.Background" + context.RequestPart.Replace("/", "."), "text/javascript");
                return;
            }
            if (context.RequestPart == "/")
            {
                context.WriteResource("SirenOfShame.Uwp.Background.html.index.html", "text/html");
                return;
            }

            context.Write404("Unknown resource" + context.RequestPart);
        }
    }
}
