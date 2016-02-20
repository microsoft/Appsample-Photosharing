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
using PhotoSharingApp.Universal.Views;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml.Media.Imaging;

namespace PhotoSharingApp.Universal.Camera
{
    /// <summary>
    /// This class is an abstraction for camera access.
    /// </summary>
    public interface ICameraEngine
    {
        /// <summary>
        /// Gets a value indicating whether multiple cameras are available.
        /// </summary>
        /// <value>
        /// <c>true</c> if multiple cameras are available; otherwise, <c>false</c>.
        /// </value>
        bool AreMultipleCamerasAvailable { get; }

        /// <summary>
        /// Gets a value indicating whether a flash is supported by the camera..
        /// </summary>
        /// <value>
        /// <c>true</c> if flash is supported; otherwise, <c>false</c>.
        /// </value>
        bool IsFlashSupported { get; }

        /// <summary>
        /// Gets the number of available devices that
        /// can be used for taking photos.
        /// </summary>
        /// <returns>The number of available devices.</returns>
        Task<int> GetNumberOfAvailableDevices();

        /// <summary>
        /// Initializes the camera engine.
        /// </summary>
        /// <param name="cameraView">The camera view.</param>
        void Init(ICameraView cameraView);

        /// <summary>
        /// Initializes the camera.
        /// </summary>
        Task InitCamera(DeviceInformation deviceInformation = null);

        /// <summary>
        /// Starts the preview.
        /// </summary>
        Task StartPreviewAsync();

        /// <summary>
        /// Switches between available cameras.
        /// </summary>
        Task SwitchCamera();

        /// <summary>
        /// Switches the flash mode.
        /// </summary>
        Task SwitchFlashMode();

        /// <summary>
        /// Takes the photo.
        /// </summary>
        /// <returns>The photo.</returns>
        Task<WriteableBitmap> TakePhoto();

        /// <summary>
        /// Unloads the camera.
        /// </summary>
        Task UnloadCamera();
    }
}