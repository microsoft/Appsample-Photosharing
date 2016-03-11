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

using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoSharingApp.AppService.Controllers;
using PhotoSharingApp.AppService.Shared.Repositories;
using PhotoSharingApp.AppService.Tests.Context;
using PhotoSharingApp.AppService.Tests.Helpers;

namespace PhotoSharingApp.AppService.Tests.Controllers
{
    [TestClass]
    class HeroPhotoControllerTests : BaseTest
    {
        private HeroPhotoController _heroPhotoController;
        private IRepository _repository;
        private UserRegistrationReferenceProviderMock _userRegistrationReferenceProviderMock;

        [TestInitialize]
        public void Init()
        {
            _repository = new DocumentDbRepository(new TestEnvironmentDefinition());
            _userRegistrationReferenceProviderMock = new UserRegistrationReferenceProviderMock();

            _heroPhotoController = new HeroPhotoController(
                _repository,
                new TelemetryClient(),
                _userRegistrationReferenceProviderMock);
        }

        [TestMethod]
        public async Task GetHeroPhotosTest()
        {
            await _repository.ReinitializeDatabase(string.Empty);

            // Populate our db with necessary objects
            var user = await _repository.CreateUser("Test User " + System.DateTime.UtcNow.Ticks);

            var category1 = await _repository.CreateCategory("Test Category " + System.DateTime.UtcNow.Ticks);
            var category2 = await _repository.CreateCategory("Test Category " + System.DateTime.UtcNow.Ticks);
            var category3 = await _repository.CreateCategory("Test Category " + System.DateTime.UtcNow.Ticks);

            var photo1 = await _repository.InsertPhoto(CreateTestPhoto(category1, user, "Test photo 1", 7), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category2, user, "Test photo 2"), 1);
            var photo2 = await _repository.InsertPhoto(CreateTestPhoto(category3, user, "Test photo 3", 6), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category1, user, "Test photo 4"), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category2, user, "Test photo 5"), 1);
            var photo3 = await _repository.InsertPhoto(CreateTestPhoto(category3, user, "Test photo 6", 5), 1);

            // Act
            var actionResult = await _heroPhotoController.GetAsync(3);

            // Verify
            Assert.IsNotNull(actionResult, "null response from service");
            Assert.AreEqual(3, actionResult.Count, "got more photos than expected");
            Assert.IsTrue(actionResult.Any(p => p.Id == photo1.Id), "photo1 wasn't returned as a hero photo");
            Assert.IsTrue(actionResult.Any(p => p.Id == photo2.Id), "photo2 wasn't returned as a hero photo");
            Assert.IsTrue(actionResult.Any(p => p.Id == photo3.Id), "photo3 wasn't returned as a hero photo");
        }
    }
}