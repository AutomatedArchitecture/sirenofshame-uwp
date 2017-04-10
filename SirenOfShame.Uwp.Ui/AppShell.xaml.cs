using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SirenOfShame.Uwp.Ui
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppShell : Page
    {
        public AppShell()
        {
            this.InitializeComponent();
        }

        public Frame AppFrame => frame;

        private void TogglePaneButton_Unchecked(object sender, RoutedEventArgs e)
        {
            
        }

        private void TogglePaneButton_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void RootSplitView_PaneClosed(SplitView sender, object args)
        {
            
        }

        private void NavMenuItemContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            
        }

        private void NavMenuList_ItemInvoked(object sender, ListViewItem e)
        {
            
        }

        private void OnNavigatingToPage(object sender, NavigatingCancelEventArgs e)
        {
            
        }
    }
}
