using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SirenOfShame.Uwp.Ui.Controls;
using SirenOfShame.Uwp.Ui.Models;
using SirenOfShame.Uwp.Ui.Services;
using SirenOfShame.Uwp.Ui.Views;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Ui
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppShell
    {
        private static readonly NavigationService _navigationService = ServiceContainer.Resolve<NavigationService>();

        // All Icons: https://docs.microsoft.com/en-us/windows/uwp/style/segoe-ui-symbol-font
        // Symbols: https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.Symbol
        private readonly List<NavMenuItem> _navlist = new List<NavMenuItem>(
            new[]
            {
                new NavMenuItem()
                {
                    Symbol = Symbol.Home,
                    Label = "Home",
                    DestPage = typeof(Views.MainUiPage),
                    IsSelected = true
                },
                new NavMenuItem()
                {
                    SymbolChar = (char) 0xE211,
                    Label = "Configure Server",
                    DestPage = typeof(ConfigureServerPage)
                },
                new NavMenuItem()
                {
                    SymbolChar = (char) 0xE15E,
                    Label = "View Logs",
                    DestPage = typeof(ViewLogsPage)
                },
            });

        public Frame AppFrame => frame;

        public bool IsPaneOpen
        {
            get { return RootSplitView.IsPaneOpen; }
            set { RootSplitView.IsPaneOpen = value; }
        }

        public AppShell()
        {
            InitializeComponent();
            Loaded += AppShell_Loaded;

            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;
            ShowBackButton();

            RootSplitView.RegisterPropertyChangedCallback(
                SplitView.DisplayModeProperty,
                (s, a) =>
                {
                    // Ensure that we update the reported size of the TogglePaneButton when the SplitView's
                    // DisplayMode changes.
                    CheckTogglePaneButtonSizeChanged();
                });

            NavMenuList.ItemsSource = _navlist;
        }

        private static void ShowBackButton()
        {
            _navigationService.ShowBackButton();
        }

        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            bool handled = e.Handled;
            BackRequested(ref handled);
            e.Handled = handled;
        }

        private void BackRequested(ref bool handled)
        {
            // Get a hold of the current frame so that we can inspect the app back stack.

            if (AppFrame == null)
                return;

            // Check to see if this is the top-most page on the app back stack.
            if (AppFrame.CanGoBack && !handled)
            {
                // If not, set the event to handled and go back to the previous page in the app.
                handled = true;
                AppFrame.GoBack();
                ShowBackButton();
            }
        }

        private void AppShell_Loaded(object sender, RoutedEventArgs e)
        {
            CheckTogglePaneButtonSizeChanged();
            IsPaneOpen = false;

            var item = _navlist[0];
            var container = (ListViewItem)NavMenuList.ContainerFromItem(item);
            NavMenuList.SetSelectedItem(container);

            // remove the thick border caused by FocusState.Keyboard
            TogglePaneButton.Focus(FocusState.Pointer);
        }

        private void TitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            // for tablet support maybe someday
        }

        private void TogglePaneButton_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckTogglePaneButtonSizeChanged();
        }

        private void TogglePaneButton_Checked(object sender, RoutedEventArgs e)
        {
            //NavPaneDivider.Visibility = Visibility.Visible;
            CheckTogglePaneButtonSizeChanged();
            SettingsNavPaneButton.IsTabStop = true;
        }

        private void RootSplitView_PaneClosed(SplitView sender, object args)
        {
            NavPaneDivider.Visibility = Visibility.Collapsed;

            SettingsNavPaneButton.IsTabStop = false;
        }

        /// <summary>
        /// Enable accessibility on each nav menu item by setting the AutomationProperties.Name on each container
        /// using the associated Label of each item.
        /// </summary>
        private void NavMenuItemContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (!args.InRecycleQueue && args.Item is NavMenuItem)
            {
                args.ItemContainer.SetValue(AutomationProperties.NameProperty, ((NavMenuItem)args.Item).Label);
            }
            else
            {
                args.ItemContainer.ClearValue(AutomationProperties.NameProperty);
            }
        }

        private void NavMenuList_ItemInvoked(object sender, ListViewItem listViewItem)
        {
            foreach (var i in _navlist)
            {
                i.IsSelected = false;
            }

            var item = (NavMenuItem)((NavMenuListView)sender).ItemFromContainer(listViewItem);

            if (item != null)
            {
                item.IsSelected = true;
                if (item.DestPage != null &&
                    item.DestPage != AppFrame.CurrentSourcePageType)
                {
                    _navigationService.NavigateTo(item.DestPage, item.Arguments);
                }
            }
        }

        private void OnNavigatingToPage(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                var item = _navlist.SingleOrDefault(p => p.DestPage == e.SourcePageType);
                if (item == null && AppFrame.BackStackDepth > 0)
                {
                    // In cases where a page drills into sub-pages then we'll highlight the most recent
                    // navigation menu item that appears in the BackStack
                    foreach (var entry in AppFrame.BackStack.Reverse())
                    {
                        item = (from p in _navlist where p.DestPage == entry.SourcePageType select p).SingleOrDefault();
                        if (item != null)
                            break;
                    }
                }

                foreach (var i in _navlist)
                {
                    i.IsSelected = false;
                }
                if (item != null)
                {
                    item.IsSelected = true;
                }

                var container = (ListViewItem)NavMenuList.ContainerFromItem(item);

                // While updating the selection state of the item prevent it from taking keyboard focus.  If a
                // user is invoking the back button via the keyboard causing the selected nav menu item to change
                // then focus will remain on the back button.
                if (container != null) container.IsTabStop = false;
                NavMenuList.SetSelectedItem(container);
                if (container != null) container.IsTabStop = true;
            }



        }

        /// <summary>
        /// Check for the conditions where the navigation pane does not occupy the space under the floating
        /// hamburger button and trigger the event.
        /// </summary>
        private void CheckTogglePaneButtonSizeChanged()
        {
            // meh, for tablet support maybe someday
        }

        private void SettingsOnClick(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo(typeof(ConfigureWifiPage), null);
        }
    }
}