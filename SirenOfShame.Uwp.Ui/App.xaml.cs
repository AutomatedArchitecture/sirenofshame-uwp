using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SirenOfShame.Uwp.Ui.Services;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Ui
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        private readonly MessageRelayService _connection;
        private readonly MessageDistributorService _messageDistributorService;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            UnhandledException += OnUnhandledException;

            var startManager = new StartManager();
            startManager.Start();
            _connection = ServiceContainer.Resolve<MessageRelayService>();
            _messageDistributorService = ServiceContainer.Resolve<MessageDistributorService>();

            LeavingBackground += OnLeavingBackground;
            Suspending += OnSuspending;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var log = MyLogManager.GetLog(typeof(App));
            log.Error("Global unhandeled exception in UI", unhandledExceptionEventArgs.Exception);
        }

        private async void OnLeavingBackground(object sender, LeavingBackgroundEventArgs leavingBackgroundEventArgs)
        {
            var log = MyLogManager.GetLog(typeof(App));
            log.Info("Starting App");
            _messageDistributorService.StartWatching();
            try
            {
                await _connection.Open();
            }
            catch (Exception ex)
            {
                // failing quietly is probably ok for now since the connection will
                //  attempt to re-open itself again on next send.  It just means
                //  we won't be able to receive messages
                log.Error("Error opening connection on startup", ex);
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = false;
            }
#endif
            AppShell shell = Window.Current.Content as AppShell;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (shell == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                shell = new AppShell();

                shell.AppFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }
            }

            // Place our app shell in the current Window
            Window.Current.Content = shell;

            if (e.PrelaunchActivated == false)
            {
                if (shell.AppFrame.Content == null)
                {
                    // When the navigation stack isn't restored, navigate to the first page
                    // suppressing the initial entrance animation.
                    var navigationService = ServiceContainer.Resolve<NavigationService>();
                    var transitionInfo = new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo();
                    navigationService.NavigateTo(typeof(MainUiPage), e.Arguments, transitionInfo);
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            //TODO: Save application state and stop any background activity
            _connection.CloseConnection();

            deferral.Complete();
        }
    }
}
