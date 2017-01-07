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
using Windows.ApplicationModel.Resources;

namespace PhotoSharingApp.Universal.Extensions
{
    /// <summary>
    /// Provides extensions for the <see cref="DateTime" /> class.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets the relative time.
        /// </summary>
        /// <param name="dateTime">The date time offset.</param>
        /// <returns>The relative time.</returns>
        internal static string ToRelativeTime(this DateTime dateTime)
        {
            var s = DateTime.Now.Subtract(dateTime);
            var resourceLoader = ResourceLoader.GetForCurrentView();

            var dayDifference = (int)s.TotalDays;

            var secondDifference = (int)s.TotalSeconds;

            if (dayDifference < 0)
            {
                return null;
            }

            if (dayDifference == 0)
            {
                if (secondDifference < 60)
                {
                    return resourceLoader.GetString("DateTime_JustNow");
                }

                if (secondDifference < 120)
                {
                    return resourceLoader.GetString("DateTime_OneMinute");
                }

                if (secondDifference < 3600)
                {
                    return string.Format(resourceLoader.GetString("DateTime_MinutesAgo"), secondDifference/60);
                }

                if (secondDifference < 7200)
                {
                    return resourceLoader.GetString("DateTime_OneHour");
                }

                if (secondDifference < 86400)
                {
                    return string.Format(resourceLoader.GetString("DateTime_HoursAgo"), secondDifference / 3600);
                }
            }

            if (dayDifference == 1)
            {
                return resourceLoader.GetString("DateTime_Yesterday");
            }

            if (dayDifference < 7)
            {
                return String.Format(resourceLoader.GetString("DateTime_DaysAgo"), dayDifference);
            }

            if (dayDifference < 14)
            {
                return resourceLoader.GetString("DateTime_OneWeekAgo");
            }

            if (dayDifference < 31)
            {
                return String.Format(resourceLoader.GetString("DateTime_WeeksAgo"), dayDifference / 7);
            }

            if (dayDifference < 62)
            {
                return resourceLoader.GetString("DateTime_OneMonthAgo");
            }

            return String.Format(resourceLoader.GetString("DateTime_MonthsAgo"), dayDifference / 31);
        }
    }
}