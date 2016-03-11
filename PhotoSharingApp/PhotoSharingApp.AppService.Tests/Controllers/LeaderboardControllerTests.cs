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

namespace PhotoSharingApp.AppService.Tests.Controllers
{
    [TestClass]
    public class LeaderboardControllerTests : BaseTest
    {
        private LeaderboardController _leaderboardController;
        private IRepository _repository;

        [TestInitialize]
        public void Init()
        {
            _repository = new DocumentDbRepository(new TestEnvironmentDefinition());
            _leaderboardController = new LeaderboardController(_repository, new TelemetryClient());
        }

        [TestMethod]
        public async Task GetLeaderboardTest()
        {
            await _repository.ReinitializeDatabase(string.Empty);

            const int categories = 3;
            const int photos = 3;
            const int wealthyUsers = 3;
            const int benevolentUsers = 3;

            // Populate our db with necessary objects
            var user = await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);
            await _repository.CreateUser("Test User " + DateTime.UtcNow.Ticks);

            var category1 = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);
            var category2 = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);
            var category3 = await _repository.CreateCategory("Test Category " + DateTime.UtcNow.Ticks);

            var photo1 = await _repository.InsertPhoto(CreateTestPhoto(category1, user, "Test photo 1", 7), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category2, user, "Test photo 2"), 1);
            var photo2 = await _repository.InsertPhoto(CreateTestPhoto(category3, user, "Test photo3", 6), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category1, user, "Test photo 4"), 1);
            await _repository.InsertPhoto(CreateTestPhoto(category2, user, "Test photo 5"), 1);
            var photo3 = await _repository.InsertPhoto(CreateTestPhoto(category3, user, "Test photo 6", 5), 1);

            // Act
            var leaderboard = await _leaderboardController.GetAsync(categories, photos, wealthyUsers, benevolentUsers);

            // Verify
            Assert.IsNotNull(leaderboard, "null response from service");

            Assert.IsTrue(leaderboard.MostGoldCategories.Count == categories,
                "leaderboard didn't return the correct category count");
            Assert.IsTrue(leaderboard.MostGoldPhotos.Count == photos,
                "leaderboard didn't return the correct photo count");
            Assert.IsTrue(leaderboard.MostGoldUsers.Count <= (wealthyUsers - 1),
                "leaderboard didn't return the correct user count");
            Assert.IsTrue(leaderboard.MostGivingUsers.Count <= (benevolentUsers - 1),
                "leaderboard didn't return the correct user count");

            Assert.IsTrue(leaderboard.MostGoldCategories[0].Model.Id == category3.Id,
                "leaderboard didn't have categories in correct order, category3 should be first");
            Assert.IsTrue(leaderboard.MostGoldCategories[1].Model.Id == category1.Id,
                "leaderboard didn't have categories in correct order, category1 should be second");
            Assert.IsTrue(leaderboard.MostGoldCategories[2].Model.Id == category2.Id,
                "leaderboard didn't have categories in correct order, category2 should be last");

            Assert.IsTrue(leaderboard.MostGoldPhotos[0].Model.Id == photo1.Id,
                "leaderboard didn't have photos in correct order, photo1 should be first");
            Assert.IsTrue(leaderboard.MostGoldPhotos[1].Model.Id == photo2.Id,
                "leaderboard didn't have photos in correct order, photo2 should be second");
            Assert.IsTrue(leaderboard.MostGoldPhotos[2].Model.Id == photo3.Id,
                "leaderboard didn't have photos in correct order, photo3 should be last");

            Assert.IsTrue(leaderboard.MostGoldUsers[0].Model.UserId == user.UserId,
                "leaderboard didn't have users in correct order, user should be first");
            Assert.IsNotNull(leaderboard.MostGoldUsers[1].Model.UserId, "leaderboard didn't have a valid user in second");
        }
    }
}