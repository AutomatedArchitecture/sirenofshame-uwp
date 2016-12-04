using System;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using IotWeb.Common.Http;
using IotWeb.Server;
using SirenOfShame.Uwp.Server;
using SirenOfShame.Uwp.Server.Services;
using SirenOfShame.Uwp.Watcher;
using SirenOfShame.Uwp.Watcher.Watcher;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SirenOfShame.Uwp.TestServer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private HttpServer _httpServer;
        private RulesEngine _rulesEngine;
        private SirenDeviceService _sirenDeviceService;
        private readonly StartManager _startManager = new StartManager();
        private readonly ILog _log = MyLogManager.GetLog(typeof(MainPage));

        public MainPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _startManager.Configure();

            _sirenDeviceService = ServiceContainer.Resolve<SirenDeviceService>();

            StartWebServer();
            try
            {
                await StartCiWatcher();
            }
            catch (Exception ex)
            {
                _log.Error("Error starting CI watcher", ex);
            }
        }

        private async Task StartCiWatcher()
        {
            var sosSettings = await SirenOfShameSettingsService.Instance.GetAppSettings();
            _rulesEngine = new RulesEngine(sosSettings);
            _rulesEngine.Start(true);
            _rulesEngine.SetLights += RulesEngineOnSetLights;
        }

        private async void RulesEngineOnSetLights(object sender, SetLightsEventArgs args)
        {
            if (_sirenDeviceService.IsConnected)
            {
                await _sirenDeviceService.PlayLightPattern(args.LedPattern, args.TimeSpan);
            }
        }

        private void StartWebServer()
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
            _sirenDeviceService.StartWatching();
        }
    }
}
