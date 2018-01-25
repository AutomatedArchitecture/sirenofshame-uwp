using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Models;
using SirenOfShame.Uwp.Core.Services;
using SirenOfShame.Uwp.Shared.Commands;
using SirenOfShame.Uwp.Ui.Services;
using SirenOfShame.Uwp.Watcher.Services;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SirenOfShame.Uwp.Ui.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConfigureAppPage
    {
        private readonly UpdateManifestService _updateManifestService;
        private readonly ILog _log = MyLogManager.GetLog(typeof(ConfigureAppPage));
        private readonly NavigationService _navigationService;
        private readonly UiMessageRelayService _uiMessageRelayService = ServiceContainer.Resolve<UiMessageRelayService>();

        public ConfigureAppPage()
        {
            _updateManifestService = new UpdateManifestService();
            _navigationService = ServiceContainer.Resolve<NavigationService>();

            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var packageVersion = Package.Current.Id.Version;
            VersionTextBlock.Text = $"v{packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}.{packageVersion.Revision}";

            await CheckForUpdates(packageVersion);
        }

        private async Task CheckForUpdates(PackageVersion packageVersion)
        {
            var uiBundle = await GetUiBundle();
            if (uiBundle == null)
            {
                UpdatesTextBlock.Text = "Error checking for updates";
                return;
            }

            var installedVersion = ToVersion(packageVersion);
            var uiUpdatesAvailable = uiBundle.Version > installedVersion;
            UpgradeNowButton.Visibility = Visibility.Visible;
            UpgradeNowButton.Content = uiUpdatesAvailable ? "Upgrade Now" : "Try To Upgrade Anyway";
            if (uiUpdatesAvailable)
            {
                UpdatesTextBlock.Text = "Updates available.  Server is at " + uiBundle.Version;
            }
            else
            {
                UpdatesTextBlock.Text = "No updates are available for the UI project.";
            }
        }

        private async Task<Bundle> GetUiBundle()
        {
            try
            {
                var bundles = await _updateManifestService.GetManifest();
                var uiBundle = bundles.FirstOrDefault(i => i.Id == UpdateManifestService.SOS_UI);
                if (uiBundle == null)
                    await _log.Error("No log found in manifest matching " + UpdateManifestService.SOS_UI);
                return uiBundle;
            }
            catch (HttpRequestException ex)
            {
                await _log.Warn("Error retrieving manifest", ex);
                return null;
            }
        }

        private Version ToVersion(PackageVersion installedVersion)
        {
            return new Version(installedVersion.Major, installedVersion.Minor, installedVersion.Build, installedVersion.Revision);
        }

        private void ViewLogsOnClick(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo<ViewLogsPage>();
        }

        private async void UpgradeNowOnClick(object sender, RoutedEventArgs e)
        {
            await _uiMessageRelayService.SendMessageAsync(MessageDestination.Maintenance, CommandNames.TRY_UPGRADE, null);
        }
    }
}
