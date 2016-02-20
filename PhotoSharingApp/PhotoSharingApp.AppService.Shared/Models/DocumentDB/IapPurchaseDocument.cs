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

using Newtonsoft.Json;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Shared.Models.DocumentDB
{
    /// <summary>
    /// The IAP purchase json document
    /// </summary>
    public class IapPurchaseDocument : BaseDocument
    {
        /// <summary>
        /// The document type string value for the IAP purchase json document.
        /// </summary>
        public static readonly string DocumentTypeIdentifier = "IAP_PURCHASE";

        /// <summary>
        /// The document type property.
        /// </summary>
        [JsonProperty(PropertyName = "DocumentType", Required = Required.Always)]
        public string DocumentType
        {
            get { return DocumentTypeIdentifier; }
        }

        /// <summary>
        /// When iap expires if at all.
        /// </summary>
        [JsonProperty(PropertyName = "ExpirationDatetime")]
        public DateDocument ExpirationDatetime { get; set; }

        /// <summary>
        /// How much gold was purchased.
        /// </summary>
        [JsonProperty(PropertyName = "GoldIncrement")]
        public int GoldIncrement { get; set; }

        /// <summary>
        /// Product purchased.
        /// </summary>
        [JsonProperty(PropertyName = "ProductId")]
        public string ProductId { get; set; }

        /// <summary>
        /// When product was purchased.
        /// </summary>
        [JsonProperty(PropertyName = "PurchaseDatetime")]
        public DateDocument PurchaseDatetime { get; set; }

        /// <summary>
        /// UserId of purcahser.
        /// </summary>
        [JsonProperty(PropertyName = "UserId")]
        public string UserId { get; set; }

        /// <summary>
        /// Creates a <see cref="IapPurchaseDocument" /> from a <see cref="IapPurchaseContract" />.
        /// </summary>
        /// <returns>The <see cref="IapPurchaseDocument" />.</returns>
        public static IapPurchaseDocument CreateFromContract(IapPurchaseContract contract)
        {
            return new IapPurchaseDocument
            {
                Id = contract.IapPurchaseId,
                UserId = contract.UserId,
                ProductId = contract.ProductId,
                PurchaseDatetime = new DateDocument
                {
                    Date = contract.PurchaseDatetime
                },
                ExpirationDatetime = new DateDocument
                {
                    Date = contract.ExpirationDatetime
                },
                GoldIncrement = contract.GoldIncrement
            };
        }

        /// <summary>
        /// Creates a <see cref="UserContract" /> from this document
        /// </summary>
        /// <returns>The <see cref="UserContract" />.</returns>
        public IapPurchaseContract ToContract()
        {
            return new IapPurchaseContract
            {
                IapPurchaseId = Id,
                UserId = UserId,
                ProductId = ProductId,
                PurchaseDatetime = PurchaseDatetime.Date,
                ExpirationDatetime = ExpirationDatetime.Date,
                GoldIncrement = GoldIncrement
            };
        }
    }
}