using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Models;

namespace SirenOfShame.Uwp.Core.Services
{
    public class UpdateManifestService
    {
        private readonly ILog _log = MyLogManager.GetLog(typeof(UpdateManifestService));
        private const string BASE_URL = "https://sirenofshame.com/DeployMe/";
        public const string SOS_UI = "SirenOfShame.Uwp.Ui-uwp";
        public const string SOS_BACKGROUND = "SirenOfShame.Uwp.Background-uwp";

        public async Task<List<Bundle>> GetManifest()
        {
            await _log.Debug("Retrieving manifest");

            using (var httpClient = new HttpClient())
            {
                var manifestStr = await httpClient.GetStringAsync(new Uri(BASE_URL + "manifest.json"));
                var manifest = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Bundle>>(manifestStr);
                return manifest;
            }
        }

    }
}
