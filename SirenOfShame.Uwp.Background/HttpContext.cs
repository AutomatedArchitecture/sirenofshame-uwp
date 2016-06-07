using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            Write(str, "200 OK");
        }
        
        public void Write404(string str)
        {
            Write(str, "404 Not Found");
        }
        
        private void Write(string str, string httpCode)
        {
            byte[] bodyArray = Encoding.UTF8.GetBytes(str);
            // Show the html 
            using (var outputStream = _socket.OutputStream)
            using (Stream response = outputStream.AsStreamForWrite())
            using (MemoryStream stream = new MemoryStream(bodyArray))
            {
                var header1 = string.Format($"HTTP/1.1 {httpCode}\r\n" +
                              "Content-Length: {0}\r\n" +
                              "Connection: close\r\n\r\n", stream.Length);
                WriteHttpHeader(response, header1);
                stream.CopyTo(response);
                response.Flush();
            }
        }

        private static void WriteHttpHeader(Stream response, string header)
        {
            byte[] headerArray = Encoding.UTF8.GetBytes(header);
            response.Write(headerArray, 0, headerArray.Length);
        }

        public void WriteResource(string resource, string contentType)
        {
            var assembly = typeof(HttpServer).GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resource))
            {
                if (stream == null)
                {
                    Write404("Resource not found: " + resource);
                }
                else
                {
                    WriteStream(_socket, stream, contentType);
                }
            }
        }

        private static void WriteStream(StreamSocket socket, Stream sourceStream, string contentType)
        {
            using (var outputStream = socket.OutputStream)
            using (Stream response = outputStream.AsStreamForWrite())
            {
                string header = String.Format("HTTP/1.1 200 OK\r\n" +
                                              "Content-Length: {0}\r\n" +
                                              "Content-Type: {1}; charset=utf-8\r\n" +
                                              "Connection: close\r\n\r\n",
                        sourceStream.Length,
                        contentType);

                WriteHttpHeader(response, header);
                sourceStream.CopyTo(response);
                response.Flush();
            }
        }

        private static readonly Dictionary<string, string> MimeTypes = new Dictionary<string, string>
        {
            { "css", "text/css" },
            { "js", "application/javascript" },
            { "gif", "image/gif" },
            { "png", "image/png" },
            { "jpg", "image/jpeg" },
            { "html", "text/html" },
        };

        public string GetRequestContentType()
        {
            var extension = RequestPart.Split('.').Last();
            string mimeType;
            return MimeTypes.TryGetValue(extension, out mimeType) ? mimeType : "text/plain";
        }
    }
}
