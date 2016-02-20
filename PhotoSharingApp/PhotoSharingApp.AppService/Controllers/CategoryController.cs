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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    /// Controller to manage categories.
    /// </summary>
    [MobileAppController]
    public class CategoryController : BaseController
    {
        private readonly IRepository _repository;
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Category controller constructor.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="telemetryClient">The telemetry client.</param>
        /// <param name="userRegistrationReferenceProvider">The user registration reference provider.</param>
        public CategoryController(IRepository repository, TelemetryClient telemetryClient,
            IUserRegistrationReferenceProvider userRegistrationReferenceProvider)
            : base(userRegistrationReferenceProvider)
        {
            _repository = repository;
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Retrieves the photo stream for provided category
        /// </summary>
        /// <verb>GET</verb>
        /// <url>http://{host}/api/category/{id}?continuationToken={continuationToken}</url>
        /// <param name="id">Category Id for which photo stream needs to be retrieved.</param>
        /// <param name="continuationToken">continuation token to page through the result.</param>
        /// <returns>Paged list of photo contract.</returns>
        [Route("api/category/{id}")]
        public async Task<PagedResponse<PhotoContract>> GetAsync(string id, [FromUri] string continuationToken)
        {
            try
            {
                _telemetryClient.TrackEvent("CategoryController GetAsync(id, continuation) invoked");

                var stream = await _repository.GetCategoryPhotoStream(id, continuationToken);

                return stream;
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
        /// Retrieves all the categories that have atleast one photo and also retrieves number
        /// of provided thumbnails for the photos in each category.
        /// </summary>
        /// <verb>GET</verb>
        /// <url>http://{host}/api/category?numberOfThumbnails={numberOfThumbnails}</url>
        /// <param name="numberOfThumbnails">Number of thumbnails to be retrieved for each category.</param>
        /// <returns>List of CategoryPreviewContract</returns>
        [Route("api/category")]
        public async Task<IList<CategoryPreviewContract>> GetAsync([FromUri] int numberOfThumbnails)
        {
            try
            {
                _telemetryClient.TrackEvent("CategoryController GetAsync(numberOfThumbnails) invoked");
                var categoriesPreviewList = await _repository.GetCategoriesPreview(numberOfThumbnails);

                // Order by categories with the latest photo added.
                IList<CategoryPreviewContract> orderedCategoriesPreviewList =
                    categoriesPreviewList.OrderByDescending(x => x.PhotoThumbnails[0].CreatedAt).ToList();
                return orderedCategoriesPreviewList;
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
        /// Retrieves all the categories.
        /// </summary>
        /// <verb>GET</verb>
        /// <url>http://{host}/api/categories</url>
        /// <returns>List of CategoryContract.</returns>
        [Route("api/categories")]
        public async Task<IList<CategoryContract>> GetAsync()
        {
            try
            {
                _telemetryClient.TrackEvent("CategoryController GetAsync invoked");

                var categories = await _repository.GetCategories();

                return categories;
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
        /// Creates a category with the provided name.
        /// </summary>
        /// <verb>POST</verb>
        /// <url>http://{host}/api/category/{name}</url>
        /// <param name="name">Category name to be created.</param>
        /// <returns>Created category</returns>
        [Authorize]
        [Route("api/category/{name}")]
        public async Task<CategoryContract> PostAsync(string name)
        {
            try
            {
                _telemetryClient.TrackEvent("CategoryController PostAsync invoked");
                await ValidateAndReturnCurrentUserId();

                var sanitizedName = name.TrimAndReplaceMultipleWhitespaces().ToTitleCase();

                return await _repository.CreateCategory(sanitizedName);
            }
            catch (DataLayerException ex)
            {
                _telemetryClient.TrackException(ex);

                if (ex.Error == DataLayerError.Unknown)
                {
                    throw ServiceExceptions.UnknownInternalFailureException(ServiceExceptions.Source);
                }
                if (ex.Error == DataLayerError.DuplicateKeyInsert)
                {
                    throw ServiceExceptions.DuplicateCategoryException(ServiceExceptions.Source);
                }

                throw ServiceExceptions.DataLayerException(ex.Message);
            }
        }
    }
}