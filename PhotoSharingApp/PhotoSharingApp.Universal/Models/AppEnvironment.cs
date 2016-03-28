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
using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Universal.ComponentModel;
using PhotoSharingApp.Universal.Extensions;
using Windows.System.Profile;

namespace PhotoSharingApp.Universal.Models
{
    /// <summary>
    /// Contains global access to specific data.
    /// </summary>
    public class AppEnvironment : ObservableObjectBase, IAppEnvironment
    {
        public static readonly int DefaultMinimumCropDimension = 200;
        public static readonly double FloatingComparisonTolerance = 0.001;
        private User _currentUser;

        /// <summary>
        /// Gets or sets the number of category thumbnails that are requested
        /// from the service.
        /// </summary>
        public int CategoryThumbnailsCount { get; private set; }

        /// <summary>
        /// Stores the current user that is logged in.
        /// </summary>
        public User CurrentUser
        {
            get { return _currentUser; }
            set
            {
                if (value != _currentUser)
                {
                    _currentUser = value;
                    NotifyPropertyChanged(nameof(CurrentUser));

                    CurrentUserChanged?.Invoke(this, CurrentUser);
                }
            }
        }

        /// <summary>
        /// Gets the device family.
        /// </summary>
        private DeviceFamily DeviceFamily { get; } = AnalyticsInfo.VersionInfo.DeviceFamily.ToDeviceFamily();

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        public static AppEnvironment Instance
        {
            get { return ServiceLocator.Current.GetInstance<IAppEnvironment>() as AppEnvironment; }
        }

        /// <summary>
        /// Determines if app runs on a desktop device.
        /// </summary>
        public bool IsDesktopDeviceFamily
        {
            get { return DeviceFamily == DeviceFamily.Desktop; }
        }

        /// <summary>
        /// Determines if app runs on a mobile device.
        /// </summary>
        public bool IsMobileDeviceFamily
        {
            get { return DeviceFamily == DeviceFamily.Mobile; }
        }

        /// <summary>
        /// Invoked when the current user has changed.
        /// </summary>
        public event EventHandler<User> CurrentUserChanged;

        /// <summary>
        /// Applies the given configuration.
        /// </summary>
        /// <param name="config">The configuration to apply.</param>
        public void SetConfig(Config config)
        {
            if (IsMobileDeviceFamily)
            {
                CategoryThumbnailsCount = config.CategoryThumbnailsSmallFormFactor;
            }
            else
            {
                CategoryThumbnailsCount = config.CategoryThumbnailsLargeFormFactor;
            }
        }
    }
}