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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using PhotoSharingApp.Universal.Editing;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Views;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace PhotoSharingApp.Universal.Controls
{
    /// <summary>
    /// A crop control that allows the user to crop images
    /// to a 1:1 ratio before uploading.
    /// </summary>
    public sealed class CropControl : Control, ICropControl, INotifyPropertyChanged
    {
        // Crop control
        private ContentControl _bottomLeftCorner;
        private ContentControl _bottomRightCorner;
        private double _cornerSize;

        // Image
        private IRandomAccessStream _fileStream;
        private Canvas _imageCanvas;
        private Grid _layoutRoot;

        private readonly Dictionary<uint, Point?> _pointerPositionHistory = new Dictionary<uint, Point?>();
        private WriteableBitmap _previewImage;
        private SelectedRegion _selectedRegion;
        private Path _selectRegion;
        private Image _sourceImage;
        private uint _sourceImagePixelHeight;
        private uint _sourceImagePixelWidth;
        private ContentControl _topLeftCorner;
        private ContentControl _topRightCorner;

        /// <summary>
        /// Initializes a new instance of the <see cref="CropControl" /> class.
        /// </summary>
        public CropControl()
        {
            DefaultStyleKey = typeof(CropControl);
        }

        /// <summary>
        /// Gets the size of the corners.
        /// </summary>
        private double CornerSize
        {
            get
            {
                if (_cornerSize <= 0)
                {
                    _cornerSize = (double)Application.Current.Resources["CornerSize"];
                }

                return _cornerSize;
            }
        }

        /// <summary>
        /// Gets the cropped preview image.
        /// </summary>
        public WriteableBitmap PreviewImage
        {
            get { return _previewImage; }
            private set
            {
                _previewImage = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PreviewImage)));
            }
        }

        /// <summary>
        /// Adds pointer events to the given control.
        /// </summary>
        /// <param name="corner">The corner control.</param>
        private void AddCornerEvents(Control corner)
        {
            corner.PointerPressed += Corner_PointerPressed;
            corner.PointerMoved += Corner_PointerMoved;
            corner.PointerReleased += Corner_PointerReleased;
        }

        /// <summary>
        /// If a pointer which is captured by the corner moves，the select region will be updated.
        /// </summary>
        private void Corner_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var currentPoint = e.GetCurrentPoint(this);
            var pointerId = currentPoint.PointerId;

            if (_pointerPositionHistory.ContainsKey(pointerId) && _pointerPositionHistory[pointerId].HasValue)
            {
                var currentPosition = currentPoint.Position;
                var previousPosition = _pointerPositionHistory[pointerId].Value;

                var xUpdate = currentPosition.X - previousPosition.X;
                var yUpdate = currentPosition.Y - previousPosition.Y;

                _selectedRegion.UpdateCorner((sender as ContentControl).Tag as string, xUpdate, yUpdate, true);

                _pointerPositionHistory[pointerId] = currentPosition;
            }

            e.Handled = true;
        }

        /// <summary>
        /// If a pointer presses in the corner, it means that the user starts to move the corner.
        /// 1. Capture the pointer, so that the UIElement can get the Pointer events (PointerMoved,
        /// PointerReleased...) even the pointer is outside of the UIElement.
        /// 2. Record the start position of the move.
        /// </summary>
        private void Corner_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            (sender as UIElement)?.CapturePointer(e.Pointer);

            var currentPoint = e.GetCurrentPoint(this);

            // Record the start point of the pointer.
            _pointerPositionHistory[currentPoint.PointerId] = currentPoint.Position;

            e.Handled = true;
        }

        /// <summary>
        /// The pressed pointer is released, which means that the move is ended.
        /// 1. Release the Pointer.
        /// 2. Clear the position history of the Pointer.
        /// </summary>
        private void Corner_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var pointerId = e.GetCurrentPoint(this).PointerId;

            if (_pointerPositionHistory.ContainsKey(pointerId))
            {
                _pointerPositionHistory.Remove(pointerId);
            }

            (sender as UIElement)?.ReleasePointerCapture(e.Pointer);
            UpdatePreviewImage();
            e.Handled = true;
        }

        /// <summary>
        /// Releases resources and unregisters from event handlers.
        /// </summary>
        public void Dispose()
        {
            _selectedRegion.PropertyChanged -= selectedRegion_PropertyChanged;

            // Handle the pointer events of the corners. 
            RemoveCornerEvents(_topLeftCorner);
            RemoveCornerEvents(_topRightCorner);
            RemoveCornerEvents(_bottomLeftCorner);
            RemoveCornerEvents(_bottomRightCorner);

            // Handle the manipulation events of the selectRegion
            _selectRegion.ManipulationDelta -= SelectRegion_ManipulationDelta;
            _selectRegion.ManipulationCompleted -= SelectRegion_ManipulationCompleted;

            _sourceImage.SizeChanged -= sourceImage_SizeChanged;
        }

        /// <summary>
        /// Creates the non-scaled cropped image.
        /// </summary>
        public async Task<WriteableBitmap> GetCroppedImage()
        {
            var sourceImageWidthScale = _imageCanvas.Width / _sourceImagePixelWidth;
            var sourceImageHeightScale = _imageCanvas.Height / _sourceImagePixelHeight;

            var previewImageSize = new Size(
                _selectedRegion.SelectedRect.Width / sourceImageWidthScale,
                _selectedRegion.SelectedRect.Height / sourceImageHeightScale);

            var croppedImage = await BitmapTools.GetCroppedBitmapAsync(
                _fileStream,
                new Point(_selectedRegion.SelectedRect.X / sourceImageWidthScale,
                    _selectedRegion.SelectedRect.Y / sourceImageHeightScale),
                previewImageSize,
                1);

            return croppedImage;
        }

        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">The file stream must not be null</exception>
        public async Task LoadImage(IRandomAccessStream fileStream)
        {
            if (fileStream == null)
            {
                throw new ArgumentException("fileStream must not be null");
            }

            _fileStream = fileStream;

            _sourceImage.Source = null;

            var decoder = await BitmapDecoder.CreateAsync(fileStream);

            _sourceImagePixelHeight = decoder.PixelHeight;
            _sourceImagePixelWidth = decoder.PixelWidth;

            if (_sourceImagePixelHeight < 2 * CornerSize ||
                _sourceImagePixelWidth < 2 * CornerSize)
            {
                // Image too small.
                throw new ArgumentOutOfRangeException(
                    $"Please select an image which is larger than {2 * CornerSize}*{2 * CornerSize}");
            }

            double sourceImageScale = 1;

            if (_sourceImagePixelHeight < _layoutRoot.ActualHeight &&
                _sourceImagePixelWidth < _layoutRoot.ActualWidth)
            {
                _sourceImage.Stretch = Stretch.None;
            }
            else
            {
                sourceImageScale = Math.Min(_layoutRoot.ActualWidth / _sourceImagePixelWidth,
                    _layoutRoot.ActualHeight / _sourceImagePixelHeight);
                _sourceImage.Stretch = Stretch.Uniform;
            }

            _sourceImage.Source = await BitmapTools.GetCroppedBitmapAsync(
                fileStream,
                new Point(0, 0),
                new Size(_sourceImagePixelWidth, _sourceImagePixelHeight),
                sourceImageScale);

            PreviewImage = null;
        }

        /// <summary>
        /// Template to manage crop selection of image.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _layoutRoot = GetTemplateChild("PART_LayoutRoot") as Grid;

            _selectRegion = GetTemplateChild("PART_SelectRegion") as Path;
            _selectRegion.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            _selectedRegion = new SelectedRegion { MinSelectRegionSize = AppEnvironment.DefaultMinimumCropDimension };
            DataContext = _selectedRegion;

            _sourceImage = GetTemplateChild("PART_SourceImage") as Image;
            _imageCanvas = GetTemplateChild("PART_ImageCanvas") as Canvas;

            _topLeftCorner = GetTemplateChild("PART_TopLeftCorner") as ContentControl;
            _topRightCorner = GetTemplateChild("PART_TopRightCorner") as ContentControl;
            _bottomLeftCorner = GetTemplateChild("PART_BottomLeftCorner") as ContentControl;
            _bottomRightCorner = GetTemplateChild("PART_BottomRightCorner") as ContentControl;

            // Handle the pointer events of the corners.
            AddCornerEvents(_topLeftCorner);
            AddCornerEvents(_topRightCorner);
            AddCornerEvents(_bottomLeftCorner);
            AddCornerEvents(_bottomRightCorner);

            // Handle the manipulation events of the selectRegion
            _selectRegion.ManipulationDelta += SelectRegion_ManipulationDelta;
            _selectRegion.ManipulationCompleted += SelectRegion_ManipulationCompleted;

            _sourceImage.SizeChanged += sourceImage_SizeChanged;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Remove pointer events for the corner.
        /// </summary>
        /// <param name="corner">The selection pointer corners to be removed.</param>
        private void RemoveCornerEvents(Control corner)
        {
            corner.PointerPressed -= Corner_PointerPressed;
            corner.PointerMoved -= Corner_PointerMoved;
            corner.PointerReleased -= Corner_PointerReleased;
        }

        /// <summary>
        /// Show the select region information.
        /// </summary>
        private void selectedRegion_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var widthScale = _imageCanvas.Width / _sourceImagePixelWidth;
            var heightScale = _imageCanvas.Height / _sourceImagePixelHeight;

            Debug.WriteLine("Start Point: ({0},{1})  Size: {2}*{3}",
                Math.Floor(_selectedRegion.SelectedRect.X / widthScale),
                Math.Floor(_selectedRegion.SelectedRect.Y / heightScale),
                Math.Floor(_selectedRegion.SelectedRect.Width / widthScale),
                Math.Floor(_selectedRegion.SelectedRect.Height / heightScale));
        }

        /// <summary>
        /// The manipulation is completed, and then update the preview image
        /// </summary>
        private void SelectRegion_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            UpdatePreviewImage();
        }

        /// <summary>
        /// The user manipulates the selectRegion. The manipulation includes
        /// 1. Translate
        /// 2. Scale
        /// </summary>
        private void SelectRegion_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _selectedRegion.UpdateSelectedRect(e.Delta.Scale, e.Delta.Translation.X, e.Delta.Translation.Y);
            e.Handled = true;
        }

        /// <summary>
        /// This event will be fired when
        /// 1. An new image is opened.
        /// 2. The source of the sourceImage is set to null.
        /// 3. The view state of this application is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sourceImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.IsEmpty || double.IsNaN(e.NewSize.Height) || e.NewSize.Height <= 0)
            {
                _imageCanvas.Visibility = Visibility.Collapsed;
                _selectedRegion.OuterRect = Rect.Empty;
                _selectedRegion.ResetCorner(0, 0, 0, 0);
            }
            else
            {
                _imageCanvas.Visibility = Visibility.Visible;

                _imageCanvas.Height = e.NewSize.Height;
                _imageCanvas.Width = e.NewSize.Width;
                _selectedRegion.OuterRect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);

                if (e.PreviousSize.IsEmpty || double.IsNaN(e.PreviousSize.Height) || e.PreviousSize.Height <= 0)
                {
                    var minLength = Math.Min(e.NewSize.Width, e.NewSize.Height);
                    _selectedRegion.ResetCorner(0, 0, minLength, minLength);
                }
                else
                {
                    var scale = e.NewSize.Height / e.PreviousSize.Height;
                    _selectedRegion.ResizeSelectedRect(scale);
                }

                // Update preview image always, to cover the case, the cropped image
                // is up-to-date even if user did not change cropping area.
                UpdatePreviewImage();
            }
        }

        /// <summary>
        /// Updates the preview image.
        /// </summary>
        private async void UpdatePreviewImage()
        {
            var sourceImageWidthScale = _imageCanvas.Width / _sourceImagePixelWidth;
            var sourceImageHeightScale = _imageCanvas.Height / _sourceImagePixelHeight;

            var previewImageSize = new Size(
                _selectedRegion.SelectedRect.Width / sourceImageWidthScale,
                _selectedRegion.SelectedRect.Height / sourceImageHeightScale);

            double previewImageScale = 1;

            if (!(previewImageSize.Width <= _imageCanvas.Width &&
                  previewImageSize.Height <= _imageCanvas.Height))
            {
                previewImageScale = Math.Min(_imageCanvas.Width / previewImageSize.Width,
                    _imageCanvas.Height / previewImageSize.Height);
            }

            try
            {
                PreviewImage = await BitmapTools.GetCroppedBitmapAsync(
                    _fileStream,
                    new Point(_selectedRegion.SelectedRect.X / sourceImageWidthScale,
                        _selectedRegion.SelectedRect.Y / sourceImageHeightScale),
                    previewImageSize,
                    previewImageScale);
            }
            catch (ArgumentException)
            {
                // Swallow this exception, sometimes XAML data binding
                // delivers some invalid values.
            }
        }
    }
}