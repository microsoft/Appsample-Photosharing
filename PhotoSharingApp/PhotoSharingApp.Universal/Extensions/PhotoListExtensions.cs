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

using System.Collections.Generic;
using System.Linq;
using PhotoSharingApp.Universal.Models;

namespace PhotoSharingApp.Universal.Extensions
{
    /// <summary>
    /// Provides extensions for photo lists.
    /// </summary>
    public static class PhotoListExtensions
    {
        /// <summary>
        /// Searches for a photo in the given photo list that matches the given
        /// photo thumbnail.
        /// </summary>
        /// <param name="photos">The photo list to search in.</param>
        /// <param name="photoThumbnail">The thumbnail to search for.</param>
        /// <returns>The photo that matches the given thumbnail.</returns>
        public static Photo FindPhotoForThumbnail(this IList<Photo> photos, PhotoThumbnail photoThumbnail)
        {
            return photos.FirstOrDefault(p => p.ThumbnailUrl.Equals(photoThumbnail.ImageUrl));
        }
    }
}