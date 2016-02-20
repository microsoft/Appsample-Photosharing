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

using PhotoSharingApp.Portable.DataContracts;
using PhotoSharingApp.Portable.Extensions;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Storage;
using PhotoSharingApp.Universal.Views;
using Windows.ApplicationModel.Resources;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The ViewModel for the upload photo control.
    /// </summary>
    public class UploadPhotoControlViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationFacade _navigationFacade;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="navigationFacade">The navigation facade.</param>
        /// <param name="dialogService">The dialog service.</param>
        public UploadPhotoControlViewModel(INavigationFacade navigationFacade, IDialogService dialogService)
        {
            _navigationFacade = navigationFacade;
            _dialogService = dialogService;

            // Initialize commands
            TakePhotoCommand = new RelayCommand(OnTakePhoto);
            BrowsePicturesLibraryCommand = new RelayCommand(OnBrowsePictures);
        }

        /// <summary>
        /// Gets the command for browsing the pictures library.
        /// </summary>
        public RelayCommand BrowsePicturesLibraryCommand { get; }

        /// <summary>
        /// Gets the take photo command.
        /// </summary>
        public RelayCommand TakePhotoCommand { get; }

        private async void OnBrowsePictures()
        {
            try
            {
                var file = await FilePickerHelper.PickSingleFile();
                if (file != null)
                {
                    _navigationFacade.NavigateToCropView(file);
                }
            }
            catch (InvalidImageDimensionsException)
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                var message = string.Format(resourceLoader.GetString("InvalidImageSize_Message"),
                    PhotoTypeContract.Standard.ToSideLength());
                var title = resourceLoader.GetString("InvalidImageSize_Title");

                await _dialogService.ShowNotification(message, title, false);
            }
        }

        private void OnTakePhoto()
        {
            _navigationFacade.NavigateToCameraView();
        }
    }
}