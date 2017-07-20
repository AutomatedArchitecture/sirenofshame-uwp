using System;
using System.Linq;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            var networks = networkReport.AvailableNetworks
                .Select(network => new WiFiNetworkDisplay(network, _firstAdapter))
                .ToList();
            ViewModel.NetworkList = networks;
        }

        private void NetworkSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedNetwork = ResultsListView.SelectedItem as WiFiNetworkDisplay;
            if (selectedNetwork == null)
            {
                return;
            }

            // Show the connection bar
            ShowNetworkInfoSection(true);

            // Only show the password box if needed
            if (selectedNetwork.AvailableNetwork.SecuritySettings.NetworkAuthenticationType == NetworkAuthenticationType.Open80211 &&
                    selectedNetwork.AvailableNetwork.SecuritySettings.NetworkEncryptionType == NetworkEncryptionType.None)
            {
                NetworkKeyInfo.Visibility = Visibility.Collapsed;
            }
            else
            {
                NetworkKeyInfo.Visibility = Visibility.Visible;
            }
        }

        private void SetVisible(FrameworkElement control, bool visible)
        {
            control.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ShowNetworkInfoSection(bool showNetworkInfoSection)
        {
            SetVisible(ConnectionBar, showNetworkInfoSection);
            SetVisible(NetworkListPanel, !showNetworkInfoSection);
        }

        private void ConnectButtonOnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void CancelButtonOnClick(object sender, RoutedEventArgs e)
        {
            ShowNetworkInfoSection(false);
        }
    }
}
