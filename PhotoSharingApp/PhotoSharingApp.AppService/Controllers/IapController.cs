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

using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using System.Xml;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Mobile.Server.Config;
using PhotoSharingApp.AppService.Helpers;
using PhotoSharingApp.AppService.ServiceCore;
using PhotoSharingApp.AppService.Shared;
using PhotoSharingApp.AppService.Shared.Repositories;
using PhotoSharingApp.AppService.Shared.Validation;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Controllers
{
    /// <summary>
    /// Controller to manage in-app purchase receipts.
    /// </summary>
    [MobileAppController]
    public class IapController : BaseController
    {
        private readonly IIapValidator _iapValidator;
        private readonly IRepository _repository;
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Iap controller constructor.
        /// </summary>
        /// <param name="repository">Repository interface.</param>
        /// <param name="telemetryClient">Telemetry interface.</param>
        /// <param name="iapValidator">IapValidator interface.</param>
        /// <param name="userRegistrationReferenceProvider">The user registration reference provider.</param>
        public IapController(IRepository repository, TelemetryClient telemetryClient, IIapValidator iapValidator,
            IUserRegistrationReferenceProvider userRegistrationReferenceProvider)
            : base(userRegistrationReferenceProvider)
        {
            _iapValidator = iapValidator;
            _repository = repository;
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Validates and deposits Iap gold.
        /// </summary>
        /// <verb>POST</verb>
        /// <url>http://{host}/api/iap</url>
        /// <param name="receipt">Receipt provided from store.</param>
        /// <returns>User object with new balance.</returns>
        [Authorize]
        [Route("api/iap")]
        public async Task<UserContract> PostAsync(StringWrapper receipt)
        {
            _telemetryClient.TrackEvent("IapController PostAsync invoked");

            if (receipt == null)
            {
                throw ServiceExceptions.IapValidationException("IapController: Receipt is null");
            }

            var registrationReference = await ValidateAndReturnCurrentUserId();
            UserContract returnUser;

            try
            {
                int goldValue;

                var xmlReceipt = new XmlDocument();
                xmlReceipt.LoadXml(receipt.Data);

                var iapReceipt = _iapValidator.ValidateXmlSignature(xmlReceipt);

                var user = await _repository.GetUser(null, registrationReference);
                iapReceipt.UserId = user.UserId;

                var productKey = "Iap";

                if (!string.IsNullOrEmpty(iapReceipt.ProductId))
                {
                    productKey = "Iap" + iapReceipt.ProductId.Trim();
                    int.TryParse(WebConfigurationManager.AppSettings[productKey], out goldValue);
                }
                else
                {
                    throw ServiceExceptions.IapValidationException(
                        $"IapController: Product not found in configuration: {productKey}");
                }

                iapReceipt.GoldIncrement = goldValue;

                returnUser = await _repository.InsertIapPurchase(iapReceipt);
            }
            catch (DataLayerException ex)
            {
                _telemetryClient.TrackException(ex);

                if (ex.Error == DataLayerError.Unknown)
                {
                    throw ServiceExceptions.UnknownInternalFailureException(ServiceExceptions.Source);
                }

                throw ServiceExceptions.DataLayerException(ex.Message);
            }
            catch (IapValidationException ex)
            {
                _telemetryClient.TrackException(ex);

                if (ex.Error == IapValidationError.Unknown)
                {
                    throw ServiceExceptions.UnknownInternalFailureException(ServiceExceptions.Source);
                }

                throw ServiceExceptions.IapValidationException(ex.Message);
            }

            return returnUser;
        }
    }
}