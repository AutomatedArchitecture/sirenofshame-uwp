using System;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml.Controls;

namespace SirenOfShame.Uwp.Ui.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConfigureServer
    {
        public ConfigureServer()
        {
            InitializeComponent();

            var url = GetIpAddress();
            var content = $"Configuring servers is currently only available via the web admin portal.{Environment.NewLine}Please open a url to http://{url}/";
            Title.Text = content;
        }

        private string GetIpAddress()
        {
            foreach (HostName localHostName in NetworkInformation.GetHostNames())
            {
                if (localHostName.IPInformation != null)
                {
                    if (localHostName.Type == HostNameType.Ipv4)
                    {
                        return localHostName.ToString();
                    }
                }
            }
            return "[url]";
        }

    }
}
