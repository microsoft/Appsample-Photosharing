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
using PhotoSharingApp.AppService.Shared.Repositories;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Tests.DAL
{
    /// <summary>
    /// A mock class for <see cref="IRepository" />.
    /// <remarks>
    /// Only members that are being used for unit tests are implemented.
    /// If members need to be mocked which are not implemented yet,
    /// corresponding function delegates need to be created.
    /// Alternatively to this class, a mocking framework can be used
    /// to create an <see cref="IRepository"/> mock.
    /// </remarks>
    /// </summary>
    public class RepositoryMock : IRepository
    {
        public Func<IList<CategoryPreviewContract>> GetCategoriesPreviewFunc { get; set; }
        public Func<LeaderboardContract> GetLeaderboardFunc { get; set; }

        public Task<CategoryContract> CreateCategory(string name)
        {
            throw new NotImplementedException();
        }

        public Task<UserContract> CreateUser(string registrationReference)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAnnotation(string annotationId, string userRegistrationReference)
        {
            throw new NotImplementedException();
        }

        public Task DeletePhoto(string photoId, string userRegistrationReference)
        {
            throw new NotImplementedException();
        }

        void IRepository.Dispose()
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<IList<CategoryContract>> GetCategories()
        {
            throw new NotImplementedException();
        }

        public Task<IList<CategoryPreviewContract>> GetCategoriesPreview(int numberOfThumbnails)
        {
            return Task.FromResult(GetCategoriesPreviewFunc());
        }

        public Task<PagedResponse<PhotoContract>> GetCategoryPhotoStream(string categoryId, string continuationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<PhotoContract>> GetHeroPhotos(int count, int daysOld)
        {
            throw new NotImplementedException();
        }

        public Task<LeaderboardContract> GetLeaderboard(int mostGoldCategoriesCount, int mostGoldPhotosCount,
            int mostGoldUsersCount,
            int mostGivingUsersCount)
        {
            return Task.FromResult(GetLeaderboardFunc());
        }

        public Task<PhotoContract> GetPhoto(string id)
        {
            throw new NotImplementedException();
        }

        public Task<UserContract> GetUser(string userId, string registrationReference = null)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResponse<PhotoContract>> GetUserPhotoStream(string userId, string continuationToken, bool includeNonActivePhotos = false)
        {
            throw new NotImplementedException();
        }

        public Task InitializeDatabaseIfNotExisting(string serverPath)
        {
            throw new NotImplementedException();
        }

        public Task<AnnotationContract> InsertAnnotation(AnnotationContract annotation)
        {
            throw new NotImplementedException();
        }

        public Task<UserContract> InsertIapPurchase(IapPurchaseContract validatedIapReciept)
        {
            throw new NotImplementedException();
        }

        public Task<PhotoContract> InsertPhoto(PhotoContract photo, int goldIncrement)
        {
            throw new NotImplementedException();
        }

        public Task<ReportContract> InsertReport(ReportContract report, string userRegistrationReference)
        {
            throw new NotImplementedException();
        }

        public Task ReinitializeDatabase(string serverPath)
        {
            throw new NotImplementedException();
        }

        public Task<PhotoContract> UpdatePhoto(PhotoContract photoContract)
        {
            throw new NotImplementedException();
        }

        public Task<PhotoContract> UpdatePhotoStatus(PhotoContract photoContract)
        {
            throw new NotImplementedException();
        }

        public Task<UserContract> UpdateUser(UserContract user)
        {
            throw new NotImplementedException();
        }
    }
}