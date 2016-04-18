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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using PhotoSharingApp.Portable.DataContracts;
using PhotoSharingApp.Portable.Extensions;
using PhotoSharingApp.Universal.ContractModelConverterExtensions;
using PhotoSharingApp.Universal.Editing;
using PhotoSharingApp.Universal.Models;
using Windows.Networking.PushNotifications;
using Windows.System.Profile;

namespace PhotoSharingApp.Universal.Services
{
    /// <summary>
    /// The service client that hits the real service via HTTP(S).
    /// </summary>
    public class ServiceClient : IPhotoService
    {
        private readonly IAuthenticationHandler _authenticationHandler;
        private readonly MobileServiceClient _mobileServiceClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceClient" /> class.
        /// </summary>
        /// <param name="authenticationHandler">The authentication handler.</param>
        public ServiceClient(IAuthenticationHandler authenticationHandler)
        {
            _authenticationHandler = authenticationHandler;

            _mobileServiceClient = AzureAppService.Current;
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <returns>The inserted category.</returns>
        public async Task<Category> CreateCategory(string categoryName)
        {
            try
            {
                var result = await _mobileServiceClient.InvokeApiAsync<CategoryContract>($"category/{categoryName}", 
                    HttpMethod.Post,
                    null);

                return result.ToDataModel();
            }
            catch (MobileServiceInvalidOperationException invalidOperationException)
            {
                var content = await invalidOperationException.Response.Content.ReadAsStringAsync();
                var serviceFault = TryGetServiceFault(content);

                if (invalidOperationException.Response.StatusCode == HttpStatusCode.Forbidden
                    && serviceFault?.Code == 3750)
                {
                    throw new CategoryMatchedException($"Category {categoryName} already exists", 
                        invalidOperationException);
                }

                throw new ServiceException("CreateCategory error", invalidOperationException);
            }
            catch (Exception e)
            {
                throw new ServiceException("CreateCategory error", e);
            }
        }

        /// <summary>
        /// Deletes the photo.
        /// </summary>
        /// <param name="photo">The photo.</param>
        public async Task DeletePhoto(Photo photo)
        {
            try
            {
                await _mobileServiceClient.InvokeApiAsync($"photo/{photo.Id}",
                    HttpMethod.Delete,
                    null);
            }
            catch (Exception e)
            {
                throw new ServiceException("DeletePhoto error", e);
            }
        }

        /// <summary>
        /// Fulfills gold on the service.
        /// </summary>
        /// <param name="productReceipt">The product receipt.</param>
        /// <returns>The user with an updated gold balance.</returns>
        public async Task<User> FulfillGold(string productReceipt)
        {
            try
            {
                var productReceiptWrapper = new StringWrapper
                {
                    Data = productReceipt
                };

                var userContract =
                    await _mobileServiceClient.InvokeApiAsync<StringWrapper, UserContract>("iap",
                        productReceiptWrapper,
                        HttpMethod.Post,
                        null);

                return userContract.ToDataModel();
            }
            catch (Exception e)
            {
                throw new ServiceException("FulfillGold error", e);
            }
        }

        /// <summary>
        /// Gets the available authentication providers.
        /// </summary>
        /// <returns>The authentication providers.</returns>
        public List<MobileServiceAuthenticationProvider> GetAvailableAuthenticationProviders()
        {
            return _authenticationHandler.AuthenticationProviders;
        }

        /// <summary>
        /// Retrieves all categories without thumbnails.
        /// </summary>
        /// <returns>The categories.</returns>
        public async Task<List<Category>> GetCategories()
        {
            try
            {
                var result = await _mobileServiceClient.InvokeApiAsync<List<CategoryContract>>("categories",
                    HttpMethod.Get,
                    null);

                return result.Select(c => c.ToDataModel()).ToList();
            }
            catch (Exception e)
            {
                throw new ServiceException("GetCategories error", e);
            }
        }

        /// <summary>
        /// Gets the configuration data.
        /// </summary>
        /// <returns>The configuration data.</returns>
        public async Task<Config> GetConfig()
        {
            try
            {
                var result = await _mobileServiceClient.InvokeApiAsync<ConfigContract>("config", 
                    HttpMethod.Get,
                    null);

                return result.ToDataModel();
            }
            catch (Exception e)
            {
                throw new ServiceException("GetConfig error", e);
            }
        }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <returns>The details for the user that is logged in.</returns>
        public async Task<User> GetCurrentUser()
        {
            try
            {
                var result = await _mobileServiceClient.InvokeApiAsync<UserContract>("user", 
                    HttpMethod.Get,
                    null);

                return result.ToDataModel();
            }
            catch (MobileServiceInvalidOperationException invalidOperationException)
            {
                if (invalidOperationException.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedException();
                }

                throw new ServiceException("GetCurrentUser error", invalidOperationException);
            }
            catch (Exception e)
            {
                throw new ServiceException("GetCurrentUser error", e);
            }
        }

        /// <summary>
        /// Retrieves hero images.
        /// </summary>
        /// <param name="count">Number of hero images.</param>
        /// <returns>The images.</returns>
        public async Task<List<Photo>> GetHeroImages(int count)
        {
            try
            {
                var result = await _mobileServiceClient.InvokeApiAsync<IList<PhotoContract>>("herophoto",
                    HttpMethod.Get,
                    new Dictionary<string, string>
                    {
                        { "count", count.ToString() }
                    });

                return result.Select(p => p.ToDataModel()).ToList();
            }
            catch (Exception e)
            {
                throw new ServiceException("GetHeroImages error", e);
            }
        }

        /// <summary>
        /// Gets the leaderboard statistical data
        /// </summary>
        /// <returns>The leaderboard</returns>
        public async Task<Leaderboard> GetLeaderboardData(int mostGoldCategoriesCount, int mostGoldPhotosCount,
            int mostGoldUsersCount, int mostGivingUsersCount)
        {
            try
            {
                var result = await _mobileServiceClient.InvokeApiAsync<LeaderboardContract>("leaderboard",
                    HttpMethod.Get,
                    new Dictionary<string, string>
                    {
                        { "mostGoldCategoriesCount", mostGoldCategoriesCount.ToString() },
                        { "mostGoldPhotosCount", mostGoldPhotosCount.ToString() },
                        { "mostGoldUsersCount", mostGoldUsersCount.ToString() },
                        { "mostGivingUsersCount", mostGivingUsersCount.ToString() }
                    });

                return result.ToDataModel();
            }
            catch (Exception e)
            {
                throw new ServiceException("GetLeaderboardData error", e);
            }
        }

        /// <summary>
        /// Gets the photo details for the given photo id.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <returns>The photo.</returns>
        public async Task<Photo> GetPhotoDetails(string photoId)
        {
            try
            {
                var result = await _mobileServiceClient.InvokeApiAsync<PhotoContract>($"photo/{photoId}",
                    HttpMethod.Get,
                    new Dictionary<string, string>
                    {
                        { "id", photoId }
                    });

                return result.ToDataModel();
            }
            catch (Exception e)
            {
                throw new ServiceException("GetPhotoDetails error", e);
            }
        }

        /// <summary>
        /// Gets photos for the given category id.
        /// </summary>
        /// <param name="categoryId">The identifier.</param>
        /// <param name="continuationToken">Optional. The continuation token. By default, null.</param>
        /// <returns>The photos.</returns>
        public async Task<PagedResponse<Photo>> GetPhotosForCategoryId(string categoryId,
            string continuationToken = null)
        {
            try
            {
                if (continuationToken == null)
                {
                    continuationToken = string.Empty;
                }

                var result = await _mobileServiceClient.InvokeApiAsync<PagedResponse<PhotoContract>>($"category/{categoryId}",
                    HttpMethod.Get,
                    new Dictionary<string, string>
                    {
                        { "continuationToken", continuationToken }
                    });

                return new PagedResponse<Photo>
                {
                    ContinuationToken = result.ContinuationToken,
                    Items = result.Items.Select(i => i.ToDataModel()).ToList()
                };
            }
            catch (Exception e)
            {
                throw new ServiceException("GetPhotosForCategory error", e);
            }
        }

        /// <summary>
        /// Gets photos uploaded by the current user.
        /// </summary>
        /// <param name="continuationToken">Optional. The continuation token. By default, null.</param>
        /// <returns>The photos.</returns>
        public async Task<PagedResponse<Photo>> GetPhotosForCurrentUser(string continuationToken = null)
        {
            try
            {
                if (continuationToken == null)
                {
                    continuationToken = string.Empty;
                }

                var result = await _mobileServiceClient.InvokeApiAsync<PagedResponse<PhotoContract>>("userphoto",
                    HttpMethod.Get,
                    new Dictionary<string, string>
                    {
                        { "continuationToken", continuationToken }
                    });

                return new PagedResponse<Photo>
                {
                    ContinuationToken = result.ContinuationToken,
                    Items = result.Items.Select(i => i.ToDataModel()).ToList()
                };
            }
            catch (Exception e)
            {
                throw new ServiceException("GetPhotosForCurrentUser error", e);
            }
        }

        /// <summary>
        /// Gets photos uploaded by the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="continuationToken">Optional. The continuation token. By default, null.</param>
        /// <returns>The photos.</returns>
        public async Task<PagedResponse<Photo>> GetPhotosForUser(User user, string continuationToken = null)
        {
            try
            {
                if (continuationToken == null)
                {
                    continuationToken = string.Empty;
                }

                var result = await _mobileServiceClient.InvokeApiAsync<PagedResponse<PhotoContract>>($"userphoto/{user.UserId}",
                    HttpMethod.Get,
                    new Dictionary<string, string>
                    {
                        { "continuationToken", continuationToken }
                    });

                return new PagedResponse<Photo> {
                    ContinuationToken = result.ContinuationToken,
                    Items = result.Items.Select(i => i.ToDataModel()).ToList()
                };
            }
            catch (Exception e)
            {
                throw new ServiceException("GetPhotosForUser error", e);
            }
        }

        /// <summary>
        /// Retrieves SAS Urls for uploading photos.
        /// </summary>
        /// <returns>The SAS Urls.</returns>
        public async Task<List<SasContract>> GetSasUrls()
        {
            try
            {
                return await _mobileServiceClient.InvokeApiAsync<List<SasContract>>("sasurl",
                    HttpMethod.Get,
                    null);
            }
            catch (Exception e)
            {
                throw new ServiceException("GetSasUrls error", e);
            }
        }

        /// <summary>
        /// Retrieves top categories with thumbnails.
        /// </summary>
        /// <param name="categoryThumbnailsCount">The number of thumbnails per each category.</param>
        /// <param name="continuationToken">Optional. The continuation token. By default, null.</param>
        /// <returns>The category list.</returns>
        public async Task<List<CategoryPreview>> GetTopCategories(int categoryThumbnailsCount, string continuationToken = null)
        {
            try
            {
                var result = await _mobileServiceClient.InvokeApiAsync<List<CategoryPreviewContract>>("category",
                    HttpMethod.Get,
                    new Dictionary<string, string>
                    {
                        { "numberOfThumbnails", categoryThumbnailsCount.ToString() }
                    });

                return result.Select(c => c.ToDataModel()).ToList();
            }
            catch (Exception e)
            {
                throw new ServiceException("GetTopCategories error", e);
            }
        }

        /// <summary>
        /// Posts the annotation.
        /// </summary>
        /// <param name="photo">The photo.</param>
        /// <param name="annotationText">The text.</param>
        /// <param name="goldCount">The amount of gold being given.</param>
        /// <returns>The annotation including the id.</returns>
        public async Task<Annotation> PostAnnotation(Photo photo, string annotationText, int goldCount)
        {
            try
            {
                var annotationContract = new AnnotationContract
                {
                    CreatedAt = DateTime.Now,
                    Text = annotationText,
                    From = AppEnvironment.Instance.CurrentUser.ToDataContract(),
                    PhotoId = photo.Id,
                    PhotoOwnerId = photo.User.UserId,
                    GoldCount = goldCount
                };

                var result =
                    await _mobileServiceClient.InvokeApiAsync<AnnotationContract, AnnotationContract>("annotation",
                        annotationContract,
                        HttpMethod.Post,
                        new Dictionary<string, string>());

                if (result != null)
                {
                    photo.GoldCount += result.GoldCount;

                    // In this case we know how much gold we have given,
                    // so we can reduce the cached amount instead of reloading
                    // the current user.
                    if (AppEnvironment.Instance.CurrentUser != null)
                    {
                        AppEnvironment.Instance.CurrentUser.GoldBalance -= result.GoldCount;
                    }
                }

                return result.ToDataModel();
            }
            catch (MobileServiceInvalidOperationException invalidOperationException)
            {
                var content = await invalidOperationException.Response.Content.ReadAsStringAsync();
                var serviceFault = TryGetServiceFault(content);

                if (invalidOperationException.Response.StatusCode == HttpStatusCode.Forbidden
                    && serviceFault?.Code == 3999)
                {
                    throw new InsufficientBalanceException("Users balance is too low",
                        invalidOperationException);
                }

                throw new ServiceException("PostAnnotation error", invalidOperationException);
            }
            catch (Exception e)
            {
                throw new ServiceException("PostAnnotation error", e);
            }
        }

        /// <summary>
        /// Registers the signed in user on the device for push notifications
        /// </summary>
        private async void RegisterForNotifications()
        {
            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

            try
            {
                var currentUserId = AppEnvironment.Instance.CurrentUser.UserId;
                var registrationClient = new NotificationRegistrationClient();
                await registrationClient.RegisterAsync(channel.Uri, new[] { "user:" + currentUserId });
            }
            catch (Exception)
            {
                // We can ignore this exception, as this may happend when not internet connection is available.
            }
        }

        /// <summary>
        /// Removes the Annotation.
        /// </summary>
        /// <param name="annotation">The Annotation.</param>
        public async Task RemoveAnnotation(Annotation annotation)
        {
            try
            {
                await _mobileServiceClient.InvokeApiAsync($"annotation/{annotation.Id}",
                    HttpMethod.Delete,
                    null);
            }
            catch (Exception e)
            {
                throw new ServiceException("RemoveAnnotation error", e);
            }
        }

        /// <summary>
        /// Reports the Annotation as inappropriate.
        /// </summary>
        /// <param name="annotation">The annotation to report.</param>
        public async Task ReportAnnotation(Annotation annotation)
        {
            try
            {
                var reportContract = new ReportContract
                {
                    ReportReason = ReportReason.Inappropriate,
                    ContentId = annotation.Id,
                    ContentType = ContentType.Annotation
                };

                await _mobileServiceClient.InvokeApiAsync<ReportContract, ReportContract>("report",
                    reportContract,
                    HttpMethod.Post,
                    null);
            }
            catch (Exception e)
            {
                throw new ServiceException("ReportAnnotation error", e);
            }
        }

        /// <summary>
        /// Reports the photo as inappropriate.
        /// </summary>
        /// <param name="photo">The photo to report.</param>
        /// <param name="reportReason">The reason for the report.</param>
        public async Task ReportPhoto(Photo photo, ReportReason reportReason)
        {
            try
            {
                var reportContract = new ReportContract
                {
                    ReportReason = reportReason,
                    ContentId = photo.Id,
                    ContentType = ContentType.Photo
                };

                await _mobileServiceClient.InvokeApiAsync<ReportContract, ReportContract>("report",
                    reportContract,
                    HttpMethod.Post,
                    null);
            }
            catch (Exception e)
            {
                throw new ServiceException("ReportPhoto error", e);
            }
        }

        /// <summary>
        /// Checks if credentials are available and not expired,
        /// which are then used to restore the sign in status.
        /// Registers the user for Push notifications.
        /// The user is not actively taken to a login page.
        /// </summary>
        public async Task RestoreSignInStatusAsync()
        {
            var user = await _authenticationHandler.RestoreSignInStatus();

            if (user != null)
            {
                AppEnvironment.Instance.CurrentUser = user;

                // To enable existing users to register for Push notifications without having to
                // log out and log in again.
                RegisterForNotifications();
            }
        }

        /// <summary>
        /// Signs the user in and registers the user for Push notifications.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public async Task SignInAsync(MobileServiceAuthenticationProvider provider)
        {
            await _authenticationHandler.AuthenticateAsync(provider);

            var user = await GetCurrentUser();
            AppEnvironment.Instance.CurrentUser = user;

            RegisterForNotifications();
        }

        /// <summary>
        /// Signs out user and deletes push notification registration.
        /// </summary>
        public async Task SignOutAsync()
        {
            await _authenticationHandler.LogoutAsync();

            // Delete push notification registration for the user.
            var registrationClient = new NotificationRegistrationClient();
            await registrationClient.DeleteRegistrationAsync();
        }

        private ServiceFaultContract TryGetServiceFault(string serializedContent)
        {
            if (serializedContent == null)
            {
                return null;
            }

            try
            {
                var serviceFault = JsonConvert.DeserializeObject<ServiceFaultContract>(serializedContent);
                return serviceFault;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Updates the photo.
        /// </summary>
        /// <param name="photo">The photo.</param>
        public async Task UpdatePhoto(Photo photo)
        {
            try
            {
                var photoContract = photo.ToDataContract();

                await _mobileServiceClient.InvokeApiAsync<PhotoContract, PhotoContract>("photo",
                    photoContract,
                    HttpMethod.Put,
                    null);
            }
            catch (Exception e)
            {
                throw new ServiceException("UpdatePhoto error", e);
            }
        }

        /// <summary>
        /// Updates the current user's profile photo
        /// </summary>
        /// <param name="photo">The new profile photo</param>
        /// <returns>The updated user</returns>
        public async Task<User> UpdateUserProfilePhoto(Photo photo)
        {
            try
            {
                var userContract = AppEnvironment.Instance.CurrentUser.ToDataContract();
                userContract.ProfilePhotoId = photo.Id;

                var result = await _mobileServiceClient.InvokeApiAsync<UserContract, UserContract>("user",
                    userContract,
                    HttpMethod.Post,
                    null);

                var updatedUser = result.ToDataModel();
                AppEnvironment.Instance.CurrentUser = updatedUser;
                return updatedUser;
            }
            catch (Exception e)
            {
                throw new ServiceException("UpdateUserProfilePhoto error", e);
            }
        }

        /// <summary>
        /// Uploads the photo.
        /// </summary>
        /// <param name="stream">The memory stream.</param>
        /// <param name="localPath">The local path.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="categoryId">The id of the assocaited category.</param>
        /// <returns>The uploaded photo.</returns>
        public async Task<Photo> UploadPhoto(Stream stream, string localPath, string caption, string categoryId)
        {
            try
            {
                var sasContracts = await GetSasUrls();

                // Upload photos into azure
                foreach (var sasContract in sasContracts)
                {
                    var blob =
                        new CloudBlockBlob(
                            new Uri($"{sasContract.FullBlobUri}{sasContract.SasToken}"));

                    var sideLength = sasContract.SasPhotoType.ToSideLength();

                    var resizedStream = await BitmapTools.Resize(
                        stream.AsRandomAccessStream(), sideLength,
                        sideLength);

                    await blob.UploadFromStreamAsync(resizedStream);
                }

                var photoContract = new PhotoContract
                {
                    CategoryId = categoryId,
                    Description = caption,
                    OSPlatform = AnalyticsInfo.VersionInfo.DeviceFamily,
                    User = AppEnvironment.Instance.CurrentUser.ToDataContract(),
                    ThumbnailUrl =
                        sasContracts.FirstOrDefault(c => c.SasPhotoType == PhotoTypeContract.Thumbnail)?
                            .FullBlobUri.ToString(),
                    StandardUrl =
                        sasContracts.FirstOrDefault(c => c.SasPhotoType == PhotoTypeContract.Standard)?
                            .FullBlobUri.ToString(),
                    HighResolutionUrl =
                        sasContracts.FirstOrDefault(c => c.SasPhotoType == PhotoTypeContract.HighRes)?
                            .FullBlobUri.ToString()
                };

                var responsePhotoContract =
                    await _mobileServiceClient.InvokeApiAsync<PhotoContract, PhotoContract>("Photo",
                        photoContract,
                        HttpMethod.Post, 
                        null);

                return responsePhotoContract.ToDataModel();
            }
            catch (Exception e)
            {
                throw new ServiceException("UploadPhoto error", e);
            }
        }
    }
}