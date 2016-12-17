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
        private MessageRelayService _messageRelayService;

        public MainPage()
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
            Title.Text = "NewNewsItem";
            Body.Text = @"{
    ""BuildDefinitionId"": null,
    ""EventDate"": ""2016-12-17T09:45:17.2015184-05:00"",
    ""Person"": {
        ""RawName"": ""Bob Shimpty"",
        ""DisplayName"": ""Bob Shimpty"",
        ""TotalBuilds"": 200,
        ""FailedBuilds"": 20,
        ""Hidden"": false,
        ""Achievements"": [],
        ""CumulativeBuildTime"": null,
        ""AvatarId"": null,
        ""NumberOfTimesFixedSomeoneElsesBuild"": 1,
        ""NumberOfTimesPerformedBackToBackBuilds"": 0,
        ""MaxBuildsInOneDay"": 0,
        ""CurrentBuildRatio"": 2.0,
        ""LowestBuildRatioAfter50Builds"": null,
        ""CurrentSuccessInARow"": 2,
        ""Email"": null,
        ""AvatarImageName"": null,
        ""AvatarImageUploaded"": false,
        ""Clickable"": true
    },
    ""Title"": ""Achieved Shame Pusher"",
    ""AvatarImageList"": null,
    ""NewsItemType"": 0,
    ""ReputationChange"": 1,
    ""BuildId"": null,
    ""IsSosOnlineEvent"": true,
    ""ShouldUpdateOldInProgressNewsItem"": false
}";
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
        }

        private async void SendOnClick(object sender, RoutedEventArgs e)
        {
            await _messageRelayService.Send(Title.Text, Body.Text);
        }
    }
}
