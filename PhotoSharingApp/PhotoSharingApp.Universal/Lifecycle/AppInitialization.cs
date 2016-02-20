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
using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;

namespace PhotoSharingApp.Universal.Lifecycle
{
    /// <summary>
    /// Helper class doing app initializations.
    /// </summary>
    public class AppInitialization
    {
        /// <summary>
        /// Does initialization tasks:
        /// 1. Restore user sign-in
        /// 2. Retrieving/setting service config
        /// </summary>
        /// <returns></returns>
        public static async Task DoInitializations()
        {
            // Sign/In and get current user
            var photoService = ServiceLocator.Current.GetInstance<IPhotoService>();

            try
            {
                // Try to restore sign in status
                await photoService.RestoreSignInStatusAsync();
            }
            catch (Exception)
            {
                // Do nothing: user will have to manually re-sign in.
            }

            try
            {
                var config = await photoService.GetConfig();
                AppEnvironment.Instance.SetConfig(config);
            }
            catch (Exception)
            {
                // Setting default config
                AppEnvironment.Instance.SetConfig(new DefaultConfig());
            }
        }
    }
}