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

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using PhotoSharingApp.Universal.Camera;
using PhotoSharingApp.Universal.Extensions;

namespace PhotoSharingApp.Universal.Tests.Extensions
{
    [TestClass]
    public class VideoEncodingPropertiesExtensionsTests
    {
        [TestMethod]
        public void GetAspectRatio16To9Test()
        {
            var ratio = VideoEncodingPropertiesExtensions.GetAspectRatio(800, 450);

            Assert.AreEqual(AspectRatio.Ratio16To9, ratio);
        }

        [TestMethod]
        public void GetAspectRatio16To9Test2()
        {
            var ratio = VideoEncodingPropertiesExtensions.GetAspectRatio(2592, 1456);

            Assert.AreEqual(AspectRatio.Ratio16To9, ratio);
        }

        [TestMethod]
        public void GetAspectRatio4To3Test()
        {
            var ratio = VideoEncodingPropertiesExtensions.GetAspectRatio(640, 480);

            Assert.AreEqual(AspectRatio.Ratio4To3, ratio);
        }

        [TestMethod]
        public void GetAspectRatio4To3Test2()
        {
            var ratio = VideoEncodingPropertiesExtensions.GetAspectRatio(3264, 2448);

            Assert.AreEqual(AspectRatio.Ratio4To3, ratio);
        }

        [TestMethod]
        public void GetAspectRatioUnknownTest()
        {
            var ratio = VideoEncodingPropertiesExtensions.GetAspectRatio(0, 0);

            Assert.AreEqual(AspectRatio.Unknown, ratio);
        }

        [TestMethod]
        public void GetAspectRatioUnknownTest2()
        {
            var ratio = VideoEncodingPropertiesExtensions.GetAspectRatio(3000, 100);

            Assert.AreEqual(AspectRatio.Unknown, ratio);
        }
    }
}