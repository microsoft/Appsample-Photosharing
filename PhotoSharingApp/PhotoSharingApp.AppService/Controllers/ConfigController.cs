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

using System.Web.Http;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Mobile.Server.Config;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Controllers
{
    /// <summary>
    /// Controller to manage config.
    /// </summary>
    [MobileAppController]
    public class ConfigController : ApiController
    {
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Config controller constructor.
        /// </summary>
        /// <param name="telemetryClient">The telemetry client.</param>
        public ConfigController(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Gets configuration data.
        /// </summary>
        /// <verb>GET</verb>
        /// <url>http://{host}/api/config</url>
        /// <returns>ConfigContract object.</returns>
        [Route("api/config")]
        public ConfigContract GetAsync()
        {
            _telemetryClient.TrackEvent("ConfigController GetAsync invoked");

            var buildVersion = GetType().Assembly.GetName().Version.ToString();

            var configContract = new ConfigContract
            {
                BuildVersion = buildVersion,
                CategoryThumbnailsLargeFormFactor = 16,
                CategoryThumbnailsSmallFormFactor = 6
            };

            return configContract;
        }
    }
}