using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.Networking;
using Windows.Networking.Connectivity;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Services;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class NetworkService
    {
        private readonly Lazy<ILog> _log = new Lazy<ILog, bool>(() => MyLogManager.GetLog(typeof(NetworkService)), true);

        public async Task<bool> IsConnected()
        {
            var adapters = await GetAdapters();
            if (adapters == null) return false;
            foreach (var adapter in adapters)
            {
                var connectedProfile = await adapter.NetworkAdapter.GetConnectedProfileAsync();
                var isConnected = connectedProfile != null;
                if (isConnected) return true;
            }

            return false;
        }

        public async Task<IEnumerable<WiFiAdapter>> GetAdapters()
        {
            var access = await WiFiAdapter.RequestAccessAsync();
            if (access != WiFiAccessStatus.Allowed)
            {
                await _log.Value.Info("Wifi access denied");
                return null;
            }

            var deviceInformationCollection = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
            if (deviceInformationCollection.Count >= 1)
            {
                var tasks = deviceInformationCollection.Select(async i => await WiFiAdapter.FromIdAsync(i.Id)).ToList();
                var wiFiAdapters = await Task.WhenAll(tasks);
                return wiFiAdapters;
            }
            await _log.Value.Warn("No WiFi Adapters detected on this machine.");
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
