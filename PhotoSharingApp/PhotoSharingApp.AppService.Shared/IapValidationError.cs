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

namespace PhotoSharingApp.AppService.Shared
{
    /// <summary>
    /// The enum for IAP validation errors.
    /// </summary>
    public enum IapValidationError
    {
        /// <summary>
        /// Unknown error.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Operation attempted to insert a row with a key that already exists in the table.
        /// </summary>
        DuplicateReceipt = 1,

        /// <summary>
        /// Signature is invalid.
        /// </summary>
        BadSignature = 2,

        /// <summary>
        /// Can't get public cert.
        /// </summary>
        IapPublicCertificateNotDowloadable = 3,

        /// <summary>
        /// Missing receipt.
        /// </summary>
        NoReceipt = 4,
    }
}