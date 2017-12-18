using Windows.UI.Xaml.Controls;
using SirenOfShame.Uwp.Ui.Models;
using SirenOfShame.Uwp.Ui.Services;
using SirenOfShame.Uwp.Ui.Views;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Ui.Controls
{
    public sealed partial class Leaders
    {
        private readonly NavigationService _navigationService = ServiceContainer.Resolve<NavigationService>();

        public Leaders()
        {
            InitializeComponent();
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var person = e.ClickedItem as PersonDto;
            _navigationService.NavigateTo<ViewUserPage>(person);
        }
    }
}
