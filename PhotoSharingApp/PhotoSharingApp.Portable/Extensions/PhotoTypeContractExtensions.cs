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
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.Portable.Extensions
{
    /// <summary>
    /// Provides extensions for <see cref="PhotoTypeContract" />.
    /// </summary>
    public static class PhotoTypeContractExtensions
    {
        /// <summary>
        /// Returns the required image side length for the given photo type.
        /// </summary>
        /// <param name="photoTypeContract">The photo type contract.</param>
        /// <returns>The image side length in pixels.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Invalid photo type.</exception>
        public static uint ToSideLength(this PhotoTypeContract photoTypeContract)
        {
            switch (photoTypeContract)
            {
                case PhotoTypeContract.Thumbnail:
                    return 150;
                case PhotoTypeContract.Standard:
                    return 650;
                case PhotoTypeContract.HighRes:
                    return 1000;
                default:
                    throw new ArgumentException(nameof(photoTypeContract));
            }
        }
    }
}