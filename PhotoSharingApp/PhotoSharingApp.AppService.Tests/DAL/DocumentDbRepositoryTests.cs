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

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoSharingApp.AppService.Shared;
using PhotoSharingApp.AppService.Shared.Context;
using PhotoSharingApp.AppService.Shared.Repositories;
using PhotoSharingApp.AppService.Tests.Context;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Tests.DAL
{
    [TestClass]
    public class DocumentDbRepositoryTests : BaseTest
    {
        private readonly EnvironmentDefinitionBase _environmentDefinition = new TestEnvironmentDefinition();
        private DocumentDbRepository _repository;

        [TestInitialize]
        public void Init()
        {
            _repository = new DocumentDbRepository(_environmentDefinition);
        }

        [TestMethod]
        public void CheckIfDocumentDbCollectionExistsTest()
        {
            Assert.IsFalse(_environmentDefinition.DocumentDbStorage.AuthorizationKey.Contains("["), "Azure AuthorizationKey is not set!");
            Assert.IsFalse(string.IsNullOrEmpty(_environmentDefinition.DocumentDbStorage.DataBaseId), "DocumentDB DatabaseId is not set!");
            Assert.IsFalse(string.IsNullOrEmpty(_environmentDefinition.DocumentDbStorage.CollectionId), "DocumentDB CollectionId is not set!");

            var actionResult = _repository.CheckIfDatabaseAndCollectionExist();

            Assert.IsTrue(actionResult, "DocumentDB Database or Collection do not exist, tests will fail! Please configure your DocumentDB credentials in the proper EnvironmentDefinition.");
        }

        [TestMethod]
        public async Task CreateAndGetUserDocumentTest()
        {
            var registrationReference = "test user " + System.DateTime.UtcNow.Ticks;

            // Act
            var actionResult = await _repository.CreateUser(registrationReference);

            Assert.IsNotNull(actionResult, "null response from server");
            Assert.AreEqual(registrationReference, actionResult.RegistrationReference, "created user's registrationReference did not match original");
            Assert.AreEqual(_environmentDefinition.NewUserGoldBalance, actionResult.GoldBalance, "new user's gold balance is not set to proper value");

            var actionResult2 = await _repository.GetUser(actionResult.UserId, actionResult.RegistrationReference);

            // Verify
            Assert.IsNotNull(actionResult2, "null response from server for actionResult2");
            Assert.AreEqual(registrationReference, actionResult2.RegistrationReference, "retrieved user's registrationReference did not match original");
            Assert.AreEqual(_environmentDefinition.NewUserGoldBalance, actionResult.GoldBalance, "new user's gold balance is not set to proper value");
            Assert.IsNotNull(actionResult2.UserCreated, "userCreated did not get set");
            Assert.IsNotNull(actionResult2.UserModified, "userModified datetime did not get updated");
            Assert.IsNotNull(actionResult2.UserId, "userId did not get created or selected");
        }

        /// <summary>
        /// This tests that a duplicate receipt may not be reused for gold purchase
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(DataLayerException))]
        public async Task IapPurchaseDocumentFailTest()
        {
            // Populate our db with necessary objects
            var user = await _repository.CreateUser("test user " + System.DateTime.UtcNow.Ticks);

            var iapReceipt1 = CreateTestIapPurchase(user.UserId, 5, "Gold");

            var afterPurchaseUser1 = await _repository.InsertIapPurchase(iapReceipt1);

            Assert.AreEqual(user.GoldBalance + 5, afterPurchaseUser1.GoldBalance, "user from service did not have additional gold from iap");

            var iapReceipt2 = CreateTestIapPurchase(user.UserId, 5, "Gold");

            iapReceipt2.IapPurchaseId = iapReceipt1.IapPurchaseId;

            await _repository.InsertIapPurchase(iapReceipt2);
        }

        /// <summary>
        /// Creates a new purchase record and adds gold to the test user
        /// </summary>
        [TestMethod]
        public async Task IapPurchaseDocumentSuccessTest()
        {
            // Populate our db with necessary objects
            var user = await _repository.CreateUser("test user " + System.DateTime.UtcNow.Ticks);

            var iapReceipt = CreateTestIapPurchase(user.UserId, 5, "Gold");

            var afterPurchaseUser = await _repository.InsertIapPurchase(iapReceipt);

            Assert.AreEqual(user.GoldBalance + 5, afterPurchaseUser.GoldBalance, "user from service did not have additional gold from iap");
        }

        [TestMethod]
        public async Task InsertAndDeleteAnnotationTest()
        {
            // Populate our db with necessary objects
            var user1 = await _repository.CreateUser("test user " + System.DateTime.UtcNow.Ticks);
            var user2 = await _repository.CreateUser("test user " + System.DateTime.UtcNow.Ticks);

            var category1 = await _repository.CreateCategory("Test Category " + System.DateTime.UtcNow.Ticks);

            var insertedPhoto = await _repository.InsertPhoto(CreateTestPhoto(category1, user1), 1);

            var annotation = CreateTestAnnotation(user2, 1, insertedPhoto.Id, insertedPhoto.User.UserId);

            // Insert annotation to test photo
            var actionResult = await _repository.InsertAnnotation(annotation);
            var photo = await _repository.GetPhoto(insertedPhoto.Id);

            // Get inserted annotation from Photo
            var insertedAnnotation = photo.Annotations.Find(x => x.Id == actionResult.Id);

            // Verify annotation is inserted
            Assert.IsNotNull(insertedAnnotation, "repository returned null");

            // Delete annotation
            await _repository.DeleteAnnotation(actionResult.Id, user1.RegistrationReference);
            photo = await _repository.GetPhoto(insertedPhoto.Id);
            insertedAnnotation = photo.Annotations.Find(x => x.Id == actionResult.Id);

            // Verify annotation is deleted
            Assert.IsNull(insertedAnnotation, "repository returned null");
        }

        [TestMethod]
        [ExpectedException(typeof(DataLayerException))]
        public async Task InsertAndDeletePhotoTest()
        {
            // Populate our db with necessary objects
            var user = await _repository.CreateUser("test user " + System.DateTime.UtcNow.Ticks);

            var category = await _repository.CreateCategory("Test Category " + System.DateTime.UtcNow.Ticks);

            var photo = CreateTestPhoto(category, user);

            // Insert photo
            var actionResult = await _repository.InsertPhoto(photo, 1);

            // Verify
            Assert.IsNotNull(actionResult, "null response from service insert");
            Assert.IsNotNull(actionResult.Id, "null id from service");

            // Get photo
            var getPhotoResult = await _repository.GetPhoto(actionResult.Id);

            // Verify
            Assert.IsNotNull(getPhotoResult, "null response from service get");
            Assert.AreEqual(getPhotoResult.Id, actionResult.Id, "photoId does not match insert response");

            // Delete Photo
            await _repository.DeletePhoto(actionResult.Id, user.RegistrationReference);

            // Get photo - expect it to fail here
            await _repository.GetPhoto(actionResult.Id);
        }

        /// <summary>
        /// Files a report against an annotation
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ReportDocumentForAnnotationTest()
        {
            // Populate our db with necessary objects
            var user1 = await _repository.CreateUser("test user " + System.DateTime.UtcNow.Ticks);
            var user2 = await _repository.CreateUser("test user " + System.DateTime.UtcNow.Ticks);

            var category = await _repository.CreateCategory("Test Category " + System.DateTime.UtcNow.Ticks);

            var photo = await _repository.InsertPhoto(CreateTestPhoto(category, user1), 1);

            var annotation = await _repository.InsertAnnotation(CreateTestAnnotation(user2, 1, photo.Id, photo.User.UserId,
                        "Wow this photo sucks, what a loser!"));

            var report = CreateTestReport(annotation.Id, ContentType.Annotation, ReportReason.Harassment, user1.UserId);

            // Act
            var reportResult = await _repository.InsertReport(report, user1.RegistrationReference);

            // Verify
            Assert.IsNotNull(reportResult);
            Assert.IsNotNull(reportResult.Id);
            Assert.AreEqual(report.ContentId, reportResult.ContentId);
            Assert.AreEqual(report.ContentType, reportResult.ContentType);
            Assert.AreEqual(report.ReportReason, reportResult.ReportReason);
            Assert.IsTrue(reportResult.Active);
            Assert.IsNotNull(reportResult.CreatedDateTime);
        }

        /// <summary>
        /// Files a report against a photo
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ReportDocumentForPhotoTest()
        {
            // Populate our db with necessary objects
            var user1 = await _repository.CreateUser("test user " + System.DateTime.UtcNow.Ticks);
            var user2 = await _repository.CreateUser("test user " + System.DateTime.UtcNow.Ticks);

            var category = await _repository.CreateCategory("Test Category " + System.DateTime.UtcNow.Ticks);

            var photo = await _repository.InsertPhoto(CreateTestPhoto(category, user1), 1);

            var report = CreateTestReport(photo.Id, ContentType.Photo, ReportReason.Spam, user2.UserId);

            // Act
            var reportResult = await _repository.InsertReport(report, user2.RegistrationReference);

            // Verify
            Assert.IsNotNull(reportResult);
            Assert.IsNotNull(reportResult.Id);
            Assert.AreEqual(report.ContentId, reportResult.ContentId);
            Assert.AreEqual(report.ContentType, reportResult.ContentType);
            Assert.AreEqual(report.ReportReason, reportResult.ReportReason);
            Assert.IsTrue(reportResult.Active);
            Assert.IsNotNull(reportResult.CreatedDateTime);
        }
    }
}