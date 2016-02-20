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

using PhotoSharingApp.Universal.Models;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The navigation arguments for the <see cref="StreamViewModel" />.
    /// </summary>
    public class StreamViewModelArgs
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="category">The category to show data for.</param>
        public StreamViewModelArgs(CategoryPreview category)
        {
            Category = category;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <remarks>
        /// Needed for serialization.
        /// </remarks>
        public StreamViewModelArgs()
        {
        }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public CategoryPreview Category { get; set; }
    }
}