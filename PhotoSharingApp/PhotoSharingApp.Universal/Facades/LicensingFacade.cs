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
using System.Threading.Tasks;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Store;
using Windows.ApplicationModel.Store;

namespace PhotoSharingApp.Universal.Facades
{
    /// <summary>
    /// Encapsulates In-app purchases.
    /// </summary>
    public class LicensingFacade : ILicensingFacade
    {
        /// <summary>
        /// The authentication enforcement handler.
        /// </summary>
        private readonly IAuthEnforcementHandler _authEnforcementHandler;

        /// <summary>
        /// The photo service.
        /// </summary>
        private readonly IPhotoService _photoService;

        public LicensingFacade(IPhotoService photoService,
            IAuthEnforcementHandler authEnforcementHandler)
        {
            _photoService = photoService;
            _authEnforcementHandler = authEnforcementHandler;
        }

        /// <summary>
        /// Does fulfillment of all available products.
        /// </summary>
        /// <remarks>
        /// Fulfillment consists of two steps:
        /// 1. Notifying PhotoSharingApp servers of purchased IAP.
        /// 2. In case of a consumable, we want to fulfill that IAP in the store
        /// so that users are able to re-purchase.
        /// </remarks>
        private async Task DoFulfillment()
        {
            var productLicenses = CurrentAppProxy.LicenseInformation.ProductLicenses;

            // Do fulfillment of all avalaible IAPs.
            await TryFulfillGold(productLicenses[InAppPurchases.Gold]);
        }

        /// <summary>
        /// Purchases Gold with the given productId.
        /// </summary>
        /// <remarks>
        /// Please see <see cref="InAppPurchases" /> for available
        /// IAPs.
        /// </remarks>
        /// <param name="productId">The productId to purchase.</param>
        /// <exception cref="InAppPurchaseException">Thrown when an error occurs during purchase.</exception>
        /// <returns>The task.</returns>
        public async Task PurchaseGold(string productId)
        {
            await _authEnforcementHandler.CheckUserAuthentication();

            // Kick off the purchase
            var purchaseResults = await CurrentAppProxy.RequestProductPurchaseAsync(productId);

            if (purchaseResults.Status == ProductPurchaseStatus.Succeeded
                || purchaseResults.Status == ProductPurchaseStatus.NotFulfilled)
            {
                await DoFulfillment();
            }
            else
            {
                // ProductPurchaseStatus is either AlreadyPurchased or NotPurchased,
                // while latter implies either user cancellation or other failures.
                throw new InAppPurchaseException();
            }
        }

        /// <summary>
        /// Tries to fulfill a Gold IAP with the given amount.
        /// </summary>
        /// <param name="productLicense">The store product license.</param>
        private async Task TryFulfillGold(ProductLicense productLicense)
        {
            if (productLicense.IsConsumable && productLicense.IsActive)
            {
                var receipt = await CurrentAppProxy.RequestProductPurchaseAsync(productLicense.ProductId);

                // Fulfill on PhotoSharingApp servers
                var user = await _photoService.FulfillGold(receipt.ReceiptXml);

                // If previous step was successful, fulfill in Store
                await CurrentAppProxy.ReportConsumableFulfillmentAsync(productLicense.ProductId, receipt.TransactionId);

                // Now update local gold balance
                AppEnvironment.Instance.CurrentUser.GoldBalance = user.GoldBalance;
            }
        }
    }
}