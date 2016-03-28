//-----------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Lifecycle;
using PhotoSharingApp.Universal.Unity;
using PhotoSharingApp.Universal.Views;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Globalization;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

[assembly: InternalsVisibleTo("PhotoSharingApp.Universal.Tests")]

namespace PhotoSharingApp.Universal
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            // Register for events
            Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// Navigate to the Photo details page for which the Gold was received.
        /// </summary>
        /// <param name="args">The notification activation event args.</param>
        private void GoldReceivedNotificationClickedHandler(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.ToastNotification)
            {
                // Get the pre-defined arguments from the eventargs.
                // Push Notification for Gold reception sends the photoId of that Photo.
                var toastArgs = args as ToastNotificationActivatedEventArgs;
                var photoId = toastArgs.Argument;

                var facade = ServiceLocator.Current.GetInstance<INavigationFacade>();

                if (!photoId.Equals(string.Empty))
                {
                    facade.NavigateToPhotoDetailsView(photoId);
                }
                else
                {
                    facade.NavigateToCategoriesView();
                }

                // Register launch if the App is not already Running or Suspended
                if (args.PreviousExecutionState.Equals(ApplicationExecutionState.NotRunning)
                    || args.PreviousExecutionState.Equals(ApplicationExecutionState.Terminated)
                    || args.PreviousExecutionState.Equals(ApplicationExecutionState.ClosedByUser))
                {
                    AppLaunchCounter.RegisterLaunch();
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Initialize the App launch.
        /// </summary>
        /// <returns>The AppShell of the app.</returns>
        private async Task<AppShell> Initialize()
        {
            var shell = Window.Current.Content as AppShell;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (shell == null)
            {
                UnityBootstrapper.Init();
                UnityBootstrapper.ConfigureRegistries();

                await AppInitialization.DoInitializations();

                // Create a AppShell to act as the navigation context and navigate to the first page
                shell = new AppShell();

                // Set the default language
                shell.Language = ApplicationLanguages.Languages[0];

                shell.AppFrame.NavigationFailed += OnNavigationFailed;
            }

            return shell;
        }

        /// <summary>
        /// Invoked when the application is activated by some means other than normal launching.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            var shell = Window.Current.Content as AppShell;

            // Check if the application needs to be initialized.
            if (shell == null)
            {
                shell = await Initialize();
            }

            // Place our app shell in the current Window
            Window.Current.Content = shell;

            GoldReceivedNotificationClickedHandler(args);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            var shell = await Initialize();

            // Place our app shell in the current Window
            Window.Current.Content = shell;

            if (shell.AppFrame.Content == null)
            {
                var facade = ServiceLocator.Current.GetInstance<INavigationFacade>();

                if (AppLaunchCounter.IsFirstLaunch())
                {
                    facade.NavigateToWelcomeView();
                }
                else
                {
                    facade.NavigateToCategoriesView();
                }
            }

            // Refresh launch counter, needs to be done
            // after AppLaunchCounter.IsFirstLaunch() is being checked.
            AppLaunchCounter.RegisterLaunch();

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
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
            deferral.Complete();
        }

        /// <summary>
        /// Invoked when the app runs into an unhandled exception. Telemetry logs the
        /// unhandled exception and appropriate message is displayed to the user experiencing
        /// the exception.
        /// </summary>
        /// <param name="sender">The source of the unhandled exception.</param>
        /// <param name="e">Details about the unhandled exception event.</param>
        private async void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            var resourceLoader = ResourceLoader.GetForCurrentView();
            var dialog = new MessageDialog(resourceLoader.GetString("UnexpectedError_Message"),
                resourceLoader.GetString("UnexpectedError_Title"));

            await dialog.ShowAsync();
        }
    }
}