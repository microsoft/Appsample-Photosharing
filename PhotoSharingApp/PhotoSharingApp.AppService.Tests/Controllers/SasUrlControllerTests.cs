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
using Microsoft.ApplicationInsights;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoSharingApp.AppService.Controllers;

namespace PhotoSharingApp.AppService.Tests.Controllers
{
    [TestClass]
    public class SasUrlControllerTests : BaseTest
    {
        private SasUrlController _controller;

        [TestInitialize]
        public void Init()
        {
            _controller = new SasUrlController(new TelemetryClient());
        }

        [TestMethod]
        public async Task GetSasUrlTest()
        {
            var result = await _controller.GetAsync();

            Assert.IsNotNull(result, "Result must be not null");
            Assert.AreEqual(3, result.Count, "Result must contain 3 SAS Urls");
        }
    }
}