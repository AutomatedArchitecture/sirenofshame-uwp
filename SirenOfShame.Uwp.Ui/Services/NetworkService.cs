using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.Networking;
using Windows.Networking.Connectivity;

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

        public string GetPossibleAdminPortals(string separator)
        {
            var myIps = GetMyIps();
            return string.Join(separator, myIps);
        }

        private string GetAdminPortalAddress(string ipAddress)
        {
            return $"http://{ipAddress}/";
        }

        private string[] GetMyIps()
        {
            return NetworkInformation.GetHostNames()
                .Where(i => i.IPInformation != null && 
                    (i.Type == HostNameType.Ipv4) || (i.Type == HostNameType.DomainName))
                .Select(i => i.ToString())
                .Select(GetAdminPortalAddress)
                .ToArray();
        }
    }
}
