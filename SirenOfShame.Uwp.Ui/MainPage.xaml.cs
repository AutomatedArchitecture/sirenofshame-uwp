using System;
using System.Collections.Generic;
using System.Linq;
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
        private AppServiceConnection _WebService;

        public MainPage()
        {
            InitializeComponent();
            SetupAppService();
        }

        private async void SetupAppService()
        {
            var listing = await AppServiceCatalog.
                  FindAppServiceProvidersAsync("BackgroundWebService");

            var packageName = (listing.Count == 1)
                                ? listing[0].PackageFamilyName
                                : string.Empty;

            _WebService = new AppServiceConnection();
            _WebService.AppServiceName = "BackgroundWebService";
            _WebService.PackageFamilyName = packageName;

            var status = await _WebService.OpenAsync();

            if (status != AppServiceConnectionStatus.Success)
            {
                MyText.Text = "Could not connect: " +
                                 status.ToString();
            }
            else
            {
                MyText.Text = "Connected: " + status;
                _WebService.RequestReceived += WebService_RequestReceived;
            }
        }

        private async void WebService_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
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
            await _WebService.SendMessageAsync(
              new ValueSet {
                new KeyValuePair<string, object>("Value", "Hello From UI") });
        }
    }
}
