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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace PhotoSharingApp.Universal.Editing
{
    /// <summary>
    /// Offers some tools for editing bitmaps.
    /// </summary>
    public class BitmapTools
    {
        /// <summary>
        /// Gets the cropped bitmap asynchronously.
        /// </summary>
        /// <param name="originalImage">The original image.</param>
        /// <param name="startPoint">The start point.</param>
        /// <param name="cropSize">Size of the corp.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>The cropped image.</returns>
        public static async Task<WriteableBitmap> GetCroppedBitmapAsync(IRandomAccessStream originalImage,
            Point startPoint, Size cropSize, double scale)
        {
            if (double.IsNaN(scale) || double.IsInfinity(scale))
            {
                scale = 1;
            }

            // Convert start point and size to integer.
            var startPointX = (uint)Math.Floor(startPoint.X * scale);
            var startPointY = (uint)Math.Floor(startPoint.Y * scale);
            var height = (uint)Math.Floor(cropSize.Height * scale);
            var width = (uint)Math.Floor(cropSize.Width * scale);

            // Create a decoder from the stream. With the decoder, we can get 
            // the properties of the image.
            var decoder = await BitmapDecoder.CreateAsync(originalImage);

            // The scaledSize of original image.
            var scaledWidth = (uint)Math.Floor(decoder.PixelWidth * scale);
            var scaledHeight = (uint)Math.Floor(decoder.PixelHeight * scale);

            // Refine the start point and the size. 
            if (startPointX + width > scaledWidth)
            {
                startPointX = scaledWidth - width;
            }

            if (startPointY + height > scaledHeight)
            {
                startPointY = scaledHeight - height;
            }

            // Get the cropped pixels.
            var pixels = await GetPixelData(decoder, startPointX, startPointY, width, height,
                scaledWidth, scaledHeight);

            // Stream the bytes into a WriteableBitmap
            var cropBmp = new WriteableBitmap((int)width, (int)height);
            var pixStream = cropBmp.PixelBuffer.AsStream();
            pixStream.Write(pixels, 0, (int)(width * height * 4));

            return cropBmp;
        }

        /// <summary>
        /// Gets the pixel data.
        /// </summary>
        /// <remarks>
        /// If you want to get the pixel data of a scaled image, set the scaledWidth and scaledHeight
        /// of the scaled image.
        /// </remarks>
        /// <param name="decoder">The bitmap decoder.</param>
        /// <param name="startPointX">The X coordinate of the start point.</param>
        /// <param name="startPointY">The Y coordinate of the start point.</param>
        /// <param name="width">The width of the source rect.</param>
        /// <param name="height">The height of the source rect.</param>
        /// <param name="scaledWidth">The desired width.</param>
        /// <param name="scaledHeight">The desired height.</param>
        /// <returns>The image data.</returns>
        private static async Task<byte[]> GetPixelData(BitmapDecoder decoder, uint startPointX, uint startPointY,
            uint width, uint height, uint scaledWidth, uint scaledHeight)
        {
            var transform = new BitmapTransform();
            var bounds = new BitmapBounds();
            bounds.X = startPointX;
            bounds.Y = startPointY;
            bounds.Height = height;
            bounds.Width = width;
            transform.Bounds = bounds;

            transform.ScaledWidth = scaledWidth;
            transform.ScaledHeight = scaledHeight;

            // Get the cropped pixels within the bounds of transform.
            var pix = await decoder.GetPixelDataAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.IgnoreExifOrientation,
                ColorManagementMode.ColorManageToSRgb);
            var pixels = pix.DetachPixelData();
            return pixels;
        }

        /// <summary>
        /// Resizes the specified stream.
        /// </summary>
        /// <param name="sourceStream">The source stream to resize.</param>
        /// <param name="newWidth">The width of the resized image.</param>
        /// <param name="newHeight">The height of the resized image.</param>
        /// <returns>The resized image stream.</returns>
        public static async Task<InMemoryRandomAccessStream> Resize(IRandomAccessStream sourceStream, uint newWidth,
            uint newHeight)
        {
            var destinationStream = new InMemoryRandomAccessStream();

            var decoder = await BitmapDecoder.CreateAsync(sourceStream);
            var transform = new BitmapTransform { ScaledWidth = newWidth, ScaledHeight = newHeight };

            var pixelData = await decoder.GetPixelDataAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.RespectExifOrientation,
                ColorManagementMode.DoNotColorManage);

            var encoder =
                await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, destinationStream);

            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, newWidth, newHeight, 96, 96,
                pixelData.DetachPixelData());
            await encoder.FlushAsync();

            return destinationStream;
        }

        /// <summary>
        /// Rotates the given stream.
        /// </summary>
        /// <param name="randomAccessStream">The random access stream.</param>
        /// <param name="rotation">The rotation.</param>
        /// <returns>The stream.</returns>
        public static async Task<InMemoryRandomAccessStream> Rotate(IRandomAccessStream randomAccessStream,
            BitmapRotation rotation)
        {
            var decoder = await BitmapDecoder.CreateAsync(randomAccessStream);

            var rotatedStream = new InMemoryRandomAccessStream();

            var encoder = await BitmapEncoder.CreateForTranscodingAsync(rotatedStream, decoder);

            encoder.BitmapTransform.Rotation = rotation;
            encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;

            await encoder.FlushAsync();

            return rotatedStream;
        }
    }
}