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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.WindowsAzure.Storage.Blob;
using PhotoSharingApp.AppService.Shared.Context;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Controllers
{
    /// <summary>
    /// Controller to manage SAS Urls.
    /// </summary>
    [MobileAppController]
    public class SasUrlController : ApiController
    {
        private readonly EnvironmentDefinitionBase _environment = SystemContext.Current.Environment;
        private readonly TelemetryClient _telemetryClient;

        public SasUrlController(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Gets a set of SAS Urls.
        /// </summary>
        /// <verb>GET</verb>
        /// <url>http://{host}/api/sasurl</url>
        /// <returns>A collection of SasUrl objects.</returns>
        [Route("api/sasurl")]
        public async Task<List<SasContract>> GetAsync()
        {
            _telemetryClient.TrackEvent("SAS Url Requested");

            var sasList = new List<SasContract>();

            var containerClient = _environment.StorageAccount.CreateCloudBlobClient();

            var fileName = $"{Guid.NewGuid()}.jpg";

            foreach (var photoType in Enum.GetNames(typeof(PhotoTypeContract)))
            {
                var currentPhotoTypeContract = (PhotoTypeContract)Enum.Parse(typeof(PhotoTypeContract), photoType, true);

                string sasContainerName;

                // Set the container to the name of the phototype, container names must be lower case
                _environment.ImageContainerNameMappings.TryGetValue(
                    currentPhotoTypeContract, out sasContainerName);

                // Create the blob client object.
                var container = containerClient.GetContainerReference(sasContainerName);
                container.CreateIfNotExists();
                container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

                var sasContract = new SasContract
                {
                    BlobFileName = fileName
                };

                // Storing jpeg images and generating the file name.
                var blob = container.GetBlockBlobReference(sasContract.BlobFileName);
                blob.Properties.ContentType = "image/jpeg";
                var accessSignatureForBlob = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
                {
                    Permissions = SharedAccessBlobPermissions.Read
                                  | SharedAccessBlobPermissions.Write,
                    SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(5)
                });

                sasContract.SasToken = accessSignatureForBlob;
                sasContract.SasPhotoType = currentPhotoTypeContract;
                sasContract.FullBlobUri = blob.Uri;
                sasList.Add(sasContract);
            }

            return await Task.FromResult(sasList);
        }
    }
}