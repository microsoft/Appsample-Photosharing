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
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Lifecycle;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Unity;
using PhotoSharingApp.Universal.ViewModels;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Globalization;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PhotoSharingApp.Universal
{
    /// <summary>
    /// Share target extensions for the app.
    /// </summary>
    public partial class App : Application
    {
        private Frame GetOrCreateRootFrame()
        {
            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                // Set the default language
                rootFrame.Language = ApplicationLanguages.Languages[0];

                rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            return rootFrame;
        }

        /// <summary>
        /// Invoked when the application is activated through sharing association.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            IAppEnvironment mainAppEnvironment = null;
            IUploadFinishedHandler mainUploadFinishedHandler = null;

            if (UnityBootstrapper.Container != null)
            {
                // We need to save the app environment from the current app.
                mainAppEnvironment = ServiceLocator.Current.GetInstance<IAppEnvironment>();
                mainUploadFinishedHandler = ServiceLocator.Current.GetInstance<IUploadFinishedHandler>();
            }
            else
            {
                // We only configure unity if the main app is not already launched.
                UnityBootstrapper.Init();
                UnityBootstrapper.ConfigureRegistries();
            }

            // Overwrite existing app environment instance.
            UnityBootstrapper.Container.RegisterInstance(typeof(IAppEnvironment), new AppEnvironment());

            // Do initializations, trying restoring current user into the
            // provided app environment.
            await AppInitialization.DoInitializations();

            // Overwrite existing upload finished handler to make sure, the app
            // does not navigate to the categories page after successful upload,
            // but closes the share target window instead.
            var uploadFinishedHandler =
                new ShareTargetUploadFinishedHandler(() =>
                {
                    // Restore original app environment.
                    if (mainAppEnvironment != null)
                    {
                        UnityBootstrapper.Container.RegisterInstance(typeof(IAppEnvironment), mainAppEnvironment);
                    }

                    // Restore original upload finished handler.
                    if (mainUploadFinishedHandler != null)
                    {
                        var uploadFinishedHandlerType = mainUploadFinishedHandler.GetType();
                        UnityBootstrapper.Container.RegisterType(typeof(IUploadFinishedHandler),
                            uploadFinishedHandlerType);
                    }

                    args.ShareOperation.ReportCompleted();
                });

            UnityBootstrapper.Container.RegisterInstance(typeof(IUploadFinishedHandler), uploadFinishedHandler);

            // Access data that has been shared.
            var data = args.ShareOperation.Data;

            if (data.Contains(StandardDataFormats.StorageItems))
            {
                try
                {
                    var items = await data.GetStorageItemsAsync();

                    // We expect the shared data to be a StorageFile.
                    var file = items.FirstOrDefault() as StorageFile;

                    if (file != null)
                    {
                        GetOrCreateRootFrame();

                        var navigationFacade = ServiceLocator.Current.GetInstance<INavigationFacade>();
                        navigationFacade.NavigateToCropView(file);

                        Window.Current.Activate();
                    }
                }
                catch (Exception)
                {
                    args.ShareOperation.ReportCompleted();
                }
            }
        }
    }
}