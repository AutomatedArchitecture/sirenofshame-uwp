using System;
using System.IO;
using System.Reflection;
using System.Text;
using Windows.Networking.Sockets;

namespace SirenOfShame.Uwp.Background
{
    public sealed class HttpContext
    {
        public string RequestPart { get; }
        private readonly StreamSocket _socket;

        public HttpContext(string requestPart, StreamSocket socket)
        {
            RequestPart = requestPart;
            _socket = socket;
        }

        public void WriteString(string str)
        {
            byte[] bodyArray = Encoding.UTF8.GetBytes(str);
            // Show the html 
            using (var outputStream = _socket.OutputStream)
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

        public void WriteResource(StreamSocket socket, string resource)
        {
            var assembly = typeof(HttpServer).GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resource))
            {
                WriteStream(socket, stream);
            }
        }

        private static void WriteStream(StreamSocket socket, Stream sourceStream)
        {
            using (var outputStream = socket.OutputStream)
            using (Stream resp = outputStream.AsStreamForWrite())
            {
                string header = String.Format("HTTP/1.1 200 OK\r\n" +
                                              "Content-Length: {0}\r\n" +
                                              "Content-Type: text/html; charset=utf-8\r\n" +
                                              "Connection: close\r\n\r\n",
                    sourceStream.Length);
                byte[] headerArray = Encoding.UTF8.GetBytes(header);
                resp.Write(headerArray, 0, headerArray.Length);
                sourceStream.CopyTo(resp);
                resp.Flush();
            }
        }
    }
}
