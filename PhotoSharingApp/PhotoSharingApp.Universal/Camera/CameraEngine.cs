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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PhotoSharingApp.Universal.ComponentModel;
using PhotoSharingApp.Universal.Editing;
using PhotoSharingApp.Universal.Extensions;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Views;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace PhotoSharingApp.Universal.Camera
{
    public class CameraEngine : ObservableObjectBase, ICameraEngine
    {
        /// <summary>
        /// Rotation metadata to apply to the preview stream and recorded videos (MF_MT_VIDEO_ROTATION)
        /// Reference: http://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh868174.aspx
        /// </summary>
        private static readonly Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");

        /// <summary>
        /// The available preview resolutions.
        /// </summary>
        private IReadOnlyList<IMediaEncodingProperties> _availablePreviewResolutions;

        /// <summary>
        /// The camera view
        /// </summary>
        private ICameraView _cameraView;

        /// <summary>
        /// The device
        /// </summary>
        private DeviceInformation _device;

        /// <summary>
        /// The available devices
        /// </summary>
        private DeviceInformationCollection _devices;

        /// <summary>
        /// Holds the status whether the camera has been initialized.
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// The media capture element for accessing the camera.
        /// </summary>
        private MediaCapture _mediaCapture;

        /// <summary>
        /// The selected previous resolution
        /// </summary>
        private VideoEncodingProperties _selectedPreviewResolution;

        /// <summary>
        /// Gets a value indicating whether multiple cameras are available.
        /// </summary>
        /// <value>
        /// <c>true</c> if multiple cameras are available; otherwise, <c>false</c>.
        /// </value>
        public bool AreMultipleCamerasAvailable
        {
            get
            {
                return _devices?.Count >= 2;
            }
        }

        /// <summary>
        /// Gets or sets the desired panel.
        /// </summary>
        /// <value>
        /// The desired panel.
        /// </value>
        public Panel DesiredPanel { get; set; } = Panel.Back;

        /// <summary>
        /// Gets a value indicating whether a flash is supported by the camera.
        /// </summary>
        /// <value>
        /// <c>true</c> if flash is supported; otherwise, <c>false</c>.
        /// </value>
        public bool IsFlashSupported
        {
            get
            {
                if (IsInitialized &&
                    _mediaCapture.VideoDeviceController != null)
                {
                    return _mediaCapture.VideoDeviceController.FlashControl.Supported;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the camera has been initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if the camera has been initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get { return _isInitialized; }
            private set
            {
                _isInitialized = value;
                NotifyPropertyChanged(nameof(IsFlashSupported));
                NotifyPropertyChanged(nameof(AreMultipleCamerasAvailable));
            }
        }

        /// <summary>
        /// Gets a value indicating whether the preview is active.
        /// </summary>
        /// <value>
        /// <c>true</c> if preview is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsPreviewing { get; private set; }

        /// <summary>
        /// Cleans the instance up.
        /// </summary>
        public async Task CleanUp()
        {
            try
            {
                if (IsPreviewing)
                {
                    await StopPreviewAsync();
                }

                _mediaCapture?.Dispose();
            }
            catch (ObjectDisposedException)
            {
                // Camera has already been disposed.
            }
        }

        /// <summary>
        /// Gets the device for the desired panel.
        /// </summary>
        /// <returns>The device. If none found, null is returned.</returns>
        private async Task<DeviceInformation> GetDevice()
        {
            _devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

#if DEBUG
            foreach (var device in _devices)
            {
                Debug.WriteLine("{0} {1}", device.Name, device.EnclosureLocation);
            }
#endif

            if (_devices.Count > 0)
            {
                var panelDevices = _devices.Where(d => d.EnclosureLocation != null
                                                       && d.EnclosureLocation.Panel == DesiredPanel);

                var deviceInformations = panelDevices as IList<DeviceInformation> ?? panelDevices.ToList();

                if (deviceInformations.Any())
                    return deviceInformations.First();

                // Return any available device if desired panel is not available.
                // This is especially useful with tablets/PCs.
                return _devices.FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// Gets the number of available devices that
        /// can be used for taking photos.
        /// </summary>
        /// <returns>The number of available devices.</returns>
        public async Task<int> GetNumberOfAvailableDevices()
        {
            var availableDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            return availableDevices.Count;
        }

        /// <summary>
        /// Initializes the camera engine.
        /// </summary>
        /// <param name="cameraView">The camera view.</param>
        public void Init(ICameraView cameraView)
        {
            _cameraView = cameraView;
        }

        /// <summary>
        /// Initializes the camera.
        /// </summary>
        public async Task InitCamera(DeviceInformation deviceInformation = null)
        {
            _mediaCapture = new MediaCapture();

            _device = deviceInformation ?? await GetDevice();

            if (_device != null)
            {
                var captureInitSettings = new MediaCaptureInitializationSettings();
                captureInitSettings.AudioDeviceId = string.Empty;
                captureInitSettings.VideoDeviceId = string.Empty;

                captureInitSettings.PhotoCaptureSource = PhotoCaptureSource.VideoPreview;

                // Set the video device here
                captureInitSettings.VideoDeviceId = _device.Id;

                // Init
                await _mediaCapture.InitializeAsync(captureInitSettings);

                await InitResolutions();

                IsInitialized = true;
            }
            else
            {
                throw new CameraNotFoundException();
            }
        }

        /// <summary>
        /// Initializes the resolutions.
        /// </summary>
        private async Task InitResolutions()
        {
            if (_mediaCapture != null)
            {
                _availablePreviewResolutions =
                    _mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo);

#if DEBUG
                foreach (var mediaEncodingPropertiese in _availablePreviewResolutions)
                {
                    var prop = mediaEncodingPropertiese as VideoEncodingProperties;
                    Debug.WriteLine("{0} {1}", prop.Width, prop.Height);
                }
#endif

                // Some devices produces black stripes around picture if highest 4:3 ratio is being used
                // For now switching to 16/9 resolution as default.
                _selectedPreviewResolution = _availablePreviewResolutions
                    .Where(r => ((VideoEncodingProperties)r).GetAspectRatio() == AspectRatio.Ratio16To9)
                    .OrderByDescending(r => ((VideoEncodingProperties)r).Width)
                    .FirstOrDefault() as VideoEncodingProperties;

                // Now set the resolution on the device
                if (_selectedPreviewResolution != null)
                {
                    await
                        _mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo,
                            _selectedPreviewResolution);
                }
            }
        }

        /// <summary>
        /// Refreshes the preview rotation.
        /// </summary>
        private async Task RefreshPreviewRotation()
        {
            if (AppEnvironment.Instance.IsMobileDeviceFamily)
            {
                // Calculate which way and how far to rotate the preview
                var rotationDegrees = DisplayOrientations.Portrait.ToDegrees();

                // On Phone, we need to mirror the preview picture for the
                // front camera
                if (_device?.EnclosureLocation?.Panel == Panel.Front)
                {
                    rotationDegrees = (360 - rotationDegrees) % 360;
                }

                var props = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
                props.Properties.Add(RotationKey, rotationDegrees);

                await _mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);
            }
        }

        /// <summary>
        /// Starts the video preview.
        /// </summary>
        public async Task StartPreviewAsync()
        {
            _cameraView.SetMediaCaptureSource(_mediaCapture);

            await _mediaCapture.StartPreviewAsync();

            await RefreshPreviewRotation();

            IsPreviewing = true;
        }

        /// <summary>
        /// Stops the preview.
        /// </summary>
        public async Task StopPreviewAsync()
        {
            if (_mediaCapture != null)
            {
                await _mediaCapture.StopPreviewAsync();
                IsPreviewing = false;
            }
        }

        /// <summary>
        /// Switches the camera.
        /// </summary>
        public async Task SwitchCamera()
        {
            // First unload camera if loaded
            await UnloadCamera();

            // Get all devices which are available
            var availableDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // We want to keep this code compatible for both mobile and desktop platform.
            // On desktop, we can have attached cameras which are not assigned to any panel.
            // So we just iterate through available cameras.
            if (_device != null
                && availableDevices.Count > 1)
            {
                var currentInList = availableDevices.SingleOrDefault(d => d.Id == _device.Id);
                if (currentInList != null)
                {
                    var nextDeviceIndex = availableDevices.ToList().IndexOf(currentInList) + 1;
                    _device = availableDevices[nextDeviceIndex % availableDevices.Count];
                }
            }

            // Now init.
            await InitCamera(_device);

            // And restart preview.
            await StartPreviewAsync();
        }

        /// <summary>
        /// Switches the flash mode.
        /// </summary>
        public Task SwitchFlashMode()
        {
            if (IsInitialized)
            {
                var flashControl = _mediaCapture.VideoDeviceController.FlashControl;
                if (flashControl.Supported)
                {
                    flashControl.Enabled = !flashControl.Enabled;
                }
            }
            return Task.FromResult(default(object));
        }

        /// <summary>
        /// Takes the photo.
        /// </summary>
        public async Task<WriteableBitmap> TakePhoto()
        {
            var stream = new InMemoryRandomAccessStream();

            if (_mediaCapture.VideoDeviceController.FocusControl.Supported)
            {
                await _mediaCapture.VideoDeviceController.FocusControl.FocusAsync();
            }

            // Capture the photo
            await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

            // Get width and height for square aspect ratio
            var minDim = Math.Min(_selectedPreviewResolution.Width, _selectedPreviewResolution.Height);

            var xOffset = (_selectedPreviewResolution.Width - minDim) / 2.0;
            var yOffset = (_selectedPreviewResolution.Height - minDim) / 2.0;

            if (AppEnvironment.Instance.IsMobileDeviceFamily)
            {
                // Adjust orientation depending on camera panel
                var rotation = (_device.EnclosureLocation?.Panel == Panel.Front)
                    ? BitmapRotation.Clockwise270Degrees
                    : BitmapRotation.Clockwise90Degrees;

                // Rotate the image
                stream = await BitmapTools.Rotate(stream, rotation);

                // We have a different orientation on mobile, so also swap offsets
                var tempOffset = xOffset;
                yOffset = xOffset;
                xOffset = tempOffset;
            }

            // Now crop the image
            var writeableBitmap =
                await
                    BitmapTools.GetCroppedBitmapAsync(stream, new Point(xOffset, yOffset), new Size(minDim, minDim),
                        1);

            return writeableBitmap;
        }

        /// <summary>
        /// Unloads the camera.
        /// </summary>
        public async Task UnloadCamera()
        {
            await CleanUp();
        }
    }
}