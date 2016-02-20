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

using System.ComponentModel.DataAnnotations;

namespace PhotoSharingApp.AppService.Shared.Models.DocumentDB
{
    /// <summary>
    /// Type of gold transaction.
    /// </summary>
    public enum GoldTransactionType
    {
        /// <summary>
        /// The transaction represents gold being awarded to a photo.
        /// </summary>
        [Display(Name = "Photo Gold Transaction.")]
        PhotoGoldTransaction = 101,

        /// <summary>
        /// The transaction represents gold being awarded to a category.
        /// </summary>
        [Display(Name = "Category Up Vote Transaction.")]
        CategoryUpVoteTransaction = 102,

        /// <summary>
        /// The transaction represents gold being awarded for registering.
        /// </summary>
        [Display(Name = "Welcome Gold Transaction.")]
        WelcomeGoldTransaction = 103,

        /// <summary>
        /// The transaction represents gold being awarded for a user
        /// setting their profile picture for the first time.
        /// </summary>
        [Display(Name = "First Profile Pic Update Transaction.")]
        FirstProfilePicUpdateTransaction = 104,

        /// <summary>
        /// The transaction represents gold being awarded for an iap purchase.
        /// </summary>
        [Display(Name = "Iap Gold Transaction.")]
        IapGoldTransaction = 105
    }
}