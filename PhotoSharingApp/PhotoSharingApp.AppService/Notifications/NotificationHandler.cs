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
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using NotificationsExtensions.Badges;
using NotificationsExtensions.Tiles;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Notifications
{
    /// <summary>
    /// Sends Push notification to registered tag.
    /// </summary>
    public class NotificationHandler : INotificationHandler
    {
        /// <summary>
        /// Generates badge notification of goldCount change for receiver.
        /// </summary>
        /// <param name="goldCount">The goldCount of the user.</param>
        /// <returns>String representation of badge object.</returns>
        private string GenerateBadgeNotification(uint goldCount)
        {
            BadgeNumericNotificationContent badgeContent = new BadgeNumericNotificationContent(goldCount);
            return badgeContent.GetContent();
        }

        /// <summary>
        /// Generates tile notification of gold notification using the receiver's picture
        /// </summary>
        /// <param name="thumbnailUrl">Image that received gold.</param>
        /// <returns>String representation of tile object.</returns>
        private string GenerateTileNotification(string thumbnailUrl)
        {
            var content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileSmall = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = thumbnailUrl
                            }
                        }
                    },

                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = thumbnailUrl
                            }
                        }
                    },

                    TileLarge = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = thumbnailUrl
                            }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage
                            {
                                Source = thumbnailUrl
                            }
                        }
                    }
                }
            };

            return content.GetContent();
        }

        /// <summary>
        /// Sends a rich push notification message with a thumbnail image and arguments to launch the image.
        /// Also sends live tile and badge notification 
        /// if the optional parameters are passed.
        /// Otherwise, only a simple text push message is sent.
        /// </summary>
        /// <param name="platform">The platform specific notification service to use.</param>
        /// <param name="userNotificationTag">The user notification tag to send notification to.</param>
        /// <param name="message">The message to display on the notification toast.</param>
        /// <param name="thumbnailUrl">Optional - The thumbnail Url of the photo to display in notification.</param>
        /// <param name="photoId">Optional - The photoId of the photo to send as activation argument.</param>
        /// <param name="goldCount">Optional - The goldCount of the photo's owner.</param>
        /// <returns>HttpStatusCode of the request.</returns>
        public async Task<HttpStatusCode> SendPushAsync(PushNotificationPlatform platform, string userNotificationTag,
            string message,
            string thumbnailUrl = "", string photoId = "", int goldCount = 0)
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

                    var tile = GenerateTileNotification(thumbnailUrl);
                    var badge = GenerateBadgeNotification(Convert.ToUInt32(goldCount));
                    outcome =
                        await
                            Models.NotificationsHubModel.Instance.Hub.SendWindowsNativeNotificationAsync(toast,
                                userNotificationTag);
                    outcome =
                        await
                            Models.NotificationsHubModel.Instance.Hub.SendWindowsNativeNotificationAsync(tile,
                                userNotificationTag);
                    outcome =
                        await
                            Models.NotificationsHubModel.Instance.Hub.SendWindowsNativeNotificationAsync(badge,
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