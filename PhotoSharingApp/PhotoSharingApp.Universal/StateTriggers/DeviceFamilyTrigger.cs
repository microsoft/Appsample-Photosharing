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

using Windows.System.Profile;
using Windows.UI.Xaml;

namespace PhotoSharingApp.Universal.StateTriggers
{
    /// <summary>
    /// Custom trigger for DeviceFamily UI states.
    /// </summary>
    public class DeviceFamilyTrigger : StateTriggerBase
    {
        private string _currentDeviceFamily;
        private string _queriedDeviceFamily;

        /// <summary>
        /// The target device family.
        /// </summary>
        public string DeviceFamily
        {
            get { return _queriedDeviceFamily; }
            set
            {
                _queriedDeviceFamily = value;

                // Get the current device family.
                _currentDeviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;

                // The trigger will be activated if the current device family
                // matches the device family value in XAML.
                SetActive(_queriedDeviceFamily == _currentDeviceFamily);
            }
        }
    }
}