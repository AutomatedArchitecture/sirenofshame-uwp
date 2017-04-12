using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SirenOfShame.Uwp.Ui.Views;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SirenOfShame.Uwp.Ui.Controls
{
    public sealed partial class Leaders
    {
        public Leaders()
        {
            this.InitializeComponent();
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var page = FindParent<Page>();
            page.Frame.Navigate(typeof(ViewUser));
        }

        private T FindParent<T>() where T : FrameworkElement
        {
            return FindParent<T>(this);
        }

        private T FindParent<T>(FrameworkElement element) where T : FrameworkElement
        {
            if (element == null) return default(T);
            var parent = element.Parent as FrameworkElement;
            var parentAsT = parent as T;
            if (parentAsT != null)
                return parentAsT;
            return FindParent<T>(parent);
        }
    }
}
