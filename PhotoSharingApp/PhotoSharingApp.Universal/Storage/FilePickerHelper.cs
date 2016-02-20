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
using Windows.Storage;
using Windows.Storage.Pickers;

namespace PhotoSharingApp.Universal.Storage
{
    /// <summary>
    /// Helper class which provides default settings for the file picker
    /// for opening photos.
    /// </summary>
    public class FilePickerHelper
    {
        /// <summary>
        /// Creates the file open picker with default parameters suitable
        /// for opening photos.
        /// </summary>
        public static FileOpenPicker CreateFileOpenPickerWithDefaultParams()
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            return openPicker;
        }

        /// <summary>
        /// Shows the file picker to let the user choose a single file.
        /// </summary>
        /// <exception cref="InvalidImageDimensionsException">Thrown when image is too small.</exception>
        /// <returns>The file that is chosen. Null, if user cancelled the dialog.</returns>
        public static async Task<StorageFile> PickSingleFile()
        {
            var openPicker = CreateFileOpenPickerWithDefaultParams();

            var file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                var imageProperties = await file.Properties.GetImagePropertiesAsync();

                var sideLength = PhotoTypeContract.Standard.ToSideLength();

                if (imageProperties.Width < sideLength || imageProperties.Height < sideLength)
                {
                    throw new InvalidImageDimensionsException();
                }
            }

            return file;
        }
    }
}