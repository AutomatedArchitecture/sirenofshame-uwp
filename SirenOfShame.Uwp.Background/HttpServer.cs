using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SirenOfShame.Uwp.Background
{
    public sealed class HttpServer
    {
        private readonly int _port;
        private readonly StreamSocketListener _listener;
        private const uint BufferSize = 8192;
        readonly HttpRouter _httpRouter = new HttpRouter();

        public HttpServer(int port)
        {
            _port = port;
            _listener = new StreamSocketListener();
            _listener.Control.KeepAlive = true;
            _listener.Control.NoDelay = true;
            _listener.ConnectionReceived += ListenerOnConnectionReceived;
        }

        private async void ListenerOnConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            var socket = args.Socket;
            var request = await GetRequest(socket);

            string requestAsString = request.ToString();
            string[] splitRequestAsString = requestAsString.Split('\n');
            if (splitRequestAsString.Length != 0)
            {
                string requestMethod = splitRequestAsString[0];
                string[] requestParts = requestMethod.Split(' ');
                if (requestParts.Length > 1)
                {
                    var httpVerb = requestParts[0];
                    await WriteResponse(httpVerb, requestParts[1], socket);
                }
            }
        }

        private static async Task<StringBuilder> GetRequest(StreamSocket socket)
        {
            StringBuilder request = new StringBuilder();
            byte[] data = new byte[BufferSize];
            IBuffer buffer = data.AsBuffer();
            uint dataRead = BufferSize;
            using (IInputStream input = socket.InputStream)
            {
                while (dataRead == BufferSize)
                {
                    await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }
            return request;
        }

        private async Task WriteResponse(string httpVerb, string requestPart, StreamSocket socket)
        {
            var httpContext = new HttpContext(httpVerb, requestPart, socket);
            try
            {
                await _httpRouter.ProcessRequest(httpContext);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                // throwing an exceptin here will crash the whole service
                httpContext.WriteString(ex.ToString());
            }
        }

        public IAsyncAction StartServerAction()
        {
            return StartServer().AsAsyncAction();
        }

        internal async Task StartServer()
        {
            await Task.Run(async () =>
            {
                await _listener.BindServiceNameAsync(_port.ToString());
            });
        }
    }
}
