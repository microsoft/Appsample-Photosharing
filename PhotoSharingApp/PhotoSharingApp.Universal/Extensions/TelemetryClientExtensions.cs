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

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.ApplicationInsights;

namespace PhotoSharingApp.Universal.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="TelemetryClient" />.
    /// </summary>
    public static class TelemetryClientExtensions
    {
        /// <summary>
        /// Sends telemetry that includes one property.
        /// </summary>
        /// <param name="telemetryClient">The telemetry client.</param>
        /// <param name="name">The name of the event.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyValue">The value of the property.</param>
        public static void TrackEvent(this TelemetryClient telemetryClient, string name, string propertyName,
            string propertyValue)
        {
            telemetryClient.TrackEvent(name, new Dictionary<string, string>
            {
                {
                    propertyName, propertyValue
                }
            });
        }

        /// <summary>
        /// Sends telemetry that includes the name of the caller member.
        /// </summary>
        /// <param name="telemetryClient">The telemetry client.</param>
        /// <param name="callerMemberName">The The name of the caller member.</param>
        public static void TrackTraceMethodInvoked(this TelemetryClient telemetryClient,
            [CallerMemberName] string callerMemberName = null)
        {
            telemetryClient.TrackTrace($"{callerMemberName} invoked");
        }

        /// <summary>
        /// Sends telemetry that includes the name of the caller member.
        /// </summary>
        /// <param name="telemetryClient">The telemetry client.</param>
        /// <param name="message">The message.</param>
        /// <param name="callerName">The name of the caller member.</param>
        public static void TrackTraceWithCallerName(this TelemetryClient telemetryClient, string message,
            [CallerMemberName] string callerName = "")
        {
            telemetryClient.TrackTrace($"{callerName}/{message}");
        }
    }
}