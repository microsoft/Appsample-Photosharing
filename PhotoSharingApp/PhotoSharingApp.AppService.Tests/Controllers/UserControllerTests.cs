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
using PhotoSharingApp.AppService.Tests.Helpers;
using PhotoSharingApp.AppService.Tests.Context;

namespace PhotoSharingApp.AppService.Tests.Controllers
{
    [TestClass]
    public class UserControllerTests : BaseTest
    {
        private IRepository _repository;
        private UserController _userController;
        private UserRegistrationReferenceProviderMock _userRegistrationReferenceProviderMock;

        [TestInitialize]
        public void Init()
        {
            _repository = new DocumentDbRepository(new TestEnvironmentDefinition());
            _userRegistrationReferenceProviderMock = new UserRegistrationReferenceProviderMock();

            _userController = new UserController(_repository, new TelemetryClient(),
                _userRegistrationReferenceProviderMock);
        }

        [TestMethod]
        public async Task GetExistingUserTest()
        {
            // Populate our db with necessary objects
            var user = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);

            _userRegistrationReferenceProviderMock.GetCurrentUserRegistrationReference =
                () => user.RegistrationReference;

            // Act
            var actionResult = await _userController.GetUser();

            // Verify
            Assert.IsNotNull(actionResult, "null response from service");
            Assert.IsNotNull(actionResult.UserId, "null id from service");
            Assert.AreEqual(user.RegistrationReference, actionResult.RegistrationReference,
                "registration reference from service doesn't match registration reference from existing user");
            Assert.AreEqual(user.UserId, actionResult.UserId, "id from service doesn't match id from existing user");
        }

        [TestMethod]
        public async Task GetNonExistingUserAndCreateNewUserTest()
        {
            // Populate our db with necessary objects
            var registrationReference = "Test User " + DateTime.UtcNow.Ticks;

            _userRegistrationReferenceProviderMock.GetCurrentUserRegistrationReference =
                () => registrationReference;

            // Act
            var actionResult = await _userController.GetUser();

            // Verify
            Assert.IsNotNull(actionResult, "null response from service");
            Assert.IsNotNull(actionResult.UserId, "null id from service");
            Assert.AreEqual(registrationReference, actionResult.RegistrationReference,
                "registration reference from service doesn't match registration reference from existing user");
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public async Task UpdateUserAsDifferentUserTest()
        {
            // Populate our db with necessary objects
            var user1 = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);
            var user2 = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);

            _userRegistrationReferenceProviderMock.GetCurrentUserRegistrationReference =
                () => user2.RegistrationReference;

            user1.ProfilePhotoUrl = "http://northwind-traders.com";

            // Act
            await _userController.UpdateUserProfile(user1);
        }

        [TestMethod]
        public async Task UpdateUserTest()
        {
            // Populate our db with necessary objects
            var user = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);

            _userRegistrationReferenceProviderMock.GetCurrentUserRegistrationReference =
                () => user.RegistrationReference;

            user.ProfilePhotoUrl = "http://northwind-traders.com";

            // Act
            var actionResult = await _userController.UpdateUserProfile(user);

            // Verify
            Assert.IsNotNull(actionResult, "null response from service");
            Assert.AreEqual(user.ProfilePhotoUrl, actionResult.ProfilePhotoUrl,
                "profile photo url wasn't updated from UpdateUserProfile service call");

            // Act II
            actionResult = await _userController.GetUser();

            // Verify
            Assert.IsNotNull(actionResult, "null response from service");
            Assert.AreEqual(user.ProfilePhotoUrl, actionResult.ProfilePhotoUrl,
                "profile photo url update wasn't persisted after UpdateUserProfile service call");
        }
    }
}