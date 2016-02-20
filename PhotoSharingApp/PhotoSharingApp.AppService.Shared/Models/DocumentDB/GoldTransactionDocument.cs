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

namespace PhotoSharingApp.AppService.Shared.Models.DocumentDB
{
    /// <summary>
    /// The gold transaction json document.
    /// </summary>
    public class GoldTransactionDocument : BaseDocument
    {
        /// <summary>
        /// The document type string value for the gold transaction json document.
        /// </summary>
        public static readonly string DocumentTypeIdentifier = "GOLD_TRANSACTION";

        /// <summary>
        /// The timestamp of this gold transaction.
        /// </summary>
        [JsonProperty(PropertyName = "CreatedDateTime")]
        public DateDocument CreatedDateTime;

        /// <summary>
        /// The originator of this gold transaction.
        /// </summary>
        [JsonProperty(PropertyName = "FromUserId")]
        public string FromUserId;

        /// <summary>
        /// The amount of gold this transaction contains.
        /// </summary>
        [JsonProperty(PropertyName = "GoldCount")]
        public int GoldCount;

        /// <summary>
        /// The photo id this gold transaction is for if any.
        /// </summary>
        [JsonProperty(PropertyName = "PhotoId")]
        public string PhotoId;

        /// <summary>
        /// The recipient of this gold transaction.
        /// </summary>
        [JsonProperty(PropertyName = "ToUserId")]
        public string ToUserId;

        /// <summary>
        /// The type of of this gold transaction.
        /// </summary>
        [JsonProperty(PropertyName = "TransactionType")]
        public GoldTransactionType TransactionType;

        /// <summary>
        /// The document type property.
        /// </summary>
        [JsonProperty(PropertyName = "DocumentType", Required = Required.Always)]
        public string DocumentType
        {
            get { return DocumentTypeIdentifier; }
        }
    }
}