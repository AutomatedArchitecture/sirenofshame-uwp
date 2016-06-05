using Windows.Networking.Sockets;

namespace SirenOfShame.Uwp.Background
{
    public sealed class HttpRouter
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.RequestPart == "/Index.html")
            {
                context.WriteString("<html><h1>Hello There!</h1></html>");
            }
            else
            {
                context.WriteString("Hello There!");
            }
        }
    }
}
