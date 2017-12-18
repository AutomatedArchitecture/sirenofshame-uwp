using System.Threading.Tasks;
using SirenOfShame.Uwp.Ui.Views;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class GettingStartedService
    {
        private readonly NavigationService _navigationService = ServiceContainer.Resolve<NavigationService>();
        private readonly AppSettingsService _appSettingsService = ServiceContainer.Resolve<AppSettingsService>();

        public async Task InitialAppStartup(string arguments)
        {
            // When the navigation stack isn't restored, navigate to the first page
            // suppressing the initial entrance animation.
            
            var transitionInfo = new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo();

            var networkService = ServiceContainer.Resolve<NetworkService>();
            var isConnected = await networkService.IsConnected();

            if (!isConnected)
            {
                _navigationService.NavigateTo<ConfigureWifi>(arguments, transitionInfo);
            }
            else
            {
                _navigationService.NavigateTo<MainUiPage>(arguments, transitionInfo);
            }
        }

        public bool IsVeryFirstLoad {
            get { return _appSettingsService.IsVeryFirstLoad ?? true; }
            private set { _appSettingsService.IsVeryFirstLoad = value; }
        }

        public void CompleteVeryFirstLoadAction()
        {
            if (IsVeryFirstLoad)
            {
                IsVeryFirstLoad = false;
                _navigationService.NavigateTo<ConfigureServer>();
            }
        }
    }
}
