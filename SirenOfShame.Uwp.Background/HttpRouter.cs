using Windows.Networking.Sockets;

namespace SirenOfShame.Uwp.Background
{
    public sealed class HttpRouter
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.RequestPart == "/css/bootstrap.min.css")
            {
                context.WriteResource("SirenOfShame.Uwp.Background.css.bootstrap.min.css", "text/css");
                return;
            }
            if (context.RequestPart == "/")
            {
                context.WriteResource("SirenOfShame.Uwp.Background.html.index.html", "text/html");
                return;
            }
            if (context.RequestPart == "/js/bootstrap.min.js")
            {
                context.WriteResource("SirenOfShame.Uwp.Background.js.bootstrap.min.js", "text/javascript");
                return;
            }
            
            context.Write404("Unknown resource" + context.RequestPart);
        }
    }
}
