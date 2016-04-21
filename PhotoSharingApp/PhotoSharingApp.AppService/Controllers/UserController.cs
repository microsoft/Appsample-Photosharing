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
    /// Controller to manager the User.
    /// </summary>
    [MobileAppController]
    public class UserController : BaseController
    {
        private readonly IRepository _repository;
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Controller to handle creation and acquisition of user object.
        /// </summary>
        /// <param name="repository">Data access layer.</param>
        /// <param name="telemetryClient">ApplicationInsights telemetry client.</param>
        /// <param name="userRegistrationReferenceProvider">The user registration reference provider.</param>
        public UserController(IRepository repository, TelemetryClient telemetryClient,
            IUserRegistrationReferenceProvider userRegistrationReferenceProvider)
            : base(userRegistrationReferenceProvider)
        {
            _repository = repository;
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        /// The client is requesting the user object for the currently authenticated user.
        /// </summary>
        /// <verb>GET</verb>
        /// <url>http://{host}/api/user</url>
        /// <returns>User object based on the UserContract.</returns>
        [Authorize]
        [Route("api/user")]
        public async Task<UserContract> GetUser()
        {
            _telemetryClient.TrackEvent("UserController GetUser invoked");

            var registrationReference = await ValidateAndReturnCurrentUserId();

            try
            {
                var currentUser = await _repository.GetUser(null, registrationReference);

                // We need to add a record for the current authenticated user
                if (currentUser.RegistrationReference == null)
                {
                    _telemetryClient.TrackEvent("No user found, creating new user");

                    currentUser = await _repository.CreateUser(registrationReference);
                }

                return currentUser;
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
        /// Generic update to the user profile.
        /// </summary>
        /// <verb>POST</verb>
        /// <url>http://{host}/api/user</url>
        /// <param name="user">User object.</param>
        /// <returns>A fresh UserContract object containing the updated gold balance if adjusted.</returns>
        [Authorize]
        [Route("api/user")]
        public async Task<UserContract> UpdateUserProfile(UserContract user)
        {
            try
            {
                _telemetryClient.TrackEvent("UserController UpdateUserProfile invoked");

                var currentUserId = await ValidateAndReturnCurrentUserId();

                // A user should only be able to update his/her own profile.
                if (!currentUserId.Equals(user.RegistrationReference))
                {
                    throw ServiceExceptions.NotAllowed();
                }

                // Check if the user owns the photo that is passed in as profile photo
                var photo = await _repository.GetPhoto(user.ProfilePhotoId);

                if (!photo.User.UserId.Equals(user.UserId))
                {
                    throw ServiceExceptions.NotAllowed();
                }

                // Refreshing profile photo url
                user.ProfilePhotoUrl = photo.ThumbnailUrl;

                var existingUser = await _repository.UpdateUser(user);

                return existingUser;
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