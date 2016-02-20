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
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Mobile.Server.Config;
using PhotoSharingApp.AppService.Helpers;
using PhotoSharingApp.AppService.ServiceCore;
using PhotoSharingApp.AppService.Shared;
using PhotoSharingApp.AppService.Shared.Repositories;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Controllers
{
    /// <summary>
    /// Controller to manage hero photos.
    /// </summary>
    [MobileAppController]
    public class HeroPhotoController : BaseController
    {
        private readonly IRepository _repository;
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Controller for hero photo operations.
        /// </summary>
        /// <param name="repository">Data layer.</param>
        /// <param name="telemetryClient">Telemetry client.</param>
        /// <param name="userRegistrationReferenceProvider">The user registration reference provider.</param>
        public HeroPhotoController(IRepository repository, TelemetryClient telemetryClient,
            IUserRegistrationReferenceProvider userRegistrationReferenceProvider)
            : base(userRegistrationReferenceProvider)
        {
            _repository = repository;
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Gets the provided count of hero photos.
        /// </summary>
        /// <verb>GET</verb>
        /// <url>http://{host}/api/herophoto?count={count}</url>
        /// <param name="count">Number of hero photos.</param>
        /// <returns>List of hero photos.</returns>
        [Route("api/herophoto")]
        public async Task<IList<PhotoContract>> GetAsync([FromUri] int count)
        {
            try
            {
                _telemetryClient.TrackEvent("HeroPhotoController GetAsync invoked ");
                var daysOld = int.Parse(WebConfigurationManager.AppSettings["HeroImagesDaysOld"]);

                return await _repository.GetHeroPhotos(count, daysOld);
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
        }
    }
}