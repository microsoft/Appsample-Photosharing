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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;

namespace PhotoSharingApp.Universal.Tests.Services
{
    [TestClass]
    public class ServiceLoadTests
    {
        private static readonly TimeSpan CacheExpirationTimeSpan = TimeSpan.FromSeconds(6);
        private const int NumberOfRequests = 100;

        private const int NumberOfTestIterations = 3;
        private Config _configuration;
        private ServiceClient _serviceClient;

        [TestInitialize]
        public void Init()
        {
            _serviceClient = new ServiceClient(new AuthenticationHandler());
            _configuration = new DefaultConfig();
        }

        [TestMethod]
        public async Task CombinedQueriesLoadTest()
        {
            // Arrange
            var topCategories =
                await _serviceClient.GetTopCategories(_configuration.CategoryThumbnailsLargeFormFactor, null);

            if (topCategories.Any())
            {
                var categoryWithMostPhotos = topCategories.OrderByDescending(c => c.PhotoThumbnails.Count).First();

                for (var j = 0; j < NumberOfTestIterations; j++)
                {
                    for (var i = 0; i < NumberOfRequests; i++)
                    {
                        // Act
                        var categories =
                            await
                                _serviceClient.GetTopCategories(_configuration.CategoryThumbnailsLargeFormFactor, null);

                        // Verify
                        Assert.IsNotNull(categories, "GetTopCategories returned null");

                        // Act
                        var photos = await _serviceClient.GetPhotosForCategoryId(categoryWithMostPhotos.Id);

                        // Verify
                        Assert.IsNotNull(photos, "GetPhotosForCategoryId returned null");

                        // Act
                        var leaderboard = await _serviceClient.GetLeaderboardData(5, 5, 5, 5);

                        // Verify
                        Assert.IsNotNull(leaderboard, "GetLeaderboardData returned null");
                    }

                    await Task.Delay(CacheExpirationTimeSpan);
                }
            }
        }

        [TestMethod]
        public async Task GetLeaderboardLoadTest()
        {
            for (var j = 0; j < NumberOfTestIterations; j++)
            {
                for (var i = 0; i < NumberOfRequests; i++)
                {
                    // Act
                    var leaderboard = await _serviceClient.GetLeaderboardData(5, 5, 5, 5);

                    // Verify
                    Assert.IsNotNull(leaderboard, "GetLeaderboardData returned null");
                }

                await Task.Delay(CacheExpirationTimeSpan);
            }
        }

        [TestMethod]
        public async Task GetPhotoStreamLoadTest()
        {
            var topCategories = await _serviceClient.GetTopCategories(100, null);

            if (topCategories.Any())
            {
                var categoryWithMostPhotos = topCategories.OrderByDescending(c => c.PhotoThumbnails.Count).First();

                for (var j = 0; j < NumberOfTestIterations; j++)
                {
                    for (var i = 0; i < NumberOfRequests; i++)
                    {
                        // Act
                        var photos = await _serviceClient.GetPhotosForCategoryId(categoryWithMostPhotos.Id);

                        // Verify
                        Assert.IsNotNull(photos, "GetPhotosForCategoryId returned null");
                    }

                    await Task.Delay(CacheExpirationTimeSpan);
                }
            }
        }

        [TestMethod]
        public async Task GetTopCategoriesBothFormFactorsLoadTest()
        {
            for (var j = 0; j < NumberOfTestIterations; j++)
            {
                for (var i = 0; i < NumberOfRequests; i++)
                {
                    // Act
                    var categoriesLargeFormFactor =
                        await _serviceClient.GetTopCategories(_configuration.CategoryThumbnailsLargeFormFactor, null);
                    var categoriesSmallFormFactor =
                        await _serviceClient.GetTopCategories(_configuration.CategoryThumbnailsSmallFormFactor, null);

                    // Verify
                    Assert.IsNotNull(categoriesLargeFormFactor, "categoriesLargeFormFactor null");
                    Assert.IsNotNull(categoriesSmallFormFactor, "categoriesSmallFormFactor null");
                }

                await Task.Delay(CacheExpirationTimeSpan);
            }
        }

        [TestMethod]
        public async Task GetTopCategoriesLoadTest()
        {
            for (var j = 0; j < NumberOfTestIterations; j++)
            {
                for (var i = 0; i < NumberOfRequests; i++)
                {
                    // Act
                    var categories =
                        await _serviceClient.GetTopCategories(_configuration.CategoryThumbnailsLargeFormFactor, null);

                    // Verify
                    Assert.IsNotNull(categories, "GetTopCategories returned null");
                }

                await Task.Delay(CacheExpirationTimeSpan);
            }
        }
    }
}