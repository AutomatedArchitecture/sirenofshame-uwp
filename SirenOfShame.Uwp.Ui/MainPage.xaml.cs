using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SirenOfShame.Uwp.Ui
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private AppServiceConnection _connection;

        public MainPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                await SetupAppService();
            }
            catch (Exception ex)
            {
                MyText.Text = "AppService Error: " + ex.Message;
            }
        }

        private async Task SetupAppService()
        {
            var appServiceName = "SirenOfShameMessageRelay";
            var listing = await AppServiceCatalog.FindAppServiceProvidersAsync(appServiceName);

            if (listing.Count == 0)
            {
                throw new Exception("Unable to find app service '" + appServiceName + "'");
            }
            var packageName = listing[0].PackageFamilyName;

            _connection = new AppServiceConnection
            {
                AppServiceName = appServiceName,
                PackageFamilyName = packageName
            };

            var status = await _connection.OpenAsync();

            if (status != AppServiceConnectionStatus.Success)
            {
                MyText.Text = "Could not connect: " +
                                 status.ToString();
            }
            else
            {
                MyText.Text = "Connected: " + status;
                _connection.RequestReceived += ConnectionRequestReceived;
            }
        }

        private async void ConnectionRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
              {
                  var message = args.Request.Message.First();
                  MyText.Text = $"{message.Key}={message.Value}";
              });
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MyText.Text = "Sending Message";
            await _connection.SendMessageAsync(
              new ValueSet {
                new KeyValuePair<string, object>("Value", "Hello From UI") });
        }
    }
}
