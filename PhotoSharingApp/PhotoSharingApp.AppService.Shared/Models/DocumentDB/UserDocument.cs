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
    /// The user json document.
    /// </summary>
    public class UserDocument : BaseDocument
    {
        /// <summary>
        /// The document type string value for the user json document.
        /// </summary>
        public static readonly string DocumentTypeIdentifier = "USER";

        /// <summary>
        /// Timestamp when the profile was created.
        /// </summary>
        [JsonProperty(PropertyName = "CreatedDateTime")]
        public DateDocument CreatedAt { get; set; }

        /// <summary>
        /// The document type property.
        /// </summary>
        [JsonProperty(PropertyName = "DocumentType", Required = Required.Always)]
        public string DocumentType
        {
            get { return DocumentTypeIdentifier; }
        }

        /// <summary>
        /// User gold total balance.
        /// </summary>
        [JsonProperty(PropertyName = "GoldBalance", Required = Required.Always)]
        public int GoldBalance { get; set; }

        /// <summary>
        /// The gold this user has given to others.
        /// </summary>
        [JsonProperty(PropertyName = "GoldGiven")]
        public int GoldGiven { get; set; }

        /// <summary>
        /// Timestamp when the profile was last modified.
        /// </summary>
        [JsonProperty(PropertyName = "ModifiedDateTime")]
        public DateDocument ModifiedAt { get; set; }

        /// <summary>
        /// Guid of profile picture, null at creation.
        /// </summary>
        [JsonProperty(PropertyName = "ProfilePhotoId")]
        public string ProfilePhotoId { get; set; }

        /// <summary>
        /// Url of profile picture, null at creation.
        /// </summary>
        [JsonProperty(PropertyName = "ProfilePhotoUrl")]
        public string ProfilePhotoUrl { get; set; }

        /// <summary>
        /// AzureMobileServices auth id, the only id we have at the time of creation.
        /// </summary>
        [JsonProperty(PropertyName = "RegistrationReference", Required = Required.Always)]
        public string RegistrationReference { get; set; }

        /// <summary>
        /// Creates a <see cref="UserDocument" /> from a <see cref="UserContract" />.
        /// </summary>
        /// <returns>The <see cref="UserDocument" />.</returns>
        public static UserDocument CreateFromContract(UserContract contract)
        {
            return new UserDocument
            {
                Id = contract.UserId,
                ProfilePhotoUrl = contract.ProfilePhotoUrl,
                ProfilePhotoId = contract.ProfilePhotoId,
                GoldBalance = contract.GoldBalance,
                RegistrationReference = contract.RegistrationReference,
                CreatedAt = new DateDocument
                {
                    Date = contract.UserCreated
                },
                ModifiedAt = new DateDocument
                {
                    Date = contract.UserModified
                }
            };
        }

        /// <summary>
        /// Creates a <see cref="UserContract" /> from this document
        /// </summary>
        /// <returns>The <see cref="UserContract" />.</returns>
        public UserContract ToContract()
        {
            return new UserContract
            {
                UserId = Id,
                ProfilePhotoUrl = ProfilePhotoUrl,
                ProfilePhotoId = ProfilePhotoId,
                GoldBalance = GoldBalance,
                RegistrationReference = RegistrationReference,
                UserCreated = CreatedAt.Date,
                UserModified = ModifiedAt.Date
            };
        }
    }
}