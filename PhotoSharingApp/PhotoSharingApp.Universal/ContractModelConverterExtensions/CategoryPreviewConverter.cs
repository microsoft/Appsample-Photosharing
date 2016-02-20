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

using System.Linq;
using PhotoSharingApp.Portable.DataContracts;
using PhotoSharingApp.Universal.Models;

namespace PhotoSharingApp.Universal.ContractModelConverterExtensions
{
    /// <summary>
    /// Helper class to convert between <see cref="CategoryPreview" />
    /// and <see cref="CategoryPreviewContract" /> classes.
    /// </summary>
    public static class CategoryPreviewConverter
    {
        /// <summary>
        /// Converts the given category preview data model to a contract.
        /// </summary>
        /// <param name="category">The category preview data model.</param>
        /// <returns>The category preview contract.</returns>
        public static CategoryPreviewContract ToDataContract(this CategoryPreview category)
        {
            return new CategoryPreviewContract
            {
                Id = category.Id,
                Name = category.Name,
                PhotoThumbnails = category.PhotoThumbnails.Select(t => t.ToDataContract()).ToList()
            };
        }

        /// <summary>
        /// Converts the given category preview contract to the category preview data model.
        /// </summary>
        /// <param name="categoryPreviewContract">The category preview data contract.</param>
        /// <returns>The category preview data model.</returns>
        public static CategoryPreview ToDataModel(this CategoryPreviewContract categoryPreviewContract)
        {
            return new CategoryPreview
            {
                Id = categoryPreviewContract.Id,
                Name = categoryPreviewContract.Name,
                PhotoThumbnails = categoryPreviewContract.PhotoThumbnails.Select(t => t.ToDataModel()).ToList()
            };
        }
    }
}