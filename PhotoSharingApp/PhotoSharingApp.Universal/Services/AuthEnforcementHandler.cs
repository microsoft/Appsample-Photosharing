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

using System.Threading.Tasks;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Views;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace PhotoSharingApp.Universal.Services
{
    /// <summary>
    /// Handles authentication enforcement.
    /// </summary>
    public class AuthEnforcementHandler : IAuthEnforcementHandler
    {
        private TaskCompletionSource<bool> _contentDialogClosedTaskCompletionSource;
        private readonly ResourceLoader _resourceLoader;
        private TaskCompletionSource<SignInCompletionSource> _signInTaskCompletionSource;

        public AuthEnforcementHandler()
        {
            _resourceLoader = ResourceLoader.GetForCurrentView();

            // We need to be aware of when a sign-in happens.
            AppEnvironment.Instance.CurrentUserChanged += AppEnvironment_CurrentUserChanged;
        }

        private void AppEnvironment_CurrentUserChanged(object sender, User e)
        {
            if (_signInTaskCompletionSource != null)
            {
                _signInTaskCompletionSource.SetResult(SignInCompletionSource.UserChanged);
            }
        }

        /// <summary>
        /// Requires a user to be signed in successfully.
        /// If not signed in already, the user will be prompted to do so.
        /// </summary>
        /// <exception cref="SignInRequiredException">When sign-in was not successful.</exception>
        public async Task CheckUserAuthentication()
        {
            // If there is a user signed in already, we're fine,
            // Otherwise, we need to prompt the user to sign in.
            if (AppEnvironment.Instance.CurrentUser == null)
            {
                // We create a content dialog and host the existing sign-in page
                // control inside of it.
                var contentDialog = new ContentDialog
                {
                    SecondaryButtonText = _resourceLoader.GetString("MessageDialog_Cancel"),
                    Title = _resourceLoader.GetString("SignInRequired_Title")
                };

                // We use the following completion source for making sure the
                // content dialog has closed before proceeding.
                _contentDialogClosedTaskCompletionSource = new TaskCompletionSource<bool>();

                contentDialog.Closed += (s, e) => { _contentDialogClosedTaskCompletionSource.SetResult(true); };

                // Use existing sign-in page as hosted content.
                var signinPage = new SignInPage();
                signinPage.ViewModel.RedirectToProfilePage = false;
                contentDialog.Content = signinPage;

                // We may complete the scenario in two ways:
                // User presses Cancel on the sign-in page, or a successful sign-in has
                // happened. We then need to manually close the existing content dialog as
                // this dialog is independent from the sign-in page.
                _signInTaskCompletionSource = new TaskCompletionSource<SignInCompletionSource>();
                var dialogTask = _signInTaskCompletionSource.Task;

                // Grab the task for showing the dialog.
                var task = contentDialog.ShowAsync();
                task.Completed = (info, status) =>
                {
                    if (_signInTaskCompletionSource != null)
                    {
                        _signInTaskCompletionSource.TrySetResult(SignInCompletionSource.DialogClosed);

                        // We need to reset the completion source after using it, otherwise
                        // sign-out will not be successful.
                        _signInTaskCompletionSource = null;
                    }
                };

                var result = await dialogTask;

                if (result == SignInCompletionSource.UserChanged)
                {
                    var contentDialogClosedTask = _contentDialogClosedTaskCompletionSource.Task;

                    // Close the dialog.
                    task.Cancel();

                    // We need to wait on the content dialog being closed.
                    await contentDialogClosedTask;
                }

                // Sign-in dialog handles failures and cancellations, so we need to check
                // if we have the current user object available.
                if (AppEnvironment.Instance.CurrentUser == null)
                {
                    throw new SignInRequiredException("Sign-in not successful");
                }
            }
        }
    }
}