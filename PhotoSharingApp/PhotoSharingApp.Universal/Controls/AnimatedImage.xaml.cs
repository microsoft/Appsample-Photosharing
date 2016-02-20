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

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace PhotoSharingApp.Universal.Controls
{
    /// <summary>
    /// An image control that allows specifying a placeholder image
    /// and that fades in the actual image once it has been loaded.
    /// </summary>
    public sealed partial class AnimatedImage : UserControl
    {
        /// <summary>
        /// The dependency property for the image source.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(AnimatedImage),
                new PropertyMetadata(default(ImageSource), SourcePropertyChanged));

        /// <summary>
        /// The dependency property for the placeholder.
        /// </summary>
        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.Register("PlaceHolder", typeof(ImageSource), typeof(AnimatedImage),
                new PropertyMetadata(default(ImageSource)));

        private readonly bool areAnimationsEnabled = true;

        /// <summary>
        /// The constructor.
        /// </summary>
        public AnimatedImage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the place holder image.
        /// </summary>
        /// <remarks>
        /// This image is shown until the desired image has been loaded.
        /// </remarks>
        public ImageSource PlaceHolder
        {
            get { return (ImageSource)GetValue(PlaceHolderProperty); }
            set { SetValue(PlaceHolderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the source image.
        /// </summary>
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private void LoadImage()
        {
            imageFadeInStoryboard.Completed += (s, e) =>
            {
                PlaceHolder = null;
                placeHolderImage.Source = null;
            };

            imageFadeInStoryboard.Begin();
        }

        private void OnSourceChanged()
        {
            if (Source != null)
            {
                placeHolderImage.Source = PlaceHolder;
                var bitmapImage = Source as BitmapImage;

                if (bitmapImage != null)
                {
                    image.Source = bitmapImage;

                    if (areAnimationsEnabled)
                    {
                        bitmapImage.ImageOpened += (sender, args) => LoadImage();
                    }
                    else
                    {
                        image.Opacity = 1.0;
                    }
                }
            }
        }

        private static void SourcePropertyChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as AnimatedImage;

            control?.OnSourceChanged();
        }
    }
}