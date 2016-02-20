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
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ApplicationInsights;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoSharingApp.AppService.Controllers;
using PhotoSharingApp.AppService.Shared.Repositories;
using PhotoSharingApp.AppService.Shared.Validation;
using PhotoSharingApp.AppService.Tests.Context;
using PhotoSharingApp.AppService.Tests.Helpers;

namespace PhotoSharingApp.AppService.Tests.Controllers
{
    [TestClass]
    public class PhotoControllerTests : BaseTest
    {
        private PhotoController _photoController;
        private IRepository _repository;
        private UserRegistrationReferenceProviderMock _userRegistrationReferenceProviderMock;

        [TestInitialize]
        public void Init()
        {
            _repository = new DocumentDbRepository(new TestEnvironmentDefinition());
            _userRegistrationReferenceProviderMock = new UserRegistrationReferenceProviderMock();

            var photoValidation = new PhotoValidation(_repository);
            _photoController = new PhotoController(
                _repository,
                new TelemetryClient(),
                photoValidation,
                _userRegistrationReferenceProviderMock);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public async Task DeletePhotoTest()
        {
            // Populate our db with necessary objects
            var user = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);

            var category = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);

            var photo = await _repository.InsertPhoto(CreateTestPhoto(category, user), 1);

            _userRegistrationReferenceProviderMock.GetCurrentUserRegistrationReference =
                () => user.RegistrationReference;

            await _photoController.DeleteAsync(photo.Id);

            // Act
            await _photoController.GetAsync(photo.Id);
        }

        [TestMethod]
        public async Task GetPhotoTest()
        {
            // Populate our db with necessary objects
            var user = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);

            var category = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);

            var photo = await _repository.InsertPhoto(CreateTestPhoto(category, user), 1);

            _userRegistrationReferenceProviderMock.GetCurrentUserRegistrationReference =
                () => user.RegistrationReference;

            // Act
            var actionResult = await _photoController.GetAsync(photo.Id);

            // Verify        
            Assert.IsNotNull(actionResult, "null response from service");
            Assert.IsNotNull(actionResult.Id, "null id from service");
            Assert.IsTrue(actionResult.Id == photo.Id, "photoId doesn't match from existing photo");
            Assert.AreEqual(photo.CategoryId, actionResult.CategoryId, "categoryId doesn't match from existing photo");
            Assert.AreEqual(photo.CategoryName, actionResult.CategoryName, "categoryName doesn't match from existing photo");
            Assert.AreEqual(user.UserId, actionResult.User.UserId, "user doesn't match from existing photo");
            Assert.AreEqual(photo.Description, actionResult.Description, "description doesn't match from existing photo");
            Assert.AreEqual(photo.OSPlatform, actionResult.OSPlatform, "osPlatform doesn't match from existing photo");
        }

        [TestMethod]
        public async Task InsertPhotoTest()
        {
            // Populate our db with necessary objects
            var user = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);

            var category = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);

            var photo = CreateTestPhoto(category, user);

            _userRegistrationReferenceProviderMock.GetCurrentUserRegistrationReference =
                () => user.RegistrationReference;

            // Act
            var actionResult = await _photoController.PostAsync(photo);

            // Verify
            Assert.IsNotNull(actionResult, "null response from service");
            Assert.IsNotNull(actionResult.Id, "null id from service");
            Assert.AreEqual(photo.CategoryId, actionResult.CategoryId, "categoryId doesn't match from posted photo");
            Assert.AreEqual(photo.CategoryName, actionResult.CategoryName, "categoryName doesn't match from posted photo");
            Assert.AreEqual(user.UserId, actionResult.User.UserId, "user doesn't match from posted photo");
            Assert.AreEqual(photo.Description, actionResult.Description, "description doesn't match from posted photo");
            Assert.AreEqual(photo.OSPlatform, actionResult.OSPlatform, "osPlatform doesn't match from posted photo");
        }

        [TestMethod]
        public async Task UpdatePhotoTest()
        {
            // Populate our db with necessary objects
            var user = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);

            var category = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);

            var photo = await _repository.InsertPhoto(CreateTestPhoto(category, user), 1);

            _userRegistrationReferenceProviderMock.GetCurrentUserRegistrationReference =
                () => user.RegistrationReference;

            // Update contents of the new photo
            const string updatedTestDescription = "Updated Test Description";
            photo.Description = updatedTestDescription;

            // Act
            var actionResult = await _photoController.PutAsync(photo);

            // Instead of returning what we put in, get the data from the db to ensure it has been updated
            var photoUpdated = await _photoController.GetAsync(actionResult.Id);

            // Verify
            Assert.IsNotNull(actionResult, "null response from service");
            Assert.IsNotNull(actionResult.Id, "null id from service");
            Assert.IsTrue(actionResult.Id == photo.Id, "id from service doesn't match id from existing photo");
            Assert.IsTrue(photoUpdated.Description == updatedTestDescription, "description was not updated properly");
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public async Task UpdateWithBadCategoryPhotoTest()
        {
            // Populate our db with necessary objects
            var user = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);

            var category = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);

            var photo = await _repository.InsertPhoto(CreateTestPhoto(category, user), 1);

            _userRegistrationReferenceProviderMock.GetCurrentUserRegistrationReference =
                () => user.RegistrationReference;

            // Update contents of the new photo
            const string updatedTestDescription = "Updated Test Description";
            photo.Description = updatedTestDescription;
            photo.CategoryId = "00000000-0000-0000-0000-000000000001";

            // Act - we expect it to fail here
            await _photoController.PutAsync(photo);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public async Task UpdateWithBadPhotoIdPhotoTest()
        {
            // Populate our db with necessary objects
            var user = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);

            var category = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);

            var photo = await _repository.InsertPhoto(CreateTestPhoto(category, user), 1);

            _userRegistrationReferenceProviderMock.GetCurrentUserRegistrationReference =
                () => user.RegistrationReference;

            // Update contents of the new photo
            const string updatedTestDescription = "Updated Test Description";
            photo.Description = updatedTestDescription;
            photo.Id = "00000000-0000-0000-0000-000000000001";

            // Act - we expect it to fail here
            await _photoController.PutAsync(photo);
        }
    }
}