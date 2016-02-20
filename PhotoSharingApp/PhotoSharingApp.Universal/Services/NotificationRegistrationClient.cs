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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using PhotoSharingApp.Portable.DataContracts;
using Windows.Storage;

namespace PhotoSharingApp.Universal.Services
{
    /// <summary>
    /// Register/update/delete notification registration for the client.
    /// </summary>
    public class NotificationRegistrationClient
    {
        private const string LocalNotificationRegistrationId = "__NHRegistrationId";
        private readonly MobileServiceClient _mobileServiceClient;

        /// <summary>
        /// Notification registration client constructor.
        /// </summary>
        public NotificationRegistrationClient()
        {
            _mobileServiceClient = AzureAppService.Current;
        }

        /// <summary>
        /// Delete the registration for locally stored registration.
        /// </summary>
        /// <returns>HttpStatusCode of the request.</returns>
        public async Task<HttpStatusCode> DeleteRegistrationAsync()
        {
            try
            {
                var settings = ApplicationData.Current.LocalSettings.Values;

                // Only if local registrationId is found then we try to delete the registration.
                if (settings.ContainsKey(LocalNotificationRegistrationId))
                {
                    string registrationId = (string)settings[LocalNotificationRegistrationId];
                    var response = await _mobileServiceClient.InvokeApiAsync<HttpStatusCode>($"notificationregistration/{registrationId}",
                        HttpMethod.Delete,
                        null);

                    // Remove the local registration id now that we have deleted it from the service.
                    settings.Remove(LocalNotificationRegistrationId);

                    return response;
                }

                // If no local registration ID is found, it is ok to continue.
                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Delete registration error.", ex);
            }
        }

        /// <summary>
        /// Registers the a notification hub handle with specified notification tags.
        /// </summary>
        /// <param name="handle">The notification handle to register with..</param>
        /// <param name="tags">The tags to register.</param>
        /// <returns>The updated notification registration.</returns>
        public async Task RegisterAsync(string handle, IEnumerable<string> tags)
        {
            var regId = await RetrieveRegistrationIdOrRequestNewOneAsync(handle);

            var deviceRegistrationContract = new DeviceRegistrationContract
            {
                Platform = "wns",
                Handle = handle,
                Tags = tags.ToArray()
            };

            var statusCode = await UpdateRegistrationAsync(regId, deviceRegistrationContract);

            if (statusCode == HttpStatusCode.Gone)
            {
                // Registration id is expired, deleting from local storage & recreating
                var settings = ApplicationData.Current.LocalSettings.Values;
                settings.Remove(LocalNotificationRegistrationId);

                regId = await RetrieveRegistrationIdOrRequestNewOneAsync(handle);
                statusCode = await UpdateRegistrationAsync(regId, deviceRegistrationContract);
            }

            if (statusCode != HttpStatusCode.OK)
            {
                throw new ServiceException("Registering for notification failed.");
            }
        }

        /// <summary>
        /// Retrieves or create a registration id and then stores it in local setting for easy access.
        /// </summary>
        /// <param name="handle">The notification handle.</param>
        /// <returns>The retrieved or newly created registration id.</returns>
        private async Task<string> RetrieveRegistrationIdOrRequestNewOneAsync(string handle)
        {
            var settings = ApplicationData.Current.LocalSettings.Values;

            if (!settings.ContainsKey(LocalNotificationRegistrationId))
            {
                try
                {
                    var response = await _mobileServiceClient.InvokeApiAsync<string>("notificationregistration",
                        HttpMethod.Post,
                        new Dictionary<string, string>
                        {
                            { "handle", handle }
                        });

                    if (response != null)
                    {
                        settings.Add(LocalNotificationRegistrationId, response);
                    }
                }
                catch (Exception)
                {
                    throw new ServiceException("Retrieve Registration id error.");
                }
            }

            return (string)settings[LocalNotificationRegistrationId];
        }

        /// <summary>
        /// Updates the existing registration with the new device registration contract with tags.
        /// </summary>
        /// <param name="registrationId">The registration id of the registration to update.</param>
        /// <param name="deviceRegistrationContract">The device registration contact to update.</param>
        /// <returns>HttpStatusCode of the operation.</returns>
        private async Task<HttpStatusCode> UpdateRegistrationAsync(string registrationId,
            DeviceRegistrationContract deviceRegistrationContract)
        {
            try
            {
                return await _mobileServiceClient.InvokeApiAsync<DeviceRegistrationContract, HttpStatusCode>(
                    $"notificationregistration/{registrationId}",
                    deviceRegistrationContract,
                    HttpMethod.Put,
                    null);
            }
            catch (Exception ex)
            {
                throw new ServiceException("Update registration error.", ex);
            }
        }
    }
}