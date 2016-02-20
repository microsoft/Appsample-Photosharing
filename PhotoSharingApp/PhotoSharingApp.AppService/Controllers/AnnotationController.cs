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
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Mobile.Server.Config;
using PhotoSharingApp.AppService.Helpers;
using PhotoSharingApp.AppService.Notifications;
using PhotoSharingApp.AppService.ServiceCore;
using PhotoSharingApp.AppService.Shared;
using PhotoSharingApp.AppService.Shared.Repositories;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Controllers
{
    /// <summary>
    /// Controller to manage annotations.
    /// </summary>
    [MobileAppController]
    public class AnnotationController : BaseController
    {
        private readonly INotificationHandler _notificationHandler;
        private readonly IRepository _repository;
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Annotation controller constructor.
        /// </summary>
        /// <param name="repository">The repository interface.</param>
        /// <param name="telemetryClient">The telemetry client.</param>
        /// <param name="notificationHandler">The notification handler.</param>
        /// <param name="userRegistrationReferenceProvider">The user registration reference provider.</param>
        public AnnotationController(IRepository repository, TelemetryClient telemetryClient,
            INotificationHandler notificationHandler,
            IUserRegistrationReferenceProvider userRegistrationReferenceProvider) :
                base(userRegistrationReferenceProvider)
        {
            _repository = repository;
            _telemetryClient = telemetryClient;
            _notificationHandler = notificationHandler;
        }

        /// <summary>
        /// Deletes the annotation.
        /// </summary>
        /// <verb>DELETE</verb>
        /// <url>http://{host}/api/annotation/{id}</url>
        /// <param name="id">The annotation id to delete.</param>
        [Authorize]
        [Route("api/annotation/{id}")]
        public async Task DeleteAsync(string id)
        {
            try
            {
                _telemetryClient.TrackEvent("AnnotationController DeleteAsync invoked");
                var registrationReference = await ValidateAndReturnCurrentUserId();

                await _repository.DeleteAnnotation(id, registrationReference);
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
        /// Inserts the provided annotation, send a Push notification to Gold receiver, perform the required gold transactions.
        /// </summary>
        /// <verb>POST</verb>
        /// <url>http://{host}/api/annotation</url>
        /// <param name="annotation">Annotation to be inserted.</param>
        /// <returns>Inserted annotation.</returns>
        [Authorize]
        [Route("api/annotation")]
        public async Task<AnnotationContract> PostAsync(AnnotationContract annotation)
        {
            try
            {
                _telemetryClient.TrackEvent("AnnotationController PostAsync invoked");
                await ValidateAndReturnCurrentUserId();

                // Get the Gold gifting user.
                var fromUser = await _repository.GetUser(annotation.From.UserId);

                // Check to see if the gifting user has enough of a balance to support gift.
                if ((fromUser.GoldBalance - annotation.GoldCount) < 0)
                {
                    throw ServiceExceptions.UserBalanceTooLow();
                }

                var insertedAnnotation = await _repository.InsertAnnotation(annotation);

                var photoContract = await _repository.GetPhoto(annotation.PhotoId);

                try
                {
                    _telemetryClient.TrackEvent("Gold received Push notification invoked.");

                    // Send push notification to the user receiving Gold
                    await
                        _notificationHandler.PushGoldReceivedNotificationAsync(PushNotificationPlatform.Windows,
                            "user:" + annotation.PhotoOwnerId,
                            "You have received GOLD!",
                            photoContract.ThumbnailUrl, annotation.PhotoId);
                }
                catch (Exception e)
                {
                    _telemetryClient.TrackException(e);
                }

                return insertedAnnotation;
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