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
    /// Controller to manage photos.
    /// </summary>
    [MobileAppController]
    public class PhotoController : BaseController
    {
        private readonly IPhotoValidation _photoValidation;
        private readonly IRepository _repository;
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Controller for photo operations.
        /// </summary>
        /// <param name="repository">Data layer.</param>
        /// <param name="telemetryClient">Telemetry client.</param>
        /// <param name="photoValidation">Validation layer.</param>
        /// <param name="userRegistrationReferenceProvider">The user registration reference provider.</param>
        public PhotoController(IRepository repository, TelemetryClient telemetryClient,
            IPhotoValidation photoValidation, IUserRegistrationReferenceProvider userRegistrationReferenceProvider)
            : base(userRegistrationReferenceProvider)
        {
            _repository = repository;
            _telemetryClient = telemetryClient;
            _photoValidation = photoValidation;
        }

        /// <summary>
        /// Deletes the provided photo id.
        /// </summary>
        /// <verb>DELETE</verb>
        /// <url>http://{host}/api/photo/{id}</url>
        /// <param name="id">Photo Id to be deleted.</param>
        [Authorize]
        [Route("api/photo/{id}")]
        public async Task DeleteAsync(string id)
        {
            try
            {
                _telemetryClient.TrackEvent("PhotoController DeleteAsync invoked");
                var registrationReference = await ValidateAndReturnCurrentUserId();

                await _repository.DeletePhoto(id, registrationReference);
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
        /// Gets the data for provided photo id.
        /// </summary>
        /// <verb>GET</verb>
        /// <url>http://{host}/api/photo/{id}</url>
        /// <param name="id">Photo Id for which data needs to be fetched.</param>
        /// <returns>Photo Contract.</returns>
        [Route("api/photo/{id}")]
        public async Task<PhotoContract> GetAsync(string id)
        {
            try
            {
                _telemetryClient.TrackEvent("PhotoController GetAsync invoked");

                return await _repository.GetPhoto(id);
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
        /// Inserts a new photo.
        /// </summary>
        /// <verb>POST</verb>
        /// <url>http://{host}/api/photo</url>
        /// <param name="photo">Photo Contract.</param>
        /// <returns>Photo object.</returns>
        [Authorize]
        [Route("api/photo")]
        public async Task<PhotoContract> PostAsync(PhotoContract photo)
        {
            try
            {
                _telemetryClient.TrackEvent("PhotoController PostAsync invoked");
                await ValidateAndReturnCurrentUserId();

                int goldIncrement;
                int.TryParse(WebConfigurationManager.AppSettings["NewPhotoAward"], out goldIncrement);

                return await _repository.InsertPhoto(photo, goldIncrement);
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
        /// Updates an existing photo.
        /// </summary>
        /// <verb>PUT</verb>
        /// <url>http://{host}/api/photo</url>
        /// <param name="photo">Photo Contract.</param>
        /// <returns>Photo object.</returns>
        [Authorize]
        [Route("api/photo")]
        public async Task<PhotoContract> PutAsync(PhotoContract photo)
        {
            try
            {
                _telemetryClient.TrackEvent("Auth: PhotoController PutAsync invoked");

                var registrationReference = await ValidateAndReturnCurrentUserId();

                if (!await _photoValidation.IsUserPhotoOwner(registrationReference, photo.Id))
                {
                    throw ServiceExceptions.NotAllowed();
                }

                return await _repository.UpdatePhoto(photo);
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