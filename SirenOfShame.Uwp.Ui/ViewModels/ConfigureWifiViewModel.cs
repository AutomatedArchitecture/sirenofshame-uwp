using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.WiFi;
using SirenOfShame.Uwp.Ui.Annotations;

namespace SirenOfShame.Uwp.Ui.ViewModels
{
    public sealed class ConfigureWifiViewModel : INotifyPropertyChanged
    {
        private string _statusText;
        private bool _anyNetworkAdapterFound;
        private List<WiFiAvailableNetwork> _networkList;

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

        public List<WiFiAvailableNetwork> NetworkList
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
}
