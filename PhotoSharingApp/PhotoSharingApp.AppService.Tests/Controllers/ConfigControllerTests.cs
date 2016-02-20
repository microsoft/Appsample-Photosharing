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
using Microsoft.ApplicationInsights;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoSharingApp.AppService.Controllers;

namespace PhotoSharingApp.AppService.Tests.Controllers
{
    [TestClass]
    public class ConfigControllerTests
    {
        private ConfigController _controller;

        [TestInitialize]
        public void Init()
        {
            _controller = new ConfigController(new TelemetryClient());
        }

        [TestMethod]
        public void GetConfigTest()
        {
            var config = _controller.GetAsync();

            Version version;
            var result = Version.TryParse(config.BuildVersion, out version);

            Assert.IsTrue(result, "BuildVersion must be a valid version");
            Assert.AreEqual(16, config.CategoryThumbnailsLargeFormFactor);
            Assert.AreEqual(6, config.CategoryThumbnailsSmallFormFactor);
        }
    }
}