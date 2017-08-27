using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SirenOfShame.Uwp.Ui.Services;
using SirenOfShame.Uwp.Ui.ViewModels;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Ui.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConfigureWifi
    {
        private readonly ILog _log = MyLogManager.GetLog(typeof(ConfigureWifi));
        private WiFiAdapter _firstAdapter;
        private readonly NetworkService _networkService = ServiceContainer.Resolve<NetworkService>();

        public ConfigureWifi()
        {
            InitializeComponent();
            DataContext = new ConfigureWifiViewModel();
        }

        private ConfigureWifiViewModel ViewModel => (ConfigureWifiViewModel) DataContext;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _firstAdapter = await _networkService.GetAdapter();
            ViewModel.AnyNetworkAdapterFound = _firstAdapter != null;
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

        private async void ConnectButtonOnClick(object sender, RoutedEventArgs e)
        {
            var selectedNetwork = ResultsListView.SelectedItem as WiFiNetworkDisplay;
            if (selectedNetwork == null || _firstAdapter == null)
            {
                _log.Warn("Network not selcted");
                return;
            }
            WiFiReconnectionKind reconnectionKind = WiFiReconnectionKind.Manual;
            if (IsAutomaticReconnection.IsChecked.HasValue && IsAutomaticReconnection.IsChecked == true)
            {
                reconnectionKind = WiFiReconnectionKind.Automatic;
            }

            WiFiConnectionResult result;
            if (selectedNetwork.AvailableNetwork.SecuritySettings.NetworkAuthenticationType == Windows.Networking.Connectivity.NetworkAuthenticationType.Open80211)
            {
                result = await _firstAdapter.ConnectAsync(selectedNetwork.AvailableNetwork, reconnectionKind);
            }
            else
            {
                // Only the password potion of the credential need to be supplied
                var credential = new PasswordCredential();
                credential.Password = NetworkKey.Password;

                result = await _firstAdapter.ConnectAsync(selectedNetwork.AvailableNetwork, reconnectionKind, credential);
            }

            if (result.ConnectionStatus == WiFiConnectionStatus.Success)
            {
                _log.Info(string.Format("Successfully connected to {0}.", selectedNetwork.Ssid));

                // todo: navigate to the home page perhaps?
                //// refresh the webpage
                //webViewGrid.Visibility = Visibility.Visible;
                //toggleBrowserButton.Content = "Hide Browser Control";
                //refreshBrowserButton.Visibility = Visibility.Visible;

            }
            else
            {
                _log.Warn(string.Format("Could not connect to {0}. Error: {1}", selectedNetwork.Ssid, result.ConnectionStatus));
            }

            // Since a connection attempt was made, update the connectivity level displayed for each
            foreach (var network in ViewModel.NetworkList)
            {
                network.UpdateConnectivityLevel();
            }
        }

        private void CancelButtonOnClick(object sender, RoutedEventArgs e)
        {
            ShowNetworkInfoSection(false);
        }
    }
}
