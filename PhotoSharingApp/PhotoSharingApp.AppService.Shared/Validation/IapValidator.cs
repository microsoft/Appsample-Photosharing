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
using System.IO;
using System.Xml;
using Microsoft.WindowsAzure.Storage;
using PhotoSharingApp.AppService.Shared.Context;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Shared.Validation
{
    /// <summary>
    /// Validate Iap purchase receipt.
    /// </summary>
    public class IapValidator : IIapValidator
    {
        private static readonly EnvironmentDefinitionBase EnvironmentDefinition = SystemContext.Current.Environment;
        private const string SignatureTagName = "Signature";

        /// <summary>
        /// Gets Iap cert name as a byte array.
        /// </summary>
        /// <returns>Byte array.</returns>
        private static byte[] GetIapPublicCertAsByteArray()
        {
            byte[] iapPublicCertificate;

            try
            {
                var containerClient = EnvironmentDefinition.StorageAccount.CreateCloudBlobClient();
                var container = containerClient.GetContainerReference(EnvironmentDefinition.IapValidationContainer);
                var blob = container.GetBlockBlobReference(EnvironmentDefinition.IapPublicCertName);

                using (var ms = new MemoryStream())
                {
                    blob.DownloadToStream(ms);
                    iapPublicCertificate = ms.ToArray();
                }
            }
            catch (StorageException ex)
            {
                throw new IapValidationException(IapValidationError.IapPublicCertificateNotDowloadable,
                    "Can't download Iap public cert for validation", ex);
            }

            return iapPublicCertificate;
        }

        /// <summary>
        /// Validates the xml signature.
        /// </summary>
        /// <param name="receipt">Receipt to validate.</param>
        /// <returns>A validated or null IapPurchaseContract object.</returns>
        public IapPurchaseContract ValidateXmlSignature(XmlDocument receipt)
        {
            if (receipt == null)
            {
                throw new IapValidationException(IapValidationError.NoReceipt, "ValidateXmlSignature: no receipt");
            }

            var iapPurchaseResult = new IapPurchaseContract();

            // For actual receipt validation we strongly suggest you follow the example
            // https://code.msdn.microsoft.com/windowsapps/In-app-purchase-receipt-c3e0bce4
            var isValid = true;

            if (isValid)
            {
                DateTime datetime;

                var productReceipt = receipt.GetElementsByTagName("ProductReceipt");

                if (DateTime.TryParse(productReceipt[0].Attributes["PurchaseDate"].Value, out datetime))
                {
                    iapPurchaseResult.PurchaseDatetime = datetime;
                }

                if (DateTime.TryParse(productReceipt[0].Attributes["ExpirationDate"].Value, out datetime))
                {
                    iapPurchaseResult.ExpirationDatetime = datetime;
                }

                iapPurchaseResult.IapPurchaseId = productReceipt[0].Attributes["Id"].Value;
                iapPurchaseResult.ProductId = productReceipt[0].Attributes["ProductId"].Value;
            }
            else
            {
                throw new IapValidationException(IapValidationError.BadSignature,
                    "ValidateXmlSignature: Bad receipt, signature not valid");
            }

            return iapPurchaseResult;
        }
    }
}