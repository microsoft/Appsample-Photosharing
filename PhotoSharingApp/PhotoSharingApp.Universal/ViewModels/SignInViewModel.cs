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
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Views;
using Windows.ApplicationModel.Resources;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The ViewModel for the sign-in view.
    /// </summary>
    public class SignInViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationFacade _navigationFacade;
        private readonly IPhotoService _photoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignInViewModel" /> class.
        /// </summary>
        /// <param name="navigationFacade">The navigation facade.</param>
        /// <param name="photoService">The photo service.</param>
        /// <param name="dialogService">The dialog service.</param>
        public SignInViewModel(INavigationFacade navigationFacade, IPhotoService photoService,
            IDialogService dialogService)
        {
            _navigationFacade = navigationFacade;
            _photoService = photoService;
            _dialogService = dialogService;

            // Initialize commands
            ChooseAuthProviderCommand = new RelayCommand<MobileServiceAuthenticationProvider>(OnChooseAuthProvider);

            // Initialize auth providers
            AuthenticationProviders = photoService.GetAvailableAuthenticationProviders();
        }

        /// <summary>
        /// Gets or sets the authentication providers.
        /// </summary>
        /// <value>
        /// The authentication providers.
        /// </value>
        public List<MobileServiceAuthenticationProvider> AuthenticationProviders { get; set; }

        /// <summary>
        /// Gets the authentication reassurance message.
        /// </summary>
        public string AuthenticationReassuranceMessage { get; } = ResourceLoader.GetForCurrentView().GetString("SignInPage_ReassuranceMessage");

        /// <summary>
        /// Gets the choose authentication provider command.
        /// </summary>
        /// <value>
        /// The choose authentication provider command.
        /// </value>
        public RelayCommand<MobileServiceAuthenticationProvider> ChooseAuthProviderCommand { get; private set; }

        /// <summary>
        /// Enables or disables the trigger to redirect
        /// to the profile page after a successful sign-in.
        /// 
        /// The default use case is that users will directly navigate to the
        // sign-in page which should redirect to the profile page.
        // Alternatively, users can sign-in using the sign-in
        // dialog, which should not do any redirections.
        /// </summary>
        public bool RedirectToProfilePage { get; set; } = true;

        private async void OnChooseAuthProvider(MobileServiceAuthenticationProvider authenticationProviderProvider)
        {
            try
            {
                await _photoService.SignInAsync(authenticationProviderProvider);

                if (RedirectToProfilePage)
                {
                    _navigationFacade.NavigateToProfileView();
                    _navigationFacade.RemoveBackStackFrames(1);
                }
            }
            catch (AuthenticationException)
            {
                await _dialogService.ShowNotification("AuthenticationFailed_Message", "AuthenticationFailed_Title");
            }
            catch (AuthenticationCanceledException)
            {
                // User canceled, do nothing in this case.
            }
            catch (Exception)
            {
                await _dialogService.ShowNotification("GenericError_Title", "GenericError_Message");
            }
        }
    }
}