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

using PhotoSharingApp.AppService.Shared.Context;

namespace PhotoSharingApp.AppService.Tests.Context
{
    /// <summary>
    /// The Test environment definition.
    /// </summary>
    public class TestEnvironmentDefinition : EnvironmentDefinitionBase
    {
        private DocumentDbStorage _documentDbStorage;

        /// <summary>
        /// The Document DB database.
        /// </summary>
        public override DocumentDbStorage DocumentDbStorage
        {
            get
            {
                if (_documentDbStorage == null)
                {
                    _documentDbStorage = new DocumentDbStorage
                    {
                        DataBaseId = "snapgold-database-test",
                        CollectionId = "snapgold-document-data",
                        EndpointUrl = "https://snapgold-documentdb.documents.azure.com:443/",
                        AuthorizationKey = "M26QUV0Z64jgk8QTn/AOpI93BW3YdACdCpBC70462t6DTJBtrd9hHWiEda2rmw4TBduUtrrCe+29IhWlykNzQw=="
                    };
                }

                return _documentDbStorage;
            }
        }

        /// <summary>
        /// The Dev environment Notification Hub's default full shared access signature.
        /// </summary>
        public override string HubFullSharedAccessSignature
        {
            get { return "Endpoint=sb://snapgoldtesthub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=s3w/ZpKE/n0lSHiz2nsY5+y69BwnU8NITJFyRnMdjPE="; }
        }

        /// <summary>
        /// The Dev environment Notification Hub's name.
        /// </summary>
        public override string HubName
        {
            get { return "snapgoldtesthub"; }
        }

        /// <summary>
        /// The Application Insights instrumentation key. This value is read from the app.config file.
        /// </summary>
        public override string InstrumentationKey
        {
            get { return "c9462f72-1480-4157-836a-843595a153de"; }
        }

        /// <summary>
        /// The Azure Storage access key that is used for storing
        /// uploaded photos.
        /// </summary>
        public override string StorageAccessKey
        {
            get { return "1Tp6Nrm/VqHUmcmWhZoNgPTpzhblGgBYyOZ+JVe/wIzCwZyo5wGDA7GWWuWxJaGIOv3BwWeq6+Inj2eowvtSoA=="; }
        }

        /// <summary>
        /// The Azure Storage account name that is used for storing
        /// uploaded photos.
        /// </summary>
        public override string StorageAccountName
        {
            get { return "canaryappstorage"; }
        }
    }
}