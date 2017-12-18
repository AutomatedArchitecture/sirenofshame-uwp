using System;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml.Controls;
using SirenOfShame.Uwp.Ui.Services;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Ui.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConfigureServer
    {
        private readonly NetworkService _networkService = ServiceContainer.Resolve<NetworkService>();

        public ConfigureServer()
        {
            InitializeComponent();

            var myUrl = _networkService.GetPossibleAdminPortals();

            var content = $"Configuring servers is currently only available via the web admin portal.{Environment.NewLine}Please open a url to: {myUrl}";
            Title.Text = content;
        }
    }
}
