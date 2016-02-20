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
    /// The category json document.
    /// </summary>
    public class CategoryDocument : BaseDocument
    {
        /// <summary>
        /// The document type string value for the category json document.
        /// </summary>
        public static readonly string DocumentTypeIdentifier = "CATEGORY";

        /// <summary>
        /// The document type property.
        /// </summary>
        [JsonProperty(PropertyName = "DocumentType", Required = Required.Always)]
        public string DocumentType
        {
            get { return DocumentTypeIdentifier; }
        }

        /// <summary>
        /// The category name.
        /// </summary>
        [JsonProperty(PropertyName = "Name", Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Creates a <see cref="CategoryContract" /> from this document.
        /// </summary>
        /// <returns></returns>
        public CategoryContract ToContract()
        {
            return new CategoryContract
            {
                Id = Id,
                Name = Name
            };
        }
    }
}