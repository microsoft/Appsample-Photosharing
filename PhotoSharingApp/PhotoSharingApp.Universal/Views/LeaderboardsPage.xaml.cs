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

using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Unity;
using PhotoSharingApp.Universal.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using System;

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// The leaderboards page.
    /// </summary>
    public sealed partial class LeaderboardsPage : BasePage
    {
        private double _leaderboardsControlWidthAndHeight;
        private double _hubSectionHeaderFontSize = 1;
        private const double NavMenuWidth = 48;

        private LeaderboardViewModel _viewModel;

        /// <summary>
        /// The constructor.
        /// </summary>
        public LeaderboardsPage()
        {
            InitializeComponent();

            SizeChanged += LeaderboardsPage_SizeChanged;
            Loaded += LeaderboardsPage_Loaded;
        }

        /// <summary>
        /// The font size of each hub section header.
        /// </summary>
        public double HubSectionHeaderFontSize
        {
            get { return _hubSectionHeaderFontSize; }
            set
            {
                if (Math.Abs(value - _hubSectionHeaderFontSize) >
                    AppEnvironment.FloatingComparisonTolerance)
                {
                    _hubSectionHeaderFontSize = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The width and height of each LeaderboardsControl.
        /// </summary>
        public double LeaderboardsControlWidthAndHeight
        {
            get { return _leaderboardsControlWidthAndHeight; }
            set
            {
                if (Math.Abs(value - _leaderboardsControlWidthAndHeight) >
                    AppEnvironment.FloatingComparisonTolerance)
                {
                    _leaderboardsControlWidthAndHeight = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void LeaderboardsPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateLeaderboardsControlWidthAndHeight();
            UpdateHubSectionHeaderFontSize();
        }

        private void LeaderboardsPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLeaderboardsControlWidthAndHeight();
            UpdateHubSectionHeaderFontSize();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var loadData = e.NavigationMode != NavigationMode.Back;
            _viewModel = ServiceLocator.Current.GetInstance<LeaderboardViewModel>(loadData);
            DataContext = _viewModel;

            if (loadData)
            {
                await _viewModel.LoadState();
            }
        }

        private void UpdateHubSectionHeaderFontSize()
        {
            if (leaderboardHubControl.Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
            {
                HubSectionHeaderFontSize = 30;
            }
            else if (ActualWidth + NavMenuWidth >= 1400)
            {
                HubSectionHeaderFontSize = 25;
            }
            else if (ActualWidth + NavMenuWidth >= 900)
            {
                HubSectionHeaderFontSize = 19;
            }
            else
            {
                HubSectionHeaderFontSize = 15;
            }
        }

        private void UpdateLeaderboardsControlWidthAndHeight()
        {
            if (leaderboardHubControl.Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
            {
                LeaderboardsControlWidthAndHeight = pageRoot.ActualWidth * .9;
            }
            else
            {
                LeaderboardsControlWidthAndHeight = pageRoot.ActualWidth * .2;
            }
        }
    }
}