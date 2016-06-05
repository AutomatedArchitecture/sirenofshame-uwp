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
                    if (requestParts[0] == "GET")
                        WriteResponse(requestParts[1], socket);
                    else
                        throw new InvalidDataException("HTTP method not supported: "
                                                       + requestParts[0]);
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

        private void WriteResponse(string requestPart, StreamSocket socket)
        {
            try
            {
                WriteString(socket, "Hello There!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                // throwing an exceptin here will crash the whole service
                WriteString(socket, ex.ToString());
            }
        }

        private static void WriteString(StreamSocket socket, string str)
        {
            byte[] bodyArray = Encoding.UTF8.GetBytes(str);
            // Show the html 
            using (var outputStream = socket.OutputStream)
            using (Stream resp = outputStream.AsStreamForWrite())
            using (MemoryStream stream = new MemoryStream(bodyArray))
            {
                string header = String.Format("HTTP/1.1 200 OK\r\n" +
                                              "Content-Length: {0}\r\n" +
                                              "Connection: close\r\n\r\n",
                    stream.Length);
                byte[] headerArray = Encoding.UTF8.GetBytes(header);
                resp.Write(headerArray, 0, headerArray.Length);
                stream.CopyTo(resp);
                resp.Flush();
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
