using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using SirenOfShame.Uwp.Ui.Services;
using SirenOfShame.Uwp.Watcher.Services;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;

namespace SirenOfShame.Uwp.Ui
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            UnhandledException += OnUnhandledException;
            LeavingBackground += OnLeavingBackground;
            EnteredBackground += OnEnteredBackground;
            Suspending += OnSuspending;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var log = MyLogManager.GetLog(typeof(App));
            log.Error("Global unhandeled exception in UI", unhandledExceptionEventArgs.Exception);
        }

        private void OnLeavingBackground(object sender, LeavingBackgroundEventArgs leavingBackgroundEventArgs)
        {
            // todo: UiMessageRelayService.SubscribeEvents(); ?
        }

        private void OnEnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            // todo: UiMessageRelayService.UnSubscribeEvents(); ?
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = false;
            }
#endif

            var startManager = new UiStartManager();
            await startManager.Start();

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is AppShell shell))
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
                    var gettingStartedService = ServiceContainer.Resolve<GettingStartedService>();
                    await gettingStartedService.InitialAppStartup(e.Arguments);
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

            deferral.Complete();
        }
    }
}
