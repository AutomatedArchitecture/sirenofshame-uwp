using Windows.UI.Xaml.Controls;
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
            Navigate<ViewUser>();
        }
    }
}
