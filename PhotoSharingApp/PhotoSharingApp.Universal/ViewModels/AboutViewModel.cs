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
using System.Reflection;
using System.Threading.Tasks;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Extensions;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Views;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The ViewModel for about view.
    /// </summary>
    public class AboutViewModel : ViewModelBase
    {
        private string _appVersion;
        private string _assemblyVersion;
        private readonly IDialogService _dialogService;
        private bool _isBusy;
        private readonly IPhotoService _photoService;
        private string _serverVersion;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="photoService">The photo service.</param>
        /// <param name="dialogService">The dialog service.</param>
        public AboutViewModel(IPhotoService photoService, IDialogService dialogService)
        {
            _photoService = photoService;
            _dialogService = dialogService;

            // Read package version
            AppVersion = Package.Current.Id.Version.ToFormattedString();

            // Read assembly version
            var assembly = GetType().GetTypeInfo().Assembly;
            var versionAttribute =
                assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            AssemblyVersion = versionAttribute?.Version;

            // Initialize commands
            CopyUserIdToClipboardCommand = new RelayCommand(OnCopyUserIdToClipboard,
                () => AppEnvironment.Instance.CurrentUser != null);
        }

        /// <summary>
        /// Gets or sets the app package version.
        /// </summary>
        public string AppVersion
        {
            get { return _appVersion; }
            set
            {
                if (value != _appVersion)
                {
                    _appVersion = value;
                    NotifyPropertyChanged(nameof(AppVersion));
                }
            }
        }

        /// <summary>
        /// Gets or sets the assembly version
        /// </summary>
        public string AssemblyVersion
        {
            get { return _assemblyVersion; }
            set
            {
                if (value != _assemblyVersion)
                {
                    _assemblyVersion = value;
                    NotifyPropertyChanged(nameof(AssemblyVersion));
                }
            }
        }

        /// <summary>
        /// Gets the command for copying the User Id to the
        /// clipboard.
        /// </summary>
        public RelayCommand CopyUserIdToClipboardCommand { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (value != _isBusy)
                {
                    _isBusy = value;
                    NotifyPropertyChanged(nameof(IsBusy));
                }
            }
        }

        /// <summary>
        /// Gets or sets the server version.
        /// </summary>
        public string ServerVersion
        {
            get { return _serverVersion; }
            set
            {
                if (value != _serverVersion)
                {
                    _serverVersion = value;
                    NotifyPropertyChanged(nameof(ServerVersion));
                }
            }
        }

        /// <summary>
        /// Gets the User Id. If user is not signed in,
        /// it returns an appropriate message.
        /// </summary>
        public string UserId
        {
            get
            {
                var currentUser = AppEnvironment.Instance.CurrentUser;
                if (currentUser != null)
                {
                    return currentUser.UserId;
                }

                return "Sign in to see your Id";
            }
        }

        /// <summary>
        /// Loads the state.
        /// </summary>
        public override async Task LoadState()
        {
            await base.LoadState();

            IsBusy = true;

            try
            {
                var config = await _photoService.GetConfig();
                ServerVersion = config.BuildVersion;
            }
            catch (Exception)
            {
                await _dialogService.ShowGenericServiceErrorNotification();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnCopyUserIdToClipboard()
        {
            var dataPackage = new DataPackage
            {
                RequestedOperation = DataPackageOperation.Copy
            };

            dataPackage.SetText(UserId);

            Clipboard.SetContent(dataPackage);
        }
    }
}