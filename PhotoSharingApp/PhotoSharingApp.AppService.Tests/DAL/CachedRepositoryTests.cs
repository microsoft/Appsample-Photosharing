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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoSharingApp.AppService.Shared.Caching;
using PhotoSharingApp.AppService.Shared.Repositories;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Tests.DAL
{
    [TestClass]
    public class CachedRepositoryTests
    {
        private CachedRepository _cachedRepository;
        private MemoryCacheService _memoryCacheService;
        private RepositoryMock _repositoryMock;

        [TestInitialize]
        public void Init()
        {
            _memoryCacheService = new MemoryCacheService { ExpirationTimeSpan = TimeSpan.FromSeconds(1) };
            _memoryCacheService.Clear();

            _repositoryMock = new RepositoryMock();
            _cachedRepository = new CachedRepository(_repositoryMock,
                _memoryCacheService);
        }

        [TestMethod]
        public async Task GetCategoriesPreviewRepositoryHitCountTest()
        {
            // Arrange
            var hitCount = 0;
            _repositoryMock.GetCategoriesPreviewFunc = () =>
            {
                hitCount++;

                return new List<CategoryPreviewContract>
                {
                    new CategoryPreviewContract()
                };
            };

            // Act
            for (var i = 0; i < 5; i++)
            {
                await _cachedRepository.GetCategoriesPreview(10);
            }

            // Verify that we hit the repository only once.
            Assert.AreEqual(1, hitCount);

            await Task.Delay(_memoryCacheService.ExpirationTimeSpan + TimeSpan.FromSeconds(1));

            // Act
            for (var i = 0; i < 5; i++)
            {
                await _cachedRepository.GetCategoriesPreview(10);
            }

            // Verify that we hit the repository twice.
            Assert.AreEqual(2, hitCount);
        }

        [TestMethod]
        public async Task GetCategoriesPreviewWithDiffThumbnailCountRepositoryHitCountTest()
        {
            // Arrange
            var hitCount = 0;
            _repositoryMock.GetCategoriesPreviewFunc = () =>
            {
                hitCount++;

                return new List<CategoryPreviewContract>
                {
                    new CategoryPreviewContract()
                };
            };

            // Act
            for (var i = 0; i < 5; i++)
            {
                await _cachedRepository.GetCategoriesPreview(10);
                await _cachedRepository.GetCategoriesPreview(6);
            }

            // Verify that we hit the repository twice.
            Assert.AreEqual(2, hitCount);

            await Task.Delay(_memoryCacheService.ExpirationTimeSpan + TimeSpan.FromSeconds(1));

            // Act
            for (var i = 0; i < 5; i++)
            {
                await _cachedRepository.GetCategoriesPreview(10);
                await _cachedRepository.GetCategoriesPreview(6);
            }

            // Verify that we hit the repository twice.
            Assert.AreEqual(4, hitCount);
        }

        [TestMethod]
        public async Task GetLeaderboardRepositoryHitCountTest()
        {
            // Arrange
            var hitCount = 0;
            _repositoryMock.GetLeaderboardFunc = () =>
            {
                hitCount++;

                return new LeaderboardContract();
            };

            // Act
            for (var i = 0; i < 5; i++)
            {
                await _cachedRepository.GetLeaderboard(5, 5, 5, 5);
            }

            // Verify that we hit the repository only once.
            Assert.AreEqual(1, hitCount);

            await Task.Delay(_memoryCacheService.ExpirationTimeSpan + TimeSpan.FromSeconds(1));

            // Act
            for (var i = 0; i < 5; i++)
            {
                await _cachedRepository.GetLeaderboard(5, 5, 5, 5);
            }

            // Verify that we hit the repository twice.
            Assert.AreEqual(2, hitCount);
        }
    }
}