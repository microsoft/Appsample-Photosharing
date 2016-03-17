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
using PhotoSharingApp.AppService.Shared.Caching;

namespace PhotoSharingApp.AppService.Tests.Caching
{
    [TestClass]
    public class MemoryCacheServiceTests
    {
        private MemoryCacheService _cache;

        [TestInitialize]
        public void Init()
        {
            _cache = new MemoryCacheService { ExpirationTimeSpan = TimeSpan.FromSeconds(1) };
            _cache.Clear();
        }

        [TestMethod]
        public async Task TestGetSet()
        {
            // Arrange
            var obj = new object();

            // Pre-Verify
            Assert.IsFalse(_cache.Contains("TestObject"));

            // Act
            await _cache.GetOrInsert(() => Task.FromResult(obj), "TestObject");

            // Verify
            Assert.IsTrue(_cache.Contains("TestObject"));
        }

        [TestMethod]
        public async Task GetExpiredItemTest()
        {
            // Arrange
            var obj = new object();

            // Pre-Verify
            Assert.IsFalse(_cache.Contains("TestObject"));

            // Act
            await _cache.GetOrInsert(() => Task.FromResult(obj), "TestObject");

            // Verify
            Assert.IsTrue(_cache.Contains("TestObject"));

            // Wait
            await Task.Delay(_cache.ExpirationTimeSpan + TimeSpan.FromSeconds(1));

            // Verify
            Assert.IsFalse(_cache.Contains("TestObject"));
        }
    }
}