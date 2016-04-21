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
using PhotoSharingApp.AppService.Tests.Context;
using PhotoSharingApp.AppService.Tests.Helpers;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Tests.Controllers
{
    [TestClass]
    public class UserPhotoControllerTests : BaseTest
    {
        private IRepository _repository;
        private UserPhotoController _userPhotoController;
        private UserRegistrationReferenceProviderMock _userRegistrationReferenceProviderMock;

        [TestInitialize]
        public void Init()
        {
            _repository = new DocumentDbRepository(new TestEnvironmentDefinition());
            _userRegistrationReferenceProviderMock = new UserRegistrationReferenceProviderMock();

            _userPhotoController = new UserPhotoController(_repository, new TelemetryClient(),
                _userRegistrationReferenceProviderMock);
        }

        [TestMethod]
        public async Task GetOtherUserPhotoStreamTest()
        {
            // Populate our db with necessary objects
            var user = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);

            var category1 = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);
            var category2 = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);
            var category3 = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);

            await _repository.InsertPhoto(CreateTestPhoto(category1, user, "Test photo 1"), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category2, user, "Test photo 2"), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category3, user, "Test photo 3"), 1);

            // Insert objectionable photo
            var photo = CreateTestPhoto(category3, user, "Test photo 3");
            var insertedPhoto = await _repository.InsertPhoto(photo, 1);

            insertedPhoto.Status = PhotoStatus.ObjectionableContent;
            await _repository.UpdatePhotoStatus(insertedPhoto);

            // Act
            var actionResult = await _userPhotoController.GetPagedAsync(user.UserId, null);

            // Verify
            Assert.IsNotNull(actionResult.Items, "null response from service");
            Assert.AreEqual(3, actionResult.Items.Count, "photo count returned was not correct");
            Assert.IsNull(actionResult.Items.FirstOrDefault(p => p.Status == PhotoStatus.ObjectionableContent));
        }

        [TestMethod]
        public async Task GetUserPhotoStreamTest()
        {
            // Populate our db with necessary objects
            var user = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);

            var category1 = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);
            var category2 = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);
            var category3 = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);

            await _repository.InsertPhoto(CreateTestPhoto(category1, user, "Test photo 1"), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category2, user, "Test photo 2"), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category3, user, "Test photo 3"), 1);

            // Insert objectionable photo
            var photo = CreateTestPhoto(category3, user, "Test photo 3");
            var insertedPhoto = await _repository.InsertPhoto(photo, 1);

            insertedPhoto.Status = PhotoStatus.ObjectionableContent;
            await _repository.UpdatePhotoStatus(insertedPhoto);

            _userRegistrationReferenceProviderMock.GetCurrentUserRegistrationReference =
                () => user.RegistrationReference;

            // Act
            var actionResult = await _userPhotoController.GetPagedAsync(null);

            // Verify
            Assert.IsNotNull(actionResult.Items, "null response from service");
            Assert.AreEqual(4, actionResult.Items.Count, "photo count returned was not correct");
            Assert.IsNotNull(actionResult.Items.FirstOrDefault(p => p.Status == PhotoStatus.ObjectionableContent));
        }
    }
}