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
using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Universal.Serialization;
using PhotoSharingApp.Universal.ViewModels;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// The page that allows taking a photo or opening
    /// a photo from the device's photos library.
    /// </summary>
    public sealed partial class CameraPage : Page
    {
        private CameraViewModel _viewModel;

        public CameraPage()
        {
            InitializeComponent();

            LayoutUpdated += CameraPage_LayoutUpdated;

            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.CameraPressed += HardwareButtons_CameraPressed;
            }
        }

        private void CameraPage_LayoutUpdated(object sender, object e)
        {
            var min = Math.Min(cameraPreviewContainer.ActualWidth, cameraPreviewContainer.ActualHeight);

            capturePreview.Width = min;
            capturePreview.Height = min;
        }

        private void HardwareButtons_CameraPressed(object sender, CameraEventArgs e)
        {
            if (_viewModel.TakePhotoCommand.CanExecute(null))
            {
                _viewModel.TakePhotoCommand.Execute(null);
            }
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
            await _viewModel.UnloadCamera();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            _viewModel = ServiceLocator.Current.GetInstance<CameraViewModel>();
            DataContext = _viewModel;

            if (e.Parameter != null)
            {
                var args = SerializationHelper.Deserialize<CameraViewModelArgs>(e.Parameter as string);
                await _viewModel.LoadState(args);
            }
            else
            {
                await _viewModel.LoadState(new CameraViewModelArgs());
            }

            if (_viewModel.CanTakePhoto)
            {
                await _viewModel.LoadCamera(capturePreview);
            }
        }
    }
}