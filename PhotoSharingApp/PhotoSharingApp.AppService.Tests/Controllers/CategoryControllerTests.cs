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
using Microsoft.ApplicationInsights;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoSharingApp.AppService.Controllers;
using PhotoSharingApp.AppService.Shared.Repositories;
using PhotoSharingApp.AppService.Tests.Helpers;
using PhotoSharingApp.AppService.Tests.Context;

namespace PhotoSharingApp.AppService.Tests.Controllers
{
    [TestClass]
    public class CategoryControllerTests : BaseTest
    {
        private CategoryController _categoryController;
        private IRepository _repository;
        private UserRegistrationReferenceProviderMock _userRegistrationReferenceProviderMock;

        [TestInitialize]
        public void Init()
        {
            _repository = new DocumentDbRepository(new TestEnvironmentDefinition());
            _userRegistrationReferenceProviderMock = new UserRegistrationReferenceProviderMock();

            _categoryController = new CategoryController(
              _repository,
                new TelemetryClient(),
                _userRegistrationReferenceProviderMock);
        }

        [TestMethod]
        public async Task GetCategoriesTest()
        {
            await _repository.ReinitializeDatabase(string.Empty);

            // Populate our db with necessary objects
            await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);
            await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);
            await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);

            // Act
            var categories = await _categoryController.GetAsync();

            // Verify
            Assert.IsNotNull(categories, "null response from service");
            Assert.AreEqual(3, categories.Count, "returned category count was not correct");
        }

        [TestMethod]
        public async Task GetPhotoStreamWithNullContinuationTokenTest()
        {
            // Populate our db with necessary objects
            var user = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);

            var category = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);

            await _repository.InsertPhoto(CreateTestPhoto(category, user, "Test photo 1"), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category, user, "Test photo 2"), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category, user, "Test photo 3"), 1);

            // Act
            var photoStream = await _categoryController.GetAsync(category.Id, null);

            // Verify
            Assert.IsNotNull(photoStream, "null response from service");
            Assert.IsNotNull(photoStream.Items, "null Items response from service");
            Assert.AreEqual(3, photoStream.Items.Count, "photos count returned was not correct");
        }

        [TestMethod]
        public async Task GetPreviewCategoriesTest()
        {
            await _repository.ReinitializeDatabase(string.Empty);

            const int numberOfThumbnails = 10;

            // Populate our db with necessary objects
            var user = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);

            var category1 = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);
            var category2 = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);
            var category3 = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);

            await _repository.InsertPhoto(CreateTestPhoto(category1, user, "Test photo 1"), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category2, user, "Test photo 2"), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category3, user, "Test photo 3"), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category1, user, "Test photo 4"), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category3, user, "Test photo 5"), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category3, user, "Test photo 6"), 1);

            // Act
            var categories = await _categoryController.GetAsync(numberOfThumbnails);

            // Verify
            Assert.IsNotNull(categories, "null response from service");
            Assert.AreEqual(3, categories.Count, "returned category count was not correct");

            Assert.IsTrue(categories.Any(c => c.Id == category1.Id), "category1 was not returned");
            Assert.AreEqual(2, categories.FirstOrDefault(c => c.Id == category1.Id)?.PhotoThumbnails.Count,
                "category1's thumbnail count was not correct");

            Assert.IsTrue(categories.Any(c => c.Id == category2.Id), "category2 was not returned");
            Assert.AreEqual(1, categories.FirstOrDefault(c => c.Id == category2.Id)?.PhotoThumbnails.Count,
                "category2's thumbnail count was not correct");

            Assert.IsTrue(categories.Any(c => c.Id == category3.Id), "category3 was not returned");
            Assert.AreEqual(3, categories.FirstOrDefault(c => c.Id == category3.Id)?.PhotoThumbnails.Count,
                "category3's thumbnail count was not correct");
        }

        [TestMethod]
        public async Task GetPreviewCategoriesWithNoDataTest()
        {
            await _repository.ReinitializeDatabase(string.Empty);

            const int numberOfThumbnails = 10;

            // Populate our db with necessary objects
            // In this case, nothing.

            // Act
            var categories = await _categoryController.GetAsync(numberOfThumbnails);

            // Verify
            Assert.IsNotNull(categories, "null response from service");
            Assert.AreEqual(0, categories.Count, "returned category count was not correct");
        }
    }
}