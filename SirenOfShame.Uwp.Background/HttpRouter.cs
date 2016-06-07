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
            context.WriteResource(rootNs + context.RequestPart.Replace("/", "."), contentType);
        }
    }
}
