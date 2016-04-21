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
    /// Controller to manage the User photo.
    /// </summary>
    [MobileAppController]
    public class UserPhotoController : BaseController
    {
        private readonly IRepository _repository;
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// User photo controller constructor.
        /// </summary>
        /// <param name="repository">The repository interface.</param>
        /// <param name="telemetryClient">The telemetry client.</param>
        /// <param name="userRegistrationReferenceProvider">The user registration reference provider.</param>
        public UserPhotoController(IRepository repository, TelemetryClient telemetryClient,
            IUserRegistrationReferenceProvider userRegistrationReferenceProvider)
            : base(userRegistrationReferenceProvider)
        {
            _repository = repository;
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Gets the paged photostream for the authorized user.
        /// </summary>
        /// <verb>GET</verb>
        /// <url>http://{host}/api/userphoto?continuationToken={continuationToken}</url>
        /// <param name="continuationToken">Continuation token for paged response.</param>
        /// <returns>Paged photo contracts which include flagged and non-flagged photos.</returns>
        [Authorize]
        [Route("api/userphoto")]
        public async Task<PagedResponse<PhotoContract>> GetPagedAsync([FromUri] string continuationToken)
        {
            try
            {
                _telemetryClient.TrackEvent("UserPhotoController GetPagedAsync invoked");

                var registrationReference = await ValidateAndReturnCurrentUserId();

                var user = await _repository.GetUser(null, registrationReference);

                var stream = await _repository.GetUserPhotoStream(user.UserId, continuationToken, true);

                return stream;
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

        /// <summary>
        /// Gets a user's paged photostream.
        /// </summary>
        /// <verb>GET</verb>
        /// <url>http://{host}/api/userphoto/{userId}?continuationToken={continuationToken}</url>
        /// <param name="userId">The userId.</param>
        /// <param name="continuationToken">Continuation token for paged response.</param>
        /// <returns>Paged photo contracts which includes non-flagged photos only.</returns>
        [Route("api/userphoto/{userId}")]
        public async Task<PagedResponse<PhotoContract>> GetPagedAsync(string userId, [FromUri] string continuationToken)
        {
            try
            {
                _telemetryClient.TrackEvent("UserPhotoController GetPagedAsync invoked");

                var user = await _repository.GetUser(userId);

                var stream = await _repository.GetUserPhotoStream(user.UserId, continuationToken);

                return stream;
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