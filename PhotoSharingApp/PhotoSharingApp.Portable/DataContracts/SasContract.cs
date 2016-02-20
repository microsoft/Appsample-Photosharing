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

namespace PhotoSharingApp.Portable.DataContracts
{
    /// <summary>
    /// The data contract for retrieving the Azure SAS url
    /// for a specific <see cref="PhotoTypeContract" />.
    /// </summary>
    public class SasContract
    {
        /// <summary>
        /// The filename of the blob.
        /// </summary>
        public string BlobFileName { get; set; }

        /// <summary>
        /// The full blob uri.
        /// </summary>
        public Uri FullBlobUri { get; set; }

        /// <summary>
        /// The photo type.
        /// </summary>
        public PhotoTypeContract SasPhotoType { get; set; }

        /// <summary>
        /// The SAS token.
        /// </summary>
        public string SasToken { get; set; }
    }
}