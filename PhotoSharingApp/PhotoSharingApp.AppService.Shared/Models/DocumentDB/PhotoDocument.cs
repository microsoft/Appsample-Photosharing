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

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Shared.Models.DocumentDB
{
    /// <summary>
    /// The photo json document.
    /// </summary>
    public class PhotoDocument : BaseDocument
    {
        /// <summary>
        /// The document type string value for the photo json document.
        /// </summary>
        public static readonly string DocumentTypeIdentifier = "PHOTO";

        /// <summary>
        /// The associated annotations of the photo.
        /// </summary>
        [JsonProperty(PropertyName = "Annotations")]
        public List<AnnotationDocument> Annotations { get; set; } = new List<AnnotationDocument>();

        /// <summary>
        /// The photo's category's id.
        /// </summary>
        [JsonProperty(PropertyName = "CategoryId", Required = Required.Always)]
        public string CategoryId { get; set; }

        /// <summary>
        /// The photo's category's name.
        /// </summary>
        [JsonProperty(PropertyName = "CategoryName", Required = Required.Always)]
        public string CategoryName { get; set; }

        /// <summary>
        /// The timestamp when the photo has been created
        /// in the system.
        /// </summary>
        [JsonProperty(PropertyName = "CreatedDateTime")]
        public DateDocument CreatedDateTime { get; set; }

        /// <summary>
        /// The photo description.
        /// </summary>
        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// The document type property.
        /// </summary>
        [JsonProperty(PropertyName = "DocumentType", Required = Required.Always)]
        public string DocumentType
        {
            get { return DocumentTypeIdentifier; }
        }

        /// <summary>
        /// The photo's number of gold pieces.
        /// </summary>
        [JsonProperty(PropertyName = "GoldCount", Required = Required.Always)]
        public int GoldCount { get; set; }

        /// <summary>
        /// The high-resolution image url.
        /// </summary>
        [JsonProperty(PropertyName = "HighResolutionUrl", Required = Required.Always)]
        public string HighResolutionUrl { get; set; }

        /// <summary>
        /// The timestamp when the photo has been modified
        /// in the system.
        /// </summary>
        [JsonProperty(PropertyName = "ModifiedDateTime")]
        public DateDocument ModifiedAt { get; set; }

        /// <summary>
        /// The operating system platform the photo has been
        /// uploaded from.
        /// </summary>
        [JsonProperty(PropertyName = "OSPlatform", Required = Required.Always)]
        public string OSPlatform { get; set; }

        /// <summary>
        /// The report for this photo.
        /// </summary>
        [JsonProperty(PropertyName = "Report")]
        public ReportDocument Report { get; set; }

        /// <summary>
        /// The standard-sized image url.
        /// </summary>
        [JsonProperty(PropertyName = "StandardUrl", Required = Required.Always)]
        public string StandardUrl { get; set; }

        /// <summary>
        /// The photo status.
        /// </summary>
        [JsonProperty(PropertyName = "Status", Required = Required.Always)]
        public PhotoStatus Status { get; set; }

        /// <summary>
        /// The thumbnail url.
        /// </summary>
        [JsonProperty(PropertyName = "ThumbnailUrl", Required = Required.Always)]
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// The photo owner.
        /// </summary>
        [JsonProperty(PropertyName = "UserId", Required = Required.Always)]
        public string UserId { get; set; }

        /// <summary>
        /// Creates a <see cref="PhotoDocument" /> from a <see cref="PhotoContract" />.
        /// </summary>
        /// <returns>The <see cref="PhotoDocument" />.</returns>
        public static PhotoDocument CreateFromContract(PhotoContract contract)
        {
            return new PhotoDocument
            {
                Id = contract.Id,
                CategoryId = contract.CategoryId,
                CategoryName = contract.CategoryName,
                UserId = contract.User.UserId,
                ThumbnailUrl = contract.ThumbnailUrl,
                StandardUrl = contract.StandardUrl,
                HighResolutionUrl = contract.HighResolutionUrl,
                Description = contract.Description,
                CreatedDateTime = new DateDocument
                {
                    Date = contract.CreatedAt
                },
                ModifiedAt = new DateDocument
                {
                    Date = contract.CreatedAt
                },
                Annotations =
                    contract.Annotations?.Select(AnnotationDocument.CreateFromContract).ToList() ??
                    new List<AnnotationDocument>(),
                GoldCount = contract.NumberOfGoldVotes,
                OSPlatform = contract.OSPlatform,
                Status = contract.Status
            };
        }

        /// <summary>
        /// Creates a <see cref="PhotoContract" /> from this document.
        /// </summary>
        /// <param name="userDocumentsForPhotosAndAnnotations">
        /// A collection of all the user documents needed for
        /// the PhotoContract.User and AnnotationContract.From fields.
        /// </param>
        /// <returns>The <see cref="PhotoContract" />.</returns>
        public PhotoContract ToContract(ICollection<UserDocument> userDocumentsForPhotosAndAnnotations)
        {
            // Find the proper UserContract for the PhotoContract.User field.
            var photoUser = userDocumentsForPhotosAndAnnotations.FirstOrDefault(u => u.Id == UserId)?.ToContract();

            // Find the proper data for the AnnotationContract conversion.
            var photoAnnotations = Annotations.Select(a => a.ToContract(
                userDocumentsForPhotosAndAnnotations.FirstOrDefault(u => u.Id == a.From)?.ToContract(), Id, UserId)
                ).ToList();

            return new PhotoContract
            {
                Id = Id,
                User = photoUser,
                CategoryId = CategoryId,
                CategoryName = CategoryName,
                ThumbnailUrl = ThumbnailUrl,
                StandardUrl = StandardUrl,
                HighResolutionUrl = HighResolutionUrl,
                Description = Description,
                CreatedAt = CreatedDateTime.Date,
                ModifiedAt = ModifiedAt.Date,
                Annotations = photoAnnotations,
                NumberOfGoldVotes = GoldCount,
                NumberOfAnnotations = Annotations.Count,
                OSPlatform = OSPlatform,
                Status = Status
            };
        }
    }
}