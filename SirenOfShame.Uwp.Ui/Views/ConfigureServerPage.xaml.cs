using System;
using SirenOfShame.Uwp.Ui.Services;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Ui.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConfigureServerPage
    {
        private readonly NetworkService _networkService = ServiceContainer.Resolve<NetworkService>();
        private readonly NavigationService _navigationService = ServiceContainer.Resolve<NavigationService>();

        public ConfigureServerPage()
        {
            InitializeComponent();

            var myUrl = _networkService.GetPossibleAdminPortals(Environment.NewLine);

            var content = $"Configuring servers is currently only available via the web admin portal.  Please open a url to one of the following URL's: {Environment.NewLine}{Environment.NewLine}{myUrl}";
            Title.Text = content;
            DoneButton.Click += DoneButton_Click;
        }

        private void DoneButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _navigationService.NavigateTo<MainUiPage>();
        }
    }
}
