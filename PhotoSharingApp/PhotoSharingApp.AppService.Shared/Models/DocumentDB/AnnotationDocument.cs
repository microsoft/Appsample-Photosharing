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
    /// The annotation json document.
    /// </summary>
    public class AnnotationDocument : BaseDocument
    {
        /// <summary>
        /// The timestamp when the annotation has been created
        /// in the system.
        /// </summary>
        [JsonProperty(PropertyName = "CreatedDateTime")]
        public DateDocument CreatedDateTime { get; set; }

        /// <summary>
        /// The originator of the annotation.
        /// </summary>
        [JsonProperty(PropertyName = "UserId", Required = Required.Always)]
        public string From { get; set; }

        /// <summary>
        /// The gold count of the annotation.
        /// </summary>
        [JsonProperty(PropertyName = "GoldCount", Required = Required.Always)]
        public int GoldCount { get; set; }

        /// <summary>
        /// The report for this photo.
        /// </summary>
        [JsonProperty(PropertyName = "Report")]
        public ReportDocument Report { get; set; }

        /// <summary>
        /// The comment.
        /// </summary>
        [JsonProperty(PropertyName = "Text")]
        public string Text { get; set; }

        /// <summary>
        /// Creates a <see cref="AnnotationDocument" /> from a <see cref="AnnotationContract" />.
        /// </summary>
        /// <returns>The <see cref="AnnotationDocument" />.</returns>
        public static AnnotationDocument CreateFromContract(AnnotationContract contract)
        {
            return new AnnotationDocument
            {
                CreatedDateTime = new DateDocument
                {
                    Date = contract.CreatedAt
                },
                Text = contract.Text,
                Id = contract.Id,
                GoldCount = contract.GoldCount,
                From = contract.From.UserId
            };
        }

        /// <summary>
        /// Creates a <see cref="AnnotationContract" /> from this document
        /// </summary>
        /// <param name="annotationOwner">The <see cref="UserContract" /> author of this annotation.</param>
        /// <param name="photoId">The id of the photo this annotation is for.</param>
        /// <param name="photoOwnerId">The id of the user that owns the photo this annotation is for.</param>
        /// <returns>The <see cref="AnnotationContract" />.</returns>
        public AnnotationContract ToContract(UserContract annotationOwner, string photoId, string photoOwnerId)
        {
            return new AnnotationContract
            {
                CreatedAt = CreatedDateTime.Date,
                Text = Text,
                Id = Id,
                GoldCount = GoldCount,
                From = annotationOwner,
                PhotoId = photoId,
                PhotoOwnerId = photoOwnerId
            };
        }
    }
}