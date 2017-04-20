using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml.Media.Imaging;
using SirenOfShame.Uwp.Ui.Annotations;

namespace SirenOfShame.Uwp.Ui.ViewModels
{
    public sealed class ConfigureWifiViewModel : INotifyPropertyChanged
    {
        private string _statusText;
        private bool _anyNetworkAdapterFound;
        private List<WiFiNetworkDisplay> _networkList;

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                if (value == _statusText) return;
                _statusText = value;
                OnPropertyChanged();
            }
        }

        public bool AnyNetworkAdapterFound
        {
            get { return _anyNetworkAdapterFound; }
            set
            {
                if (value == _anyNetworkAdapterFound) return;
                _anyNetworkAdapterFound = value;
                OnPropertyChanged();
            }
        }

        public List<WiFiNetworkDisplay> NetworkList
        {
            get { return _networkList; }
            set
            {
                if (Equals(value, _networkList)) return;
                _networkList = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateStatus(string status)
        {
            StatusText = status;
        }
    }

    public class WiFiNetworkDisplay : INotifyPropertyChanged
    {
        private readonly WiFiAdapter _adapter;
        public WiFiNetworkDisplay(WiFiAvailableNetwork availableNetwork, WiFiAdapter adapter)
        {
            AvailableNetwork = availableNetwork;
            _adapter = adapter;
            UpdateWiFiImage();
            UpdateConnectivityLevel();
        }

        private void UpdateWiFiImage()
        {
            string imageFileNamePrefix = "secure";
            if (AvailableNetwork.SecuritySettings.NetworkAuthenticationType == NetworkAuthenticationType.Open80211)
            {
                imageFileNamePrefix = "open";
            }

            string imageFileName = string.Format("ms-appx:/Assets/ConfigureWifi/{0}_{1}bar.png", imageFileNamePrefix, AvailableNetwork.SignalBars);

            WiFiImage = new BitmapImage(new Uri(imageFileName));

            OnPropertyChanged("WiFiImage");

        }

        private async void UpdateConnectivityLevel()
        {
            string connectivityLevel = "Not Connected";
            string connectedSsid = null;

            var connectedProfile = await _adapter.NetworkAdapter.GetConnectedProfileAsync();
            if (connectedProfile != null &&
                connectedProfile.IsWlanConnectionProfile &&
                connectedProfile.WlanConnectionProfileDetails != null)
            {
                connectedSsid = connectedProfile.WlanConnectionProfileDetails.GetConnectedSsid();
            }

            if (!string.IsNullOrEmpty(connectedSsid))
            {
                if (connectedSsid.Equals(AvailableNetwork.Ssid))
                {
                    connectivityLevel = connectedProfile.GetNetworkConnectivityLevel().ToString();
                }
            }

            ConnectivityLevel = connectivityLevel;

            OnPropertyChanged("ConnectivityLevel");
        }

        public string Ssid => AvailableNetwork.Ssid;

        public string SecuritySettings => string.Format("Authentication: {0}; Encryption: {1}", AvailableNetwork.SecuritySettings.NetworkAuthenticationType, AvailableNetwork.SecuritySettings.NetworkEncryptionType);

        public string ConnectivityLevel
        {
            get;
            private set;
        }

        public BitmapImage WiFiImage
        {
            get;
            private set;
        }


        public WiFiAvailableNetwork AvailableNetwork { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

}
