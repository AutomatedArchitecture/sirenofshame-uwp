using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class NavigationService
    {
        public AppShell AppShell => Window.Current.Content as AppShell;

        public Frame AppFrame => AppShell?.AppFrame;

        public void NavigateTo(Type sourcePageType, object arguments)
        {
            AppFrame.Navigate(sourcePageType, arguments);
            if (sourcePageType == typeof(MainUiPage))
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
