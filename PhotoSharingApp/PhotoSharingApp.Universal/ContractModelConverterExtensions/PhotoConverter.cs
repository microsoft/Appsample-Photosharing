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

using System.Collections.ObjectModel;
using System.Linq;
using PhotoSharingApp.Portable.DataContracts;
using PhotoSharingApp.Universal.Models;

namespace PhotoSharingApp.Universal.ContractModelConverterExtensions
{
    /// <summary>
    /// Helper class to convert between <see cref="Photo" />
    /// and <see cref="PhotoContract" /> classes.
    /// </summary>
    public static class PhotoConverter
    {
        /// <summary>
        /// Converts the given data model into a photo data contract.
        /// </summary>
        /// <param name="photo">The data model.</param>
        /// <returns>The photo data contract.</returns>
        public static PhotoContract ToDataContract(this Photo photo)
        {
            var photoContract = new PhotoContract
            {
                Id = photo.Id,
                CategoryId = photo.CategoryId,
                CategoryName = photo.CategoryName,
                ThumbnailUrl = photo.ThumbnailUrl,
                StandardUrl = photo.StandardUrl,
                HighResolutionUrl = photo.HighResolutionUrl,
                User = photo.User.ToDataContract(),
                Description = photo.Caption,
                CreatedAt = photo.CreatedAt,
                NumberOfAnnotations = photo.NumberOfAnnotations,
                NumberOfGoldVotes = photo.GoldCount,
                Status = photo.Status
            };

            return photoContract;
        }

        /// <summary>
        /// Converts the given contract into a data model.
        /// </summary>
        /// <param name="photoContract">The data contract.</param>
        /// <returns>The data model.</returns>
        public static Photo ToDataModel(this PhotoContract photoContract)
        {
            var photo = new Photo
            {
                Id = photoContract.Id,
                CategoryId = photoContract.CategoryId,
                ThumbnailUrl = photoContract.ThumbnailUrl,
                StandardUrl = photoContract.StandardUrl,
                HighResolutionUrl = photoContract.HighResolutionUrl,
                User = photoContract.User.ToDataModel(),
                Caption = photoContract.Description,
                CreatedAt = photoContract.CreatedAt,
                NumberOfAnnotations = photoContract.NumberOfAnnotations,
                GoldCount = photoContract.NumberOfGoldVotes,
                CategoryName = photoContract.CategoryName,
                Status = photoContract.Status
            };

            if (photoContract.Annotations != null)
            {
                photo.Annotations = new ObservableCollection<Annotation>(
                    photoContract.Annotations.Select(a => a.ToDataModel()));
            }

            return photo;
        }
    }
}