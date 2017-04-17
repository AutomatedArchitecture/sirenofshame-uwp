using System;
using System.Linq;
using Windows.Devices.WiFi;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using SirenOfShame.Uwp.Ui.Services;
using SirenOfShame.Uwp.Ui.ViewModels;

namespace SirenOfShame.Uwp.Ui.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConfigureWifi
    {
        private readonly ILog _log = MyLogManager.GetLog(typeof(ConfigureWifi));
        private WiFiAdapter _firstAdapter;

        public ConfigureWifi()
        {
            InitializeComponent();
            DataContext = new ConfigureWifiViewModel();
        }

        private ConfigureWifiViewModel ViewModel => (ConfigureWifiViewModel) DataContext;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.AnyNetworkAdapterFound = false;
            var access = await WiFiAdapter.RequestAccessAsync();
            if (access != WiFiAccessStatus.Allowed)
            {
                _log.Info("Wifi access denied");
                return;
            }

            var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
            if (result.Count >= 1)
            {
                _firstAdapter = await WiFiAdapter.FromIdAsync(result[0].Id);
                ViewModel.AnyNetworkAdapterFound = true;
            }
            else
            {
                _log.Warn("No WiFi Adapters detected on this machine.");
            }
        }

        private async void ScanWifiOnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.UpdateStatus("Scanning for WiFi Networks");
            await _firstAdapter.ScanAsync();
            ViewModel.UpdateStatus(null);
            var networkReport = _firstAdapter.NetworkReport;
            ViewModel.NetworkList = networkReport.AvailableNetworks.ToList();
        }
    }
}
