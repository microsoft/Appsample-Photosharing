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
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Serialization;
using PhotoSharingApp.Universal.Unity;
using PhotoSharingApp.Universal.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// The profile page that shows all uploaded photos of a user.
    /// </summary>
    public sealed partial class ProfilePage : BasePage
    {
        private static object _lastUsedNavigationArgs;
        private const int ItemMargin = 4;
        private const double PreferredImageWidth = 300;
        private Thickness _imageMargin;
        private double _imageWidth;
        private ProfileViewModel _viewModel;

        /// <summary>
        /// The constructor
        /// </summary>
        public ProfilePage()
        {
            InitializeComponent();

            UpdateImageWidth();

            SizeChanged += ProfilePage_SizeChanged;
        }

        /// <summary>
        /// The margin of each image control.
        /// </summary>
        public Thickness ImageMargin
        {
            get { return _imageMargin; }
            set
            {
                if (_imageMargin != value)
                {
                    _imageMargin = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The width of each image control.
        /// </summary>
        public double ImageWidth
        {
            get { return _imageWidth; }
            set
            {
                if (Math.Abs(value - _imageWidth) >
                    AppEnvironment.FloatingComparisonTolerance)
                {
                    _imageWidth = value;
                    NotifyPropertyChanged();
                }
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var navigationArgs = e.Parameter;

            // We only load data if we either navigate forwards or
            // if navigation parameters have changed (e.g. showing different user on profile page).
            var loadData = e.NavigationMode != NavigationMode.Back
                           || _lastUsedNavigationArgs != navigationArgs;

            _viewModel = ServiceLocator.Current.GetInstance<ProfileViewModel>(loadData);
            DataContext = _viewModel;

            if (loadData)
            {
                _lastUsedNavigationArgs = navigationArgs;

                if (navigationArgs != null)
                {
                    var profilePageNavigationArgs =
                        SerializationHelper.Deserialize<ProfileViewModelArgs>(navigationArgs as string);

                    await _viewModel.LoadState(profilePageNavigationArgs);
                }
                else
                {
                    await _viewModel.LoadState();
                }
            }
        }

        private void ProfilePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateImageWidth();
        }

        private void UpdateImageWidth()
        {
            if (pageRoot.ActualWidth < 720)
            {
                ImageWidth = pageRoot.ActualWidth;
                ImageMargin = new Thickness(0, 0, 0, 4);
            }
            else
            {
                // Calculating how many items per row we get with preferred with + image margin
                var itemsPerRow = (int) (pageRoot.ActualWidth/(PreferredImageWidth + ItemMargin));

                // Calculating rest and distribute among items
                var rest = (int) (pageRoot.ActualWidth%(PreferredImageWidth + ItemMargin));
                var additionalWidthPerItem = rest/itemsPerRow;

                ImageWidth = PreferredImageWidth + additionalWidthPerItem;
                ImageMargin = new Thickness(0, 0, ItemMargin, ItemMargin);
            }
        }
    }
}