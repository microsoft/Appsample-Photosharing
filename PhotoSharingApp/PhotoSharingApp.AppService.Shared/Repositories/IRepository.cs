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
using System.Threading.Tasks;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Shared.Repositories
{
    /// <summary>
    /// Interface to the data access layer.
    /// </summary>
    public interface IRepository : IDisposable
    {
        /// <summary>
        /// Creates a new category with the provided name.
        /// </summary>
        /// <param name="name">The category name to be created.</param>
        /// <returns>The created category.</returns>
        Task<CategoryContract> CreateCategory(string name);

        /// <summary>
        /// Inserts a new user record in the database.
        /// </summary>
        /// <param name="registrationReference">The Azure Mobile Service user id.</param>
        /// <returns>Updated user object.</returns>
        Task<UserContract> CreateUser(string registrationReference);

        /// <summary>
        /// Deletes an annotation.
        /// </summary>
        /// <param name="annotationId">Id of annotation to be deleted.</param>
        /// <param name="userRegistrationReference">userRegistrationReference of annotation to be deleted.</param>
        Task DeleteAnnotation(string annotationId, string userRegistrationReference);

        /// <summary>
        /// Deletes all the data for the provided photo id.
        /// </summary>
        /// <param name="photoId">Id of the photo to be deleted.</param>
        /// <param name="userRegistrationReference">Azure Mobile Service user id.</param>
        Task DeletePhoto(string photoId, string userRegistrationReference);

        /// <summary>
        /// Releases resources.
        /// </summary>
        new void Dispose();

        /// <summary>
        /// Fetches all the categories and sorts them by name.
        /// </summary>
        /// <returns>List interface of CategoryContract sorted by name.</returns>
        Task<IList<CategoryContract>> GetCategories();

        /// <summary>
        /// Retrieves all the categories that have atleast one photo and also retrieves
        /// number of provided thumbnails for the photos in each category.
        /// </summary>
        /// <param name="numberOfThumbnails">Max number of thumbnails per category.</param>
        /// <returns>List interface of CategoryPreviewContract.</returns>
        Task<IList<CategoryPreviewContract>> GetCategoriesPreview(int numberOfThumbnails);

        /// <summary>
        /// Fetches the photo stream data for a provided category.
        /// </summary>
        /// <param name="categoryId">THe category id.</param>
        /// <param name="continuationToken">Last captured ticks in the form of a string.</param>
        /// <returns>List of photos up to the page size.</returns>
        Task<PagedResponse<PhotoContract>> GetCategoryPhotoStream(string categoryId, string continuationToken);

        /// <summary>
        /// Gets hero photos.
        /// </summary>
        /// <param name="count">The number of hero photos.</param>
        /// <param name="daysOld">The number of days old the photos can be.</param>
        /// <returns>List interface of hero photos.</returns>
        Task<IList<PhotoContract>> GetHeroPhotos(int count, int daysOld);

        /// <summary>
        /// Gets the leaderboard data.
        /// </summary>
        /// <param name="mostGoldCategoriesCount">Count of categories.</param>
        /// <param name="mostGoldPhotosCount">Count of photos.</param>
        /// <param name="mostGoldUsersCount">Count of wealthiest users.</param>
        /// <param name="mostGivingUsersCount">Count of most giving users.</param>
        /// <returns>The leaderboard data.</returns>
        Task<LeaderboardContract> GetLeaderboard(int mostGoldCategoriesCount, int mostGoldPhotosCount,
            int mostGoldUsersCount, int mostGivingUsersCount);

        /// <summary>
        /// Gets the photo data for provided photo id.
        /// </summary>
        /// <param name="id">The photo id.</param>
        /// <returns>The requested photo.</returns>
        Task<PhotoContract> GetPhoto(string id);

        /// <summary>
        /// Gets the user by an existing app user id OR registrationReference
        /// from Azure App Services auth mechanism as the userId may not be known
        /// at time of entry.
        /// </summary>
        /// <param name="userId">The app user id.</param>
        /// <param name="registrationReference">[Optional] The Azure App Service user id. Default value is null.</param>
        /// <returns>The User contract.</returns>
        Task<UserContract> GetUser(string userId, string registrationReference = null);

        /// <summary>
        /// Fetches the photo stream data for a specified user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="continuationToken">Last captured ticks in the form of a string.</param>
        /// <param name="includeNonActivePhotos">By default, false. If true, non-active photos are included.</param>
        /// <returns>List of photos up to the page size.</returns>
        Task<PagedResponse<PhotoContract>> GetUserPhotoStream(string userId, string continuationToken, bool includeNonActivePhotos = false);

        /// <summary>
        /// Checks if the database exists and initializes it if it doesn't.
        /// </summary>
        Task InitializeDatabaseIfNotExisting(string serverPath);

        /// <summary>
        /// Inserts the annotation object and performs the required gold transactions.
        /// </summary>
        /// <param name="annotation">Annotation to be inserted.</param>
        /// <returns>AnnotationContract.</returns>
        Task<AnnotationContract> InsertAnnotation(AnnotationContract annotation);

        /// <summary>
        /// Inserts receipt and adds gold to user.
        /// </summary>
        /// <param name="validatedIapReciept">Validated receipt values.</param>
        /// <returns>User object containing new gold balance.</returns>
        Task<UserContract> InsertIapPurchase(IapPurchaseContract validatedIapReciept);

        /// <summary>
        /// Insert the photo object into storage.
        /// </summary>
        /// <param name="photo">Photo Object.</param>
        /// <param name="goldIncrement">Gold to award for new photo.</param>
        /// <returns>New PhotoContract with updated user balance.</returns>
        Task<PhotoContract> InsertPhoto(PhotoContract photo, int goldIncrement);

        /// <summary>
        /// Inserts a report into Report table.
        /// </summary>
        /// <param name="report">The report being inserted.</param>
        /// <param name="userRegistrationReference">Azure Mobile Service user id who is reporting it.</param>
        /// <returns>The inserted Report.</returns>
        Task<ReportContract> InsertReport(ReportContract report, string userRegistrationReference);

        /// <summary>
        /// Forces a reinitalization of the database, deleting any existing data.
        /// </summary>
        Task ReinitializeDatabase(string serverPath);

        /// <summary>
        /// Updates an existing photo object's category and description fields.
        /// </summary>
        /// <param name="photoContract">Photo object.</param>
        /// <returns>PhotoContract containing updated data.</returns>
        Task<PhotoContract> UpdatePhoto(PhotoContract photoContract);

        /// <summary>
        /// Updates the status of the stored photo.
        /// </summary>
        /// <param name="photoContract">Photo object.</param>
        /// <returns>PhotoContract containing updated data.</returns>
        Task<PhotoContract> UpdatePhotoStatus(PhotoContract photoContract);

        /// <summary>
        /// Updates the user profile picture.  User gold balance is also updated if it is the first time
        /// the user is updating their profile picture.
        /// </summary>
        /// <param name="user">We need the whole user object as it is assumed the client will have given the photoId and url.</param>
        /// <returns>A new UserContract object.</returns>
        Task<UserContract> UpdateUser(UserContract user);
    }
}