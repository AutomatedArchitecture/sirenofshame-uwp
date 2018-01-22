using Windows.UI.Xaml;
using SirenOfShame.Uwp.Core.Services;
using SirenOfShame.Uwp.Server.Services;
using SirenOfShame.Uwp.TestServer.Services;
using SirenOfShame.Uwp.Watcher.Services;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SirenOfShame.Uwp.TestServer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainTestPage
    {
        private readonly ServerStartManager _startManager = new TestServerStartManager();
        private ServerMessageRelayService _messageRelayService;

        public MainTestPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await _startManager.Start();

            _messageRelayService = ServiceContainer.Resolve<ServerMessageRelayService>();
        }

        private async void SendOnClick(object sender, RoutedEventArgs e)
        {
            await _messageRelayService.SendMessageAsync(MessageDestination.AppUi, Title.Text, Body.Text);
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
