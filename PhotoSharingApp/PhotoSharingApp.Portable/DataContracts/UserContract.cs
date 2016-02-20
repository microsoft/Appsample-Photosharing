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
    /// The user data contract.
    /// </summary>
    public class UserContract
    {
        /// <summary>
        /// User gold total balance.
        /// </summary>
        public int GoldBalance { get; set; }

        /// <summary>
        /// Guid of profile picture, null at creation.
        /// </summary>
        public string ProfilePhotoId { get; set; }

        /// <summary>
        /// Url of profile picture, null at creation.
        /// </summary>
        public string ProfilePhotoUrl { get; set; }

        /// <summary>
        /// AzureMobileServices auth id, the only id we have at the time of creation.
        /// </summary>
        public string RegistrationReference { get; set; }

        /// <summary>
        /// Timestamp when the profile was created.
        /// </summary>
        public DateTime UserCreated { get; set; }

        /// <summary>
        /// UserId in the form of a guid.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Timestamp when the profile was last modified.
        /// </summary>
        public DateTime UserModified { get; set; }
    }
}