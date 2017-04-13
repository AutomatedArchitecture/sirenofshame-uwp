using Windows.UI.Xaml.Controls;
using SirenOfShame.Uwp.Ui.Models;
using SirenOfShame.Uwp.Ui.Views;

namespace SirenOfShame.Uwp.Ui.Controls
{
    public sealed partial class Leaders
    {
        public Leaders()
        {
            InitializeComponent();
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var person = e.ClickedItem as PersonDto;
            Navigate<ViewUser>(person);
        }
    }
}
