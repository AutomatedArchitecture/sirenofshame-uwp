using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class NavigationService
    {
        private AppShell AppShell => Window.Current.Content as AppShell;

        private Frame AppFrame => AppShell?.AppFrame;

        public void NavigateTo(Type destination, object arguments)
        {
            AppFrame.Navigate(destination, arguments);
            PostNavigate(destination);
        }

        public void NavigateTo(Type type, string arguments, NavigationTransitionInfo navigationTransitionInfo)
        {
            AppFrame.Navigate(type, arguments, navigationTransitionInfo);
            PostNavigate(type);
        }

        private void PostNavigate(Type destination)
        {
            if (destination == typeof(MainUiPage))
            {
                AppFrame.BackStack.Clear();
            }
            ShowBackButton();
            AppShell.IsPaneOpen = false;
        }

        public void ShowBackButton()
        {
            var visible = AppFrame?.CanGoBack ?? false;
            ShowBackButton(visible);
        }

        private void ShowBackButton(bool visible)
        {
            var visibility = visible ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = visibility;
        }
    }
}
