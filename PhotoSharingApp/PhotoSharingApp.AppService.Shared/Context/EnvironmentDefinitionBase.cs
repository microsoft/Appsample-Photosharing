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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Shared.Context
{
    /// <summary>
    /// The service environment base definition.
    /// </summary>
    public abstract class EnvironmentDefinitionBase
    {
        private const int FirstProfilePhotoUpdateGoldAwardCount = 5;
        private const string IapCertificateValidationContainer = "validation";
        private const string IapPublicCertificateName = "IapReceiptProduction.cer";
        private const int NewUserGoldAwardCount = 40;
        private CloudStorageAccount _storageAccount;

        /// <summary>
        /// The DocumentDB database.
        /// </summary>
        public abstract DocumentDbStorage DocumentDbStorage { get; }

        /// <summary>
        /// The gold amount that is awarded to the user the first time they update their profile picture.
        /// </summary>
        public virtual int FirstProfilePhotoUpdateGoldAward
        {
            get { return FirstProfilePhotoUpdateGoldAwardCount; }
        }

        /// <summary>
        /// The Notification Hub's default full shared access signature.
        /// </summary>
        public abstract string HubFullSharedAccessSignature { get; }

        /// <summary>
        /// The Notification Hub's name.
        /// </summary>
        public abstract string HubName { get; }

        /// <summary>
        /// File name of the public Iap cert. For testing you may want to try different certs or if a cert gets changed, it can be
        /// tested before deployed.
        /// </summary>
        public virtual string IapPublicCertName
        {
            get { return IapPublicCertificateName; }
        }

        /// <summary>
        /// Container where the Iap public cert is stored.
        /// </summary>
        public virtual string IapValidationContainer
        {
            get { return IapCertificateValidationContainer; }
        }

        /// <summary>
        /// Gets name mappings for image containers.
        /// </summary>
        public Dictionary<PhotoTypeContract, string> ImageContainerNameMappings { get; } = new Dictionary
            <PhotoTypeContract, string>
        {
            { PhotoTypeContract.HighRes, "highres" },
            { PhotoTypeContract.Standard, "standard" },
            { PhotoTypeContract.Thumbnail, "thumbnail" }
        };

        /// <summary>
        /// The Application Insights instrumentation key. This value is read from the app.config file.
        /// </summary>
        public abstract string InstrumentationKey { get; }

        /// <summary>
        /// Default new user gold balance.
        /// </summary>
        public virtual int NewUserGoldBalance
        {
            get { return NewUserGoldAwardCount; }
        }

        /// <summary>
        /// The Azure Storage access key that is used for storing
        /// uploaded photos.
        /// </summary>
        public abstract string StorageAccessKey { get; }

        /// <summary>
        /// The Azure Blob storage.
        /// </summary>
        public virtual CloudStorageAccount StorageAccount
        {
            get
            {
                if (_storageAccount == null)
                {
                    _storageAccount = new CloudStorageAccount(
                        new StorageCredentials(
                            StorageAccountName,
                            StorageAccessKey),
                        true);
                }

                return _storageAccount;
            }
        }

        /// <summary>
        /// The Azure Storage account name that is used for storing
        /// uploaded photos.
        /// </summary>
        public abstract string StorageAccountName { get; }
    }
}