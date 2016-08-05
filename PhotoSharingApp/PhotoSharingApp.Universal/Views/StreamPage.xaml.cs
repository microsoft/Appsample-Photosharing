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
using System.Linq;

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// The photo stream page.
    /// </summary>
    public sealed partial class StreamPage : BasePage
    {
        private const int ItemMargin = 6;
        private double PreferredImageWidth = 300;
        private bool _firstTimeLoadingFinished;
        private Thickness _imageMargin;
        private double _imageWidth;
        private double _imageHeight;
        private StreamViewModel _viewModel;

        public StreamPage()
        {
            InitializeComponent();

            UpdateImageWidth();
            UpdateImageHeight();

            SizeChanged += StreamPage_SizeChanged;

            // We want to scroll to the previously selected item,
            // so we need to watch the Loaded event as this is the
            // earliest time we can change the scroll position.
            photosList.Loaded += (s, e) => UpdateScrollPosition();
        }

        /// <summary>
        /// Gets or sets the height of images in the stream.
        /// </summary>
        public double ImageHeight 
         { 
             get { return _imageHeight; } 
             set 
             { 
                 if (Math.Abs(value - _imageHeight) > 
                     AppEnvironment.FloatingComparisonTolerance) 
                 { 
                     _imageHeight = value; 
                     NotifyPropertyChanged(nameof(ImageHeight)); 
                 } 
             } 
         }

        /// <summary>
        /// Gets or sets the margin of images in the stream.
        /// </summary>
        public Thickness ImageMargin
        {
            get { return _imageMargin; }
            set
            {
                if (_imageMargin != value)
                {
                    _imageMargin = value;
                    NotifyPropertyChanged(nameof(ImageMargin));
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of images in the stream.
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
                    NotifyPropertyChanged(nameof(ImageWidth));
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            var hightlightedPhoto = _viewModel.Photos.FirstOrDefault(p => p.IsHighlighted);
            if (hightlightedPhoto != null)
            {
                hightlightedPhoto.IsHighlighted = false;
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var navigationArgs = e.Parameter.ToString();

            var loadData = e.NavigationMode != NavigationMode.Back;
            _viewModel = ServiceLocator.Current.GetInstance<StreamViewModel>(loadData);
            DataContext = _viewModel;

            _viewModel.Photos.LoadingFinished += Photos_LoadingFinished;

            if (loadData)
            {
                if (navigationArgs.Contains(typeof(StreamViewModelArgs).Name))
                {
                    var args = SerializationHelper.Deserialize<StreamViewModelArgs>(e.Parameter as string);

                    await _viewModel.LoadState(args);
                }
                else if (navigationArgs.Contains(typeof(StreamViewModelThumbnailArgs).Name))
                {
                    var args = SerializationHelper.Deserialize<StreamViewModelThumbnailArgs>(e.Parameter as string);

                    await _viewModel.LoadState(args);
                }
            }
        }

        private void Photos_LoadingFinished(object sender, EventArgs e)
        {
            if (_viewModel?.SelectedPhoto != null)
            {
                _viewModel.SelectedPhoto.IsHighlighted = true;
            }

            if (!_firstTimeLoadingFinished)
            {
                UpdateScrollPosition();
                _firstTimeLoadingFinished = true;
            }
        }

        private void StreamPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateImageWidth();
            UpdateImageHeight();
        }

        private void UpdateImageHeight()
        {
            // Calculating the additional height needed per item
            // depending on the overall page width.
            int additionalHeightPerItem;

            if (pageRoot.ActualWidth < 720)
            {
                additionalHeightPerItem = (int)(ImageWidth / 3.25);
            }
            else if (pageRoot.ActualWidth > 1900)
            {
                additionalHeightPerItem = (int)(ImageWidth / 2);
            }
            else
            {
                additionalHeightPerItem = (int)(ImageWidth / 2.75);
            }

            ImageHeight = ImageWidth + additionalHeightPerItem;
        }

        private void UpdateImageWidth()
        {
            if (pageRoot.ActualWidth < 720)
            {
                ImageWidth = pageRoot.ActualWidth;
                ImageMargin = new Thickness(0, 0, 0, 6);
            }
            else
            {
                // Calculating how many items per row we get with preferred with + image margin
                var itemsPerRow = (int)(pageRoot.ActualWidth / (PreferredImageWidth + ItemMargin + 6));

                // Calculating rest and distribute among items
                var rest = (int)(pageRoot.ActualWidth % (PreferredImageWidth + ItemMargin + 6));
                var additionalWidthPerItem = rest / itemsPerRow;

                ImageWidth = PreferredImageWidth + additionalWidthPerItem;
                ImageMargin = new Thickness(0, 0, ItemMargin, ItemMargin);
            }
        }

        private void UpdateScrollPosition()
        {
            // If there is a photo selected already, we want the view to update
            // the scroll position to show it.
            if (_viewModel?.SelectedPhoto != null)
            {
                photosList.ScrollIntoView(_viewModel.SelectedPhoto);
            }
        }
    }
}