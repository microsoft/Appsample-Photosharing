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
using PhotoSharingApp.Universal.ComponentModel;

namespace PhotoSharingApp.Universal.Models
{
    /// <summary>
    /// Represents user data.
    /// </summary>
    public class User : ObservableObjectBase
    {
        private int _goldBalance;

        /// <summary>
        /// Gets or sets the gold balance.
        /// </summary>
        public int GoldBalance
        {
            get { return _goldBalance; }
            set
            {
                if (value != _goldBalance)
                {
                    _goldBalance = value;

                    if (_goldBalance < 0)
                    {
                        _goldBalance = 0;
                        throw new ArgumentException("User balance must not fall below zero.");
                    }

                    NotifyPropertyChanged(nameof(GoldBalance));
                }
            }
        }

        /// <summary>
        /// Gets or sets the profile picture URL.
        /// </summary>
        public string ProfilePictureId { get; set; }

        /// <summary>
        /// Gets or sets the profile picture URL.
        /// </summary>
        public string ProfilePictureUrl { get; set; }

        /// <summary>
        /// Get or set the RegistrationReference
        /// </summary>
        public string RegistrationReference { get; set; }

        /// <summary>
        /// Datetime user record inserted
        /// </summary>
        public DateTime UserCreated { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Datetime user record last updated
        /// </summary>
        public DateTime UserModified { get; set; }
    }
}