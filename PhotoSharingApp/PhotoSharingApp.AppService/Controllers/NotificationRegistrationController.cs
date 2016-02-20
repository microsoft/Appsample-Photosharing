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
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Controllers
{
    /// <summary>
    /// Controller to manage notification registrations.
    /// </summary>
    [MobileAppController]
    public class NotificationRegistrationController : ApiController
    {
        private readonly NotificationHubClient _hub;

        /// <summary>
        /// Initialize an instance of notification hub for registration.
        /// </summary>
        public NotificationRegistrationController()
        {
            _hub = Models.NotificationsHubModel.Instance.Hub;
        }

        /// <summary>
        /// Deletes the registration for a specific registration id.
        /// </summary>
        /// <verb>DELETE</verb>
        /// <url>http://{host}/api/notificationregistration/{id}</url>
        /// <param name="id">The registration id to delete.</param>
        /// <returns>HttpsStatusCode of the request.</returns>
        [Route("api/notificationregistration/{id}")]
        public async Task<HttpStatusCode> Delete(string id)
        {
            try
            {
                await _hub.DeleteRegistrationAsync(id);
                return HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        /// <summary>
        /// Creates a registration id.
        /// </summary>
        /// <verb>POST</verb>
        /// <url>http://{host}/api/notificationregistration?handle={handle}</url>
        /// <param name="handle">The notification handle.</param>
        /// <returns>The registration id for the notification handle.</returns>
        [Route("api/notificationregistration")]
        public async Task<string> Post([FromUri] string handle)
        {
            string notificationRegistrationId = null;

            // Make sure there are no existing registrations for this push handle (used for iOS and Android)
            if (handle != null)
            {
                var registrations = await _hub.GetRegistrationsByChannelAsync(handle, 100);

                foreach (var registration in registrations)
                {
                    if (notificationRegistrationId == null)
                    {
                        notificationRegistrationId = registration.RegistrationId;
                    }
                    else
                    {
                        await _hub.DeleteRegistrationAsync(registration);
                    }
                }
            }

            if (notificationRegistrationId == null)
            {
                notificationRegistrationId = await _hub.CreateRegistrationIdAsync();
            }

            return notificationRegistrationId;
        }

        /// <summary>
        /// This creates or updates a registration.
        /// </summary>
        /// <verb>PUT</verb>
        /// <url>http://{host}/api/notificationregistration/{id}</url>
        /// <param name="id">The registration id to update.</param>
        /// <param name="deviceUpdate">The contract with the details to be updated.</param>
        /// <returns>HTTPStatusCode of the request.</returns>
        [Route("api/notificationregistration/{id}")]
        public async Task<HttpStatusCode> Put(string id, DeviceRegistrationContract deviceUpdate)
        {
            RegistrationDescription registration;

            switch (deviceUpdate.Platform)
            {
                case "mpns":
                    registration = new MpnsRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "wns":
                    registration = new WindowsRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "apns":
                    registration = new AppleRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "gcm":
                    registration = new GcmRegistrationDescription(deviceUpdate.Handle);
                    break;
                default:
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            registration.RegistrationId = id;
            registration.Tags = new HashSet<string>(deviceUpdate.Tags);

            try
            {
                await _hub.CreateOrUpdateRegistrationAsync(registration);
                return HttpStatusCode.OK;
            }
            catch (MessagingException e)
            {
                var webex = e.InnerException as WebException;

                if (webex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = (HttpWebResponse)webex.Response;

                    if (response.StatusCode == HttpStatusCode.Gone)
                    {
                        // Client will force a refresh for a new id after receiving this message.
                        return HttpStatusCode.Gone;
                    }
                }

                return HttpStatusCode.InternalServerError;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}