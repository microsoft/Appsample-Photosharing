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
    /// Iap purchase details.
    /// </summary>
    public class IapPurchaseContract
    {
        /// <summary>
        /// When Iap expires if at all.
        /// </summary>
        public DateTime ExpirationDatetime { get; set; }

        /// <summary>
        /// How much gold was purchased.
        /// </summary>
        public int GoldIncrement { get; set; }

        /// <summary>
        /// Iap PurchaseId from the reciept.
        /// </summary>
        public string IapPurchaseId { get; set; }

        /// <summary>
        /// Product purchased.
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// When product was purchased.
        /// </summary>
        public DateTime PurchaseDatetime { get; set; }

        /// <summary>
        /// UserId of purchaser.
        /// </summary>
        public string UserId { get; set; }
    }
}