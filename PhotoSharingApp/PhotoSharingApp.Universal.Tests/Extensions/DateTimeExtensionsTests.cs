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
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using PhotoSharingApp.Universal.Extensions;

namespace PhotoSharingApp.Universal.Tests.Extensions
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        [TestMethod]
        public void ToRelativeTime1HourTest()
        {
            var dateTime = DateTime.Now - TimeSpan.FromHours(1);

            Assert.AreEqual("1 hour ago", dateTime.ToRelativeTime());
        }

        [TestMethod]
        public void ToRelativeTime1MinuteTest()
        {
            var dateTime = DateTime.Now - TimeSpan.FromMinutes(1);

            Assert.AreEqual("1 minute ago", dateTime.ToRelativeTime());
        }

        [TestMethod]
        public void ToRelativeTime1MonthTest()
        {
            var dateTime = DateTime.Now - TimeSpan.FromDays(31);

            Assert.AreEqual("1 month ago", dateTime.ToRelativeTime());
        }

        [TestMethod]
        public void ToRelativeTime1WeekTest()
        {
            var dateTime = DateTime.Now - TimeSpan.FromDays(7);

            Assert.AreEqual("1 week ago", dateTime.ToRelativeTime());
        }

        [TestMethod]
        public void ToRelativeTime2DaysTest()
        {
            var dateTime = DateTime.Now - TimeSpan.FromDays(2);

            Assert.AreEqual("2 days ago", dateTime.ToRelativeTime());
        }

        [TestMethod]
        public void ToRelativeTime2HoursTest()
        {
            var dateTime = DateTime.Now - TimeSpan.FromHours(2);

            Assert.AreEqual("2 hours ago", dateTime.ToRelativeTime());
        }

        [TestMethod]
        public void ToRelativeTime2MinutesTest()
        {
            var dateTime = DateTime.Now - TimeSpan.FromMinutes(2);

            Assert.AreEqual("2 minutes ago", dateTime.ToRelativeTime());
        }

        [TestMethod]
        public void ToRelativeTime2MonthsTest()
        {
            var dateTime = DateTime.Now - TimeSpan.FromDays(62);

            Assert.AreEqual("2 months ago", dateTime.ToRelativeTime());
        }

        [TestMethod]
        public void ToRelativeTime2WeeksTest()
        {
            var dateTime = DateTime.Now - TimeSpan.FromDays(14);

            Assert.AreEqual("2 weeks ago", dateTime.ToRelativeTime());
        }

        [TestMethod]
        public void ToRelativeTimeJustnowTest()
        {
            var dateTime = DateTime.Now;

            Assert.AreEqual("just now", dateTime.ToRelativeTime());
        }

        [TestMethod]
        public void ToRelativeTimeYesterdayTest()
        {
            var dateTime = DateTime.Now - TimeSpan.FromDays(1);

            Assert.AreEqual("yesterday", dateTime.ToRelativeTime());
        }
    }
}