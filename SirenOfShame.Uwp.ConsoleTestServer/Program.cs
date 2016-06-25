using System;
using System.Reflection;
using IotWeb.Common.Http;
using IotWeb.Server;
using SirenOfShame.Uwp.Background;

namespace SirenOfShame.Uwp.ConsoleTestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var _httpServer = new HttpServer(8001);
            _httpServer.AddHttpRequestHandler(
                "/",
                new HttpResourceHandler(typeof(Program).GetTypeInfo().Assembly,
                "wwwroot", "index.html"));
            _httpServer.AddWebSocketRequestHandler(
                "/sockets/",
                new WebSocketHandler()
                );
            _httpServer.Start();
            Console.WriteLine("Server started on port 8001");
            Console.WriteLine("Press any key to exit ..");
            Console.ReadKey();
        }
    }
}
