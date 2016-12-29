using System;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using IotWeb.Common.Http;
using IotWeb.Server;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Server;
using SirenOfShame.Uwp.Server.Services;
using SirenOfShame.Uwp.Watcher;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Watcher;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SirenOfShame.Uwp.TestServer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainTestPage
    {
        private HttpServer _httpServer;
        private RulesEngine _rulesEngine;
        private SirenDeviceService _sirenDeviceService;
        private readonly StartManager _startManager = new StartManager();
        private readonly ILog _log = MyLogManager.GetLog(typeof(MainTestPage));
        private MessageRelayService _messageRelayService;
        private readonly SirenOfShameSettingsService _sosSettings = ServiceContainer.Resolve<SirenOfShameSettingsService>();

        public MainTestPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _startManager.Configure();

            _messageRelayService = ServiceContainer.Resolve<MessageRelayService>();
            _sirenDeviceService = ServiceContainer.Resolve<SirenDeviceService>();

            InitializeMockCiServer();
            StartWebServer();
            _sirenDeviceService.StartWatching();
            await StartMessageRelayService();
            try
            {
                await StartCiWatcher();
            }
            catch (Exception ex)
            {
                _log.Error("Error starting CI watcher", ex);
            }
        }

        private void InitializeMockCiServer()
        {
        }

        private async Task StartMessageRelayService()
        {
            try
            {
                await _messageRelayService.Open();
            }
            catch (Exception ex)
            {
                _log.Error("Unable to start message rleay service", ex);
            }
        }

        private async Task StartCiWatcher()
        {
            var sosSettings = await _sosSettings.GetAppSettings();
            _rulesEngine = new RulesEngine(sosSettings);
            _rulesEngine.Start(true);
            _rulesEngine.SetLights += RulesEngineOnSetLights;
            _rulesEngine.RefreshStatus += RulesEngineOnRefreshStatus;
            _rulesEngine.NewNewsItem += RulesEngineOnNewNewsItem;
            _rulesEngine.NewUser += RulesEngineOnNewUser;
            _rulesEngine.StatsChanged += RulesEngineOnStatsChanged;
        }

        private async void RulesEngineOnStatsChanged(object sender, StatsChangedEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            await _messageRelayService.Send("StatsChanged", argsAsJson);
        }

        private async void RulesEngineOnNewUser(object sender, NewUserEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            await _messageRelayService.Send("NewUser", argsAsJson);
        }

        private async void RulesEngineOnNewNewsItem(object sender, NewNewsItemEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            await _messageRelayService.Send("NewNewsItem", argsAsJson);
        }

        private async void RulesEngineOnRefreshStatus(object sender, RefreshStatusEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            await _messageRelayService.Send("RefreshStatus", argsAsJson);
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
                new HttpResourceHandler(typeof(MainTestPage).GetTypeInfo().Assembly,
                    "wwwroot", "index.html"));
            _httpServer.AddWebSocketRequestHandler(
                "/sockets/",
                new WebSocketHandler()
                );
            _httpServer.Start();
        }

        private async void SendOnClick(object sender, RoutedEventArgs e)
        {
            await _messageRelayService.Send(Title.Text, Body.Text);
        }

        private void NewNewsOnClick(object sender, RoutedEventArgs e)
        {
            Title.Text = "NewNewsItem";
            Body.Text = FakeData.NewNewsItem;
        }

        private void NewUserOnClick(object sender, RoutedEventArgs e)
        {
            Title.Text = "NewUser";
            Body.Text = FakeData.NewUserItem;
        }

        private void BuildStatusOnClick(object sender, RoutedEventArgs e)
        {
            Title.Text = "RefreshStatus";
            Body.Text = FakeData.GetFakeBuilds();
        }

        private void StatsChangedOnClick(object sender, RoutedEventArgs e)
        {
            Title.Text = "StatsChanged";
            Body.Text = FakeData.StatsChanged;
        }
    }
}
