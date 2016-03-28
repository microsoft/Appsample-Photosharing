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
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Views;
using Windows.System;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The ViewModel for the settings view.
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationFacade _navigationFacade;
        private readonly IPhotoService _photoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel" /> class.
        /// </summary>
        /// <param name="photoService">The photo service.</param>
        /// <param name="navigationFacade">The navigation facade.</param>
        /// <param name="dialogService">The dialog service.</param>
        public SettingsViewModel(IPhotoService photoService, INavigationFacade navigationFacade,
            IDialogService dialogService)
        {
            _photoService = photoService;
            _navigationFacade = navigationFacade;
            _dialogService = dialogService;

            PrivacyCommand = new RelayCommand(OnShowPrivacyPolicy);
            AboutCommand = new RelayCommand(OnShowAbout);
            SignOutCommand = new RelayCommand(OnSignOut);
        }

        /// <summary>
        /// Gets the about command
        /// </summary>
        public RelayCommand AboutCommand { get; private set; }

        /// <summary>
        /// Returns true, if user is signed in. Otherwise, false.
        /// </summary>
        public bool IsUserSignedIn
        {
            get { return AppEnvironment.Instance.CurrentUser != null; }
        }

        /// <summary>
        /// Gets the privacy policy command.
        /// </summary>
        public RelayCommand PrivacyCommand { get; private set; }

        /// <summary>
        /// Gets the sign out command.
        /// </summary>
        public RelayCommand SignOutCommand { get; private set; }

        private void OnShowAbout()
        {
            _navigationFacade.NavigateToAboutView();
        }

        private async void OnShowPrivacyPolicy()
        {
            await Launcher.LaunchUriAsync(new Uri("http://Your_Privacy_Page.com"));
        }

        private async void OnSignOut()
        {
            try
            {
                await _photoService.SignOutAsync();

                // Resetting the current user.
                AppEnvironment.Instance.CurrentUser = null;

                NotifyPropertyChanged(nameof(IsUserSignedIn));
            }
            catch (Exception)
            {
                await _dialogService.ShowGenericServiceErrorNotification();
            }
        }
    }
}