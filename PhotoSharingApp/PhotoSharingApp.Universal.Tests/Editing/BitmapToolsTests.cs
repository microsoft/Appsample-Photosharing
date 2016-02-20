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
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using PhotoSharingApp.Universal.Editing;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;

namespace PhotoSharingApp.Universal.Tests.Editing
{
    [TestClass]
    public class BitmapToolsTests
    {
        [TestMethod]
        public async Task ResizeJpgTest()
        {
            // Arrange
            const uint newSideLength = 100u;

            var folder = await Package.Current.InstalledLocation.GetFolderAsync(@"Assets\TestImages");
            var testPhoto = await folder.GetFileAsync("TestUploadPhoto.jpg");

            var stream = await testPhoto.OpenReadAsync();

            // Image should not be already in destination size
            var sourceDecoder = await BitmapDecoder.CreateAsync(stream);
            Assert.AreNotEqual(newSideLength, sourceDecoder.PixelWidth);
            Assert.AreNotEqual(newSideLength, sourceDecoder.PixelHeight);

            // Act
            var result = await BitmapTools.Resize(stream, newSideLength, newSideLength);

            // Verify
            Assert.IsNotNull(result);

            var decoder = await BitmapDecoder.CreateAsync(result);

            Assert.AreEqual(newSideLength, decoder.PixelHeight);
            Assert.AreEqual(newSideLength, decoder.PixelWidth);
        }

        [TestMethod]
        public async Task ResizePngTest()
        {
            // Arrange
            const uint newSideLength = 100u;

            var folder = await Package.Current.InstalledLocation.GetFolderAsync(@"Assets\TestImages");
            var testPhoto = await folder.GetFileAsync("TestUploadPhoto.png");

            var stream = await testPhoto.OpenReadAsync();

            // Image should not be already in destination size
            var sourceDecoder = await BitmapDecoder.CreateAsync(stream);
            Assert.AreNotEqual(newSideLength, sourceDecoder.PixelWidth);
            Assert.AreNotEqual(newSideLength, sourceDecoder.PixelHeight);

            // Act
            var result = await BitmapTools.Resize(stream, newSideLength, newSideLength);

            // Verify
            Assert.IsNotNull(result);

            var decoder = await BitmapDecoder.CreateAsync(result);

            Assert.AreEqual(newSideLength, decoder.PixelHeight);
            Assert.AreEqual(newSideLength, decoder.PixelWidth);
        }

        [TestMethod]
        public async Task RotateJpgTest()
        {
            // Arrange
            var folder = await Package.Current.InstalledLocation.GetFolderAsync(@"Assets\TestImages");
            var testPhoto = await folder.GetFileAsync("TestUploadPhoto.jpg");
            var photoStream = await testPhoto.OpenReadAsync();

            var sourceImageDecoder = await BitmapDecoder.CreateAsync(photoStream);

            // Act
            var rotatedStream = await BitmapTools.Rotate(photoStream, BitmapRotation.Clockwise90Degrees);

            // Verify
            Assert.IsNotNull(rotatedStream);

            var rotatedImageDecoder = await BitmapDecoder.CreateAsync(rotatedStream);

            Assert.AreEqual(sourceImageDecoder.PixelWidth, rotatedImageDecoder.PixelHeight);
            Assert.AreEqual(sourceImageDecoder.PixelHeight, rotatedImageDecoder.PixelWidth);
        }

        [TestMethod]
        public async Task RotatePngTest()
        {
            // Arrange
            var folder = await Package.Current.InstalledLocation.GetFolderAsync(@"Assets\TestImages");
            var testPhoto = await folder.GetFileAsync("TestUploadPhoto.png");
            var photoStream = await testPhoto.OpenReadAsync();

            var sourceImageDecoder = await BitmapDecoder.CreateAsync(photoStream);

            // Act
            var rotatedStream = await BitmapTools.Rotate(photoStream, BitmapRotation.Clockwise90Degrees);

            // Verify
            Assert.IsNotNull(rotatedStream);

            var rotatedImageDecoder = await BitmapDecoder.CreateAsync(rotatedStream);

            Assert.AreEqual(sourceImageDecoder.PixelWidth, rotatedImageDecoder.PixelHeight);
            Assert.AreEqual(sourceImageDecoder.PixelHeight, rotatedImageDecoder.PixelWidth);
        }
    }
}