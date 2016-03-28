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
using System.Threading.Tasks;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Editing;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Views;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The ViewModel for the crop view.
    /// </summary>
    public class CropViewModel : ViewModelBase
    {
        private readonly ICropControl _cropControl;
        private readonly IDialogService _dialogService;

        private WriteableBitmap _image;

        private readonly INavigationFacade _navigationFacade;

        private readonly BitmapRotation[] _rotationOrder =
        {
            BitmapRotation.None,
            BitmapRotation.Clockwise90Degrees,
            BitmapRotation.Clockwise180Degrees,
            BitmapRotation.Clockwise270Degrees
        };

        private StorageFile _sourceFile;

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="navigationFacade">The navigation facade.</param>
        /// <param name="cropControl">The crop control.</param>
        /// <param name="dialogService">The dialog service.</param>
        public CropViewModel(INavigationFacade navigationFacade, ICropControl cropControl,
            IDialogService dialogService)
        {
            _navigationFacade = navigationFacade;
            _cropControl = cropControl;
            _dialogService = dialogService;

            Rotation = _rotationOrder.FirstOrDefault();

            // Initialize commands
            NextCommand = new RelayCommand(OnNext);
            RotateClockwiseCommand = new RelayCommand(OnRotateClockwise);
        }

        /// <summary>
        /// Gets or sets the current category.
        /// </summary>
        public CategoryPreview Category { get; set; }

        /// <summary>
        /// Gets or sets the image that needs to be cropped.
        /// </summary>
        public WriteableBitmap Image
        {
            get { return _image; }
            set
            {
                if (value != _image)
                {
                    _image = value;
                    NotifyPropertyChanged(nameof(Image));
                }
            }
        }

        /// <summary>
        /// Gets the go to next screen command.
        /// </summary>
        public RelayCommand NextCommand { get; private set; }

        /// <summary>
        /// Gets or sets the clockwise rotation command.
        /// </summary>
        public RelayCommand RotateClockwiseCommand { get; private set; }

        /// <summary>
        /// Gets or sets the current rotation.
        /// </summary>
        private BitmapRotation Rotation { get; set; }

        /// <summary>
        /// Gets or sets the source file.
        /// </summary>
        public StorageFile SourceFile
        {
            get { return _sourceFile; }
            set
            {
                if (value != _sourceFile)
                {
                    _sourceFile = value;
                    NotifyPropertyChanged(nameof(SourceFile));
                }
            }
        }

        /// <summary>
        /// Loads the image
        /// </summary>
        public async Task LoadImage()
        {
            try
            {
                var fileStream = await SourceFile.OpenAsync(FileAccessMode.Read);

                if (Rotation != BitmapRotation.None)
                {
                    var rotatedStream = await BitmapTools.Rotate(fileStream, Rotation);
                    await _cropControl.LoadImage(rotatedStream);
                }
                else
                {
                    await _cropControl.LoadImage(fileStream);
                }
            }
            catch (Exception)
            {
                await _dialogService.ShowNotification("InvalidImage_Message", "InvalidImage_Title");
                _navigationFacade.GoBack();
            }
        }

        /// <summary>
        /// Loads the state.
        /// </summary>
        /// <param name="args">The viewmodel arguments.</param>
        public async Task LoadState(CropViewModelArgs args)
        {
            await base.LoadState();

            Category = args.Category;
            SourceFile = args.StorageFile;
        }

        private async void OnNext()
        {
            try
            {
                var croppedImage = await _cropControl.GetCroppedImage();
                _navigationFacade.NavigateToUploadView(croppedImage, Category);
            }
            catch (Exception)
            {
                await _dialogService.ShowNotification("CroppingError_Message", "GenericError_Title");
            }
        }

        private async void OnRotateClockwise()
        {
            var currentIndex = Array.IndexOf(_rotationOrder, Rotation);
            var index = (currentIndex + 1)%_rotationOrder.Length;
            Rotation = _rotationOrder[index];

            await LoadImage();
        }
    }
}