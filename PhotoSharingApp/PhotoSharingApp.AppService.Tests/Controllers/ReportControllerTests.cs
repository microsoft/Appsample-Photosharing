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
    public class ReportControllerTests : BaseTest
    {
        private ReportController _reportController;
        private IRepository _repository;
        private UserRegistrationReferenceProviderMock _userRegistrationReferenceProviderMock;

        [TestInitialize]
        public void Init()
        {
            _repository = new DocumentDbRepository(new TestEnvironmentDefinition());
            _userRegistrationReferenceProviderMock = new UserRegistrationReferenceProviderMock();

            _reportController = new ReportController(_repository, new TelemetryClient(),
                _userRegistrationReferenceProviderMock);
        }

        [TestMethod]
        public async Task InsertReportForAnnotationTest()
        {
            // Populate our db with necessary objects
            var user1 = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);
            var user2 = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);

            var category = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);

            var photo = await _repository.InsertPhoto(CreateTestPhoto(category, user1), 1);

            var annotation = await _repository.InsertAnnotation(CreateTestAnnotation(user2, 1, photo.Id,
                photo.User.UserId, "Wow this photo sucks, what a loser!"));

            _userRegistrationReferenceProviderMock.GetCurrentUserRegistrationReference =
                () => user1.RegistrationReference;

            var report = CreateTestReport(annotation.Id, ContentType.Annotation,
                ReportReason.Harassment, user1.UserId);

            // Act
            var actionResult = await _reportController.PostAsync(report);

            // Verify
            Assert.IsNotNull(actionResult, "null response from service");
            Assert.IsNotNull(actionResult.Id, "null id from service");
        }

        [TestMethod]
        public async Task InsertReportForPhotoTest()
        {
            // Populate our db with necessary objects
            var user1 = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);
            var user2 = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);

            var category = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);
            var photo = await _repository.InsertPhoto(CreateTestPhoto(category, user2), 1);

            _userRegistrationReferenceProviderMock.GetCurrentUserRegistrationReference =
                () => user1.RegistrationReference;

            var report = CreateTestReport(photo.Id, ContentType.Photo,
                ReportReason.Spam, user1.UserId);

            // Act
            var actionResult = await _reportController.PostAsync(report);

            // Verify
            Assert.IsNotNull(actionResult, "null response from service");
            Assert.IsNotNull(actionResult.Id, "null id from service");
        }
    }
}