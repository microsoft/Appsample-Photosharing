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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoSharingApp.AppService.Shared.Context;
using PhotoSharingApp.AppService.Shared.Repositories;
using PhotoSharingApp.AppService.Tests.Context;

namespace PhotoSharingApp.AppService.Tests.DAL
{
    [TestClass]
    public class DocumentDbLoadTests
    {
        private readonly EnvironmentDefinitionBase _environmentDefinition = new TestEnvironmentDefinition();
        private IRepository _repository;

        [TestInitialize]
        public void Init()
        {
            _repository = new DocumentDbRepository(_environmentDefinition);
        }

        [TestMethod]
        public async Task GetLeaderboard50HitsTest()
        {
            for (var i = 0; i < 50; i++)
            {
                await _repository.GetLeaderboard(5, 5, 5, 5);
            }
        }

        [TestMethod]
        public async Task GetRecentPhotosForCategoriesStoredProcedureShouldNotBeBlockedTest()
        {
            try
            {
                for (var i = 0; i < 50; i++)
                {
                    await _repository.GetCategoriesPreview(10);
                }
            }
            catch (Exception e)
            {
                if (
                    e.Message.Contains(
                        "blocked for execution"))
                {
                    Assert.Fail("Stored procedure was blocked");
                }

                // Note: It is fine if it fails due to request rate is too large as we should have caching
                // enabled. The stored procedure should not get blocked though.
            }
        }
    }
}