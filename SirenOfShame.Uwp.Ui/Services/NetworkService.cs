using System;
using System.Threading.Tasks;
using Windows.Devices.WiFi;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class NetworkService
    {
        private readonly Lazy<Watcher.ILog> _log = new Lazy<Watcher.ILog, bool>(() => Watcher.MyLogManager.GetLog(typeof(NetworkService)), true);

        public async Task<bool> IsConnected()
        {
            var adapter = await GetAdapter();
            if (adapter == null) return false;
            var connectedProfile = await adapter.NetworkAdapter.GetConnectedProfileAsync();
            var isConnected = connectedProfile != null;
            return isConnected;
        }

        public async Task<WiFiAdapter> GetAdapter()
        {
            var access = await WiFiAdapter.RequestAccessAsync();
            if (access != WiFiAccessStatus.Allowed)
            {
                _log.Value.Info("Wifi access denied");
                return null;
            }

            var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
            if (result.Count >= 1)
            {
                return await WiFiAdapter.FromIdAsync(result[0].Id);
            }
            _log.Value.Warn("No WiFi Adapters detected on this machine.");
            return null;
        }

    }
}
