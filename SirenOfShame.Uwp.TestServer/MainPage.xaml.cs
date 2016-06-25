using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using IotWeb.Common.Http;
using IotWeb.Server;
using SirenOfShame.Uwp.Server;
using SirenOfShame.Uwp.Server.Services;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SirenOfShame.Uwp.TestServer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private HttpServer _httpServer;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _httpServer = new HttpServer(8001);
            _httpServer.AddHttpRequestHandler(
                "/",
                new HttpResourceHandler(typeof(MainPage).GetTypeInfo().Assembly,
                "wwwroot", "index.html"));
            _httpServer.AddWebSocketRequestHandler(
                "/sockets/",
                new WebSocketHandler()
                );
            _httpServer.Start();
            SirenService.Instance.StartWatching();
        }
    }
}
