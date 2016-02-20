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
using Windows.ApplicationModel.Store;
using Windows.Foundation;

namespace PhotoSharingApp.Universal.Store
{
    /// <summary>
    /// Proxy class for <see cref="CurrentApp" /> and
    /// <see cref="CurrentAppSimulator" />.
    /// This class is handling mocking and therefore should be the only
    /// entry point for store licensing calls.
    /// </summary>
    public class CurrentAppProxy
    {
        /// <summary>
        /// Determines if mocking is enabled. If enabled,
        /// <see cref="CurrentAppSimulator" /> is used, otherwise
        /// <see cref="CurrentApp" />.
        /// </summary>
        public static bool IsMockEnabled { get; set; }

        /// <summary>
        /// Gets the license metadata for the current app.
        /// </summary>
        public static LicenseInformation LicenseInformation
        {
            get
            {
                return IsMockEnabled
                    ? CurrentAppSimulator.LicenseInformation
                    : CurrentApp.LicenseInformation;
            }
        }

        /// <summary>
        /// Gets the Uniform Resource Identifier (URI) of the
        /// app's listing page in the web catalog of the Windows Store.
        /// </summary>
        public static Uri LinkUri
        {
            get
            {
                return IsMockEnabled
                    ? CurrentAppSimulator.LinkUri
                    : CurrentApp.LinkUri;
            }
        }

        /// <summary>
        /// Retrieves the promotion campaign ID for the current app.
        /// </summary>
        /// <returns>The advertising campaign ID for your app.</returns>
        public static IAsyncOperation<string> GetAppPurchaseCampaignIdAsync()
        {
            return IsMockEnabled
                ? CurrentAppSimulator.GetAppPurchaseCampaignIdAsync()
                : CurrentApp.GetAppPurchaseCampaignIdAsync();
        }

        /// <summary>
        /// Requests all receipts for the purchase of the app
        /// and any in-app products.
        /// </summary>
        /// <returns>
        /// An XML-formatted string that contains all receipt
        /// information for the purchase of the app and any in-app products.
        /// </returns>
        public static IAsyncOperation<string> GetAppReceiptAsync()
        {
            return IsMockEnabled
                ? CurrentAppSimulator.GetAppReceiptAsync()
                : CurrentApp.GetAppReceiptAsync();
        }

        /// <summary>
        /// Returns a list of purchased consumable in-app products
        /// that have not been reported to the Windows Store as fulfilled.
        /// </summary>
        /// <returns>
        /// When the operation completes, a list of consumable
        /// in-app products not yet reported as fulfilled is returned
        /// (UnfulfilledConsumable objects).
        /// </returns>
        public static IAsyncOperation<IReadOnlyList<UnfulfilledConsumable>> GetUnfulfilledConsumablesAsync()
        {
            return IsMockEnabled
                ? CurrentAppSimulator.GetUnfulfilledConsumablesAsync()
                : CurrentApp.GetUnfulfilledConsumablesAsync();
        }

        /// <summary>
        /// Loads the app's listing information asynchronously.
        /// </summary>
        /// <returns>
        /// The apps' listing information. If the method fails,
        /// it returns an HRESULT error code.
        /// </returns>
        public static IAsyncOperation<ListingInformation> LoadListingInformationAsync()
        {
            return IsMockEnabled
                ? CurrentAppSimulator.LoadListingInformationAsync()
                : CurrentApp.LoadListingInformationAsync();
        }

        /// <summary>
        /// Loads the app listing information asynchronously, returning features
        /// and products in the ProductListings collection that match all supplied keywords.
        /// </summary>
        /// <param name="keywords">
        /// The list of keywords by which to filter the ProductListings
        /// collection that is returned in the ListingInformation object.
        /// </param>
        /// <returns>
        /// The app's listing information, with ProductListings collection filtered by keywords. If the method fails, it
        /// returns an HRESULT error code. If no products or features are found that match all of the given keywords, the
        /// ProductListings collection will be empty.
        /// </returns>
        public static IAsyncOperation<ListingInformation> LoadListingInformationByKeywordsAsync(
            IEnumerable<string> keywords)
        {
            return IsMockEnabled
                ? CurrentAppSimulator.LoadListingInformationByKeywordsAsync(keywords)
                : CurrentApp.LoadListingInformationByKeywordsAsync(keywords);
        }

        /// <summary>
        /// Loads the app listing information asynchronously, returning features and products in the ProductListings collection
        /// that match any of the given products IDs.
        /// </summary>
        /// <param name="productIds">The list of product IDs by which to filter the ProductListings collection.</param>
        /// <returns>
        /// The app's listing information, with ProductListings collection filtered by product IDs. If the method fails,
        /// it returns an HRESULT error code. If no products or features are found that match the given product IDs, the
        /// ProductListings collection will be empty.
        /// </returns>
        public static IAsyncOperation<ListingInformation>
            LoadListingInformationByProductIdsAsync(IEnumerable<string> productIds)
        {
            return IsMockEnabled
                ? CurrentAppSimulator.LoadListingInformationByProductIdsAsync(productIds)
                : CurrentApp.LoadListingInformationByProductIdsAsync(productIds);
        }

        /// <summary>
        /// Notifies the Windows Store that the purchase of a consumable is fulfilled and that the user has the right to access the
        /// content.
        /// </summary>
        /// <param name="productId">Identifies the consumable.</param>
        /// <param name="transactionId">Identifies a transaction that includes the purchase of the consumable (productId).</param>
        /// <returns>
        /// A value that indicates the status of fulfillment for a consumable in-app product. Possible values are defined
        /// by the FulfillmentResult enumeration.
        /// </returns>
        public static IAsyncOperation<FulfillmentResult>
            ReportConsumableFulfillmentAsync(string productId, Guid transactionId)
        {
            return IsMockEnabled
                ? CurrentAppSimulator.ReportConsumableFulfillmentAsync(productId, transactionId)
                : CurrentApp.ReportConsumableFulfillmentAsync(productId, transactionId);
        }

        /// <summary>
        /// Notifies the Windows Store that the application has fulfilled the in-app product. This product cannot be re-purchased
        /// until the app has confirmed fulfillment using this method.
        /// </summary>
        /// <param name="productId">The ID of the product that has been delivered to the user.</param>
        public static void ReportProductFulfillment(string productId)
        {
            if (IsMockEnabled)
            {
                // There is no fulfillment method in the simulator that
                // accepts one parameter.
                throw new ArgumentException();
            }

            CurrentApp.ReportProductFulfillment(productId);
        }

        /// <summary>
        /// Requests the purchase of a full app license.
        /// </summary>
        /// <param name="includeReceipt">Determines if this method should return the receipts for this app.</param>
        /// <returns>
        /// If the includeReceipt parameter is set to true, this string contains XML that represents all receipts for the
        /// app and any in-app purchases. If includeReceipt is set to false, this string is empty.
        /// </returns>
        public static IAsyncOperation<string> RequestAppPurchaseAsync(bool includeReceipt)
        {
            return IsMockEnabled
                ? CurrentAppSimulator.RequestAppPurchaseAsync(includeReceipt)
                : CurrentApp.RequestAppPurchaseAsync(includeReceipt);
        }

        /// <summary>
        /// Requests the purchase of an in-app product. Additionally, calling this method displays the UI that is used to complete
        /// the transaction via the Windows Store.
        /// </summary>
        /// <param name="productId">Specifies the id of the in-app product.</param>
        /// <returns>The results of the in-app product purchase request.</returns>
        public static IAsyncOperation<PurchaseResults>
            RequestProductPurchaseAsync(string productId)
        {
            return IsMockEnabled
                ? CurrentAppSimulator.RequestProductPurchaseAsync(productId)
                : CurrentApp.RequestProductPurchaseAsync(productId);
        }

        /// <summary>
        /// Requests the purchase of an in-app product. Additionally, calling this method displays the UI that is used to complete
        /// the transaction via the Windows Store.
        /// </summary>
        /// <param name="productId">Specifies the id of the in-app product.</param>
        /// <param name="offerId">
        /// The specific in-app feature or content within the large purchase catalog represented on the
        /// Windows Store by the productId. This value correlates with the content your app is responsible for fulfilling. The
        /// Windows Store only uses this value to itemize the PurchaseResults.
        /// </param>
        /// <param name="displayProperties">
        /// The name of the app feature or content offer that is displayed to the user at time of
        /// purchase.
        /// </param>
        /// <returns>The results of the in-app product purchase request.</returns>
        public static IAsyncOperation<PurchaseResults> RequestProductPurchaseAsync(string productId, string offerId,
            ProductPurchaseDisplayProperties displayProperties)
        {
            return IsMockEnabled
                ? CurrentAppSimulator.RequestProductPurchaseAsync(productId, offerId, displayProperties)
                : CurrentApp.RequestProductPurchaseAsync(productId, offerId, displayProperties);
        }
    }
}