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
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using PhotoSharingApp.Universal.Services;

namespace PhotoSharingApp.Universal.Tests.Services
{
    [TestClass]
    public class ServiceClientTests
    {
        private ServiceClient _serviceClient;

        [TestMethod]
        public async Task GetCategoriesTest()
        {
            var categories = await _serviceClient.GetCategories();

            Assert.IsNotNull(categories, "GetCategories returned null");
        }

        [TestMethod]
        public async Task GetConfigTest()
        {
            // Act
            var config = await _serviceClient.GetConfig();

            // Verify
            Assert.IsNotNull(config, "GetConfig returned null");
            Assert.IsNotNull(config.BuildVersion, "Invalid BuildVersion");
        }

        [TestMethod]
        public async Task GetHeroImagesTest()
        {
            // Act
            var heroImages = await _serviceClient.GetHeroImages(5);

            // Verify
            Assert.IsNotNull(heroImages, "GetHeroImages returned null");
        }

        [TestMethod]
        public async Task GetTopCategoriesTest()
        {
            var categories = await _serviceClient.GetTopCategories(10, null);

            Assert.IsNotNull(categories, "GetTopCategories returned null");
        }

        [TestInitialize]
        public void Init()
        {
            _serviceClient = new ServiceClient(new AuthenticationHandler());
        }
    }
}