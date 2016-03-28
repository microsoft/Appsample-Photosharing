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
using System.Threading.Tasks;
using PhotoSharingApp.Portable.DataContracts;
using PhotoSharingApp.Portable.Extensions;
using PhotoSharingApp.Universal.Camera;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Storage;
using PhotoSharingApp.Universal.Views;
using Windows.ApplicationModel.Resources;
using Windows.Media.Capture;
using Windows.UI.Xaml.Controls;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The ViewModel for the camera view.
    /// </summary>
    public class CameraViewModel : ViewModelBase, ICameraView
    {
        /// <summary>
        /// The camera engine
        /// </summary>
        private readonly ICameraEngine _cameraEngine;

        private bool _canTakePhoto;

        /// <summary>
        /// The capture preview element
        /// </summary>
        private CaptureElement _capturePreviewElement;

        /// <summary>
        /// The category
        /// </summary>
        private CategoryPreview _category;

        private readonly IDialogService _dialogService;

        private bool _isBusy;

        /// <summary>
        /// The navigation facade
        /// </summary>
        private readonly INavigationFacade _navigationFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraViewModel" /> class.
        /// </summary>
        /// <param name="cameraEngine">The camera engine.</param>
        /// <param name="navigationFacade">The navigation facade.</param>
        /// <param name="dialogService">The dialog service.</param>
        public CameraViewModel(ICameraEngine cameraEngine, INavigationFacade navigationFacade,
            IDialogService dialogService)
        {
            _navigationFacade = navigationFacade;
            _dialogService = dialogService;
            _cameraEngine = cameraEngine;

            cameraEngine.Init(this);

            // Initialize commands
            TakePhotoCommand = new RelayCommand(OnTakePhoto, () => !IsBusy);
            SwitchCameraCommand = new RelayCommand(OnSwitchCamera, () => !IsBusy);
            SwitchFlashCommand = new RelayCommand(OnSwitchFlash, () => !IsBusy);
            OpenPictureCommand = new RelayCommand(OnOpenPicture, () => !IsBusy);

            // By default, we expect the user is able to
            // have a device attached for taking photos.
            // If we don't do this, mobile users will always see
            // the UI switching from state "No device attached" to
            // the viewfinder whenever navigating to this view.
            CanTakePhoto = true;
        }

        /// <summary>
        /// Gets the camera engine.
        /// </summary>
        public ICameraEngine CameraEngine
        {
            get { return _cameraEngine; }
        }

        /// <summary>
        /// Gets or sets a state which determines if the
        /// </summary>
        public bool CanTakePhoto
        {
            get { return _canTakePhoto; }
            set
            {
                if (value != _canTakePhoto)
                {
                    _canTakePhoto = value;
                    NotifyPropertyChanged(nameof(CanTakePhoto));
                }
            }
        }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public CategoryPreview Category
        {
            get { return _category; }
            set
            {
                if (value != _category)
                {
                    _category = value;
                    NotifyPropertyChanged(nameof(Category));
                }
            }
        }

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

                    TakePhotoCommand.RaiseCanExecuteChanged();
                    SwitchCameraCommand.RaiseCanExecuteChanged();
                    SwitchFlashCommand.RaiseCanExecuteChanged();
                    OpenPictureCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets the open picture from library command.
        /// </summary>
        public RelayCommand OpenPictureCommand { get; }

        /// <summary>
        /// Gets the switch camera command.
        /// </summary>
        /// <value>
        /// The switch camera command.
        /// </value>
        public RelayCommand SwitchCameraCommand { get; }

        /// <summary>
        /// Gets the switch flash command.
        /// </summary>
        /// <value>
        /// The switch flash command.
        /// </value>
        public RelayCommand SwitchFlashCommand { get; }

        /// <summary>
        /// Gets the take photo command.
        /// </summary>
        /// <value>
        /// The take photo command.
        /// </value>
        public RelayCommand TakePhotoCommand { get; }

        /// <summary>
        /// Loads the camera.
        /// </summary>
        /// <param name="capturePreview">The capture preview.</param>
        public async Task LoadCamera(CaptureElement capturePreview)
        {
            _capturePreviewElement = capturePreview;

            try
            {
                await _cameraEngine.InitCamera();
                await _cameraEngine.StartPreviewAsync();
            }
            catch (CameraNotFoundException)
            {
                await _dialogService.ShowNotification("CameraNotFound_Message", "CameraNotFound_Title");
            }
            catch (Exception)
            {
                await _dialogService.ShowNotification("GenericError_Message", "GenericError_Title");
            }
        }

        /// <summary>
        /// Loads the state.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public async Task LoadState(CameraViewModelArgs args)
        {
            Category = args.Category;

            try
            {
                IsBusy = true;

                var numberOfAvailableDevices = await _cameraEngine.GetNumberOfAvailableDevices();
                CanTakePhoto = numberOfAvailableDevices >= 1;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnOpenPicture()
        {
            try
            {
                var file = await FilePickerHelper.PickSingleFile();

                if (file != null)
                {
                    _navigationFacade.NavigateToCropView(file, Category);
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

        private async void OnSwitchCamera()
        {
            try
            {
                await _cameraEngine.SwitchCamera();
            }
            catch (Exception)
            {
                await _dialogService.ShowNotification("GenericError_Message", "GenericError_Title");
            }
        }

        private async void OnSwitchFlash()
        {
            try
            {
                await _cameraEngine.SwitchFlashMode();
            }
            catch (Exception)
            {
                await _dialogService.ShowNotification("GenericError_Message", "GenericError_Title");
            }
        }

        private async void OnTakePhoto()
        {
            try
            {
                IsBusy = true;

                var photo = await _cameraEngine.TakePhoto();
                _navigationFacade.NavigateToUploadView(photo, Category);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Sets the media capture source to enable
        /// live preview in the UI.
        /// </summary>
        /// <param name="mediaCapture">The media capture object.</param>
        public void SetMediaCaptureSource(MediaCapture mediaCapture)
        {
            _capturePreviewElement.Source = mediaCapture;
        }

        /// <summary>
        /// Unloads the camera.
        /// </summary>
        public async Task UnloadCamera()
        {
            await _cameraEngine.UnloadCamera();
        }
    }
}