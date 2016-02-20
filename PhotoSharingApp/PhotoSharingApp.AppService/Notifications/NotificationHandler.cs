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

using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Notifications
{
    /// <summary>
    /// Sends Push notification to registered tag.
    /// </summary>
    public class NotificationHandler : INotificationHandler
    {
        /// <summary>
        /// Sends a Push notification for receiving Gold to a specified registered user.
        /// The thumbnail of the photo receiving gold is also sent.
        /// Photo id of the photo is sent to enable launching of the photo on the client on
        /// clicking the notification.
        /// </summary>
        /// <param name="platform">The platform specific notification service to use.</param>
        /// <param name="userNotificationTag">The user notification tag to send notification to.</param>
        /// <param name="message">The message to display on the notification toast.</param>
        /// <param name="thumbnailUrl">The thumbnail Url of the photo to display in notification.</param>
        /// <param name="photoId">The photoId of the photo to send as activation argument.</param>
        /// <returns>HttpStatusCode of the request.</returns>
        public async Task<HttpStatusCode> PushGoldReceivedNotificationAsync(PushNotificationPlatform platform,
            string userNotificationTag, string message,
            string thumbnailUrl, string photoId)
        {
            return await SendPush(platform, userNotificationTag, message, thumbnailUrl, photoId);
        }

        /// <summary>
        /// Send a text push notification to a specified registered user.
        /// </summary>
        /// <param name="platform">The platform specific notification service to use.</param>
        /// <param name="userNotificationTag">The user notification tag to send notification to.</param>
        /// <param name="message">The message to display on the notification toast.</param>
        /// <returns>HttpStatusCode of the request.</returns>
        public async Task<HttpStatusCode> PushMessageNotificationAsync(PushNotificationPlatform platform,
            string userNotificationTag, string message)
        {
            return await SendPush(platform, userNotificationTag, message);
        }

        /// <summary>
        /// Sends a rich push notification message with a thumbnail image and arguments to lauch the image
        /// if the optional parameters are passed.
        /// Otherwise, only a simple text push message is sent.
        /// </summary>
        /// <param name="platform">The platform specific notification service to use.</param>
        /// <param name="userNotificationTag">The user notification tag to send notification to.</param>
        /// <param name="message">The message to display on the notification toast.</param>
        /// <param name="thumbnailUrl">Optional - The thumbnail Url of the photo to display in notification.</param>
        /// <param name="photoId">Optional - The photoId of the photo to send as activation argument.</param>
        /// <returns>HttpStatusCode of the request.</returns>
        private async Task<HttpStatusCode> SendPush(PushNotificationPlatform platform, string userNotificationTag,
            string message,
            string thumbnailUrl = "", string photoId = "")
        {
            NotificationOutcome outcome = null;
            var statusCode = HttpStatusCode.InternalServerError;

            switch (platform)
            {
                case PushNotificationPlatform.Windows:

                    // Windows 10
                    var toast =
                        $@"<toast launch=""{photoId
                            }"">
                                    <visual>
                                        <binding template=""ToastGeneric"">
                                            <text>
                                                {
                            message
                            }
                                            </text>
                                            <image src=""{
                            thumbnailUrl
                            }"" placement=""inline"" />
                                        </binding>
                                    </visual>
                                   </toast>";

                    outcome =
                        await
                            Models.NotificationsHubModel.Instance.Hub.SendWindowsNativeNotificationAsync(toast,
                                userNotificationTag);
                    break;

                case PushNotificationPlatform.Apple:
                    var alert = "{\"aps\":{\"alert\":\"" + message + "\"}}";
                    outcome =
                        await
                            Models.NotificationsHubModel.Instance.Hub.SendAppleNativeNotificationAsync(alert,
                                userNotificationTag);
                    break;

                case PushNotificationPlatform.Android:
                    var notify = "{ \"data\" : {\"message\":\"" + ": " + message + "\"}}";
                    outcome =
                        await
                            Models.NotificationsHubModel.Instance.Hub.SendGcmNativeNotificationAsync(notify, userNotificationTag);
                    break;
            }

            if (outcome != null)
            {
                if (!((outcome.State == NotificationOutcomeState.Abandoned) ||
                      (outcome.State == NotificationOutcomeState.Unknown)))
                {
                    statusCode = HttpStatusCode.OK;
                }
            }

            return statusCode;
        }
    }
}