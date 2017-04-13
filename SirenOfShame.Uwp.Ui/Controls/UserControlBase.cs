using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SirenOfShame.Uwp.Ui.Controls
{
    public class UserControlBase : UserControl
    {
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

        protected void Navigate<T>()
        {
            var page = FindParent<Page>();
            page.Frame.Navigate(typeof(T));
        }
    }
}
