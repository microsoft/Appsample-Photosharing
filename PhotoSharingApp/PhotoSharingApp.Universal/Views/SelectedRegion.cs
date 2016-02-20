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
using PhotoSharingApp.Universal.ComponentModel;
using PhotoSharingApp.Universal.Models;
using Windows.Foundation;

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// Encapsulates the cropping region used by <see cref="CropControl" />.
    /// </summary>
    public class SelectedRegion : ObservableObjectBase
    {
        public const string BottomLeftCornerName = "BottomLeftCorner";
        public const string BottomRightCornerName = "BottomRightCorner";
        public const string TopLeftCornerName = "TopLeftCorner";
        public const string TopRightCornerName = "TopRightCorner";

        private double _bottomRightCornerCanvasLeft;
        private double _bottomRightCornerCanvasTop;
        private Rect _outerRect;
        private Rect _selectedRect;
        private double _topLeftCornerCanvasLeft;
        private double _topLeftCornerCanvasTop;

        /// <summary>
        /// The Canvas.Left property of the BottomRight corner.
        /// </summary>
        public double BottomRightCornerCanvasLeft
        {
            get { return _bottomRightCornerCanvasLeft; }
            protected set
            {
                if (Math.Abs(_bottomRightCornerCanvasLeft - value) >
                    AppEnvironment.FloatingComparisonTolerance)
                {
                    _bottomRightCornerCanvasLeft = value;
                    OnPropertyChanged(nameof(BottomRightCornerCanvasLeft));
                }
            }
        }

        /// <summary>
        /// The Canvas.Top property of the BottomRight corner.
        /// </summary>
        public double BottomRightCornerCanvasTop
        {
            get { return _bottomRightCornerCanvasTop; }
            protected set
            {
                if (Math.Abs(_bottomRightCornerCanvasTop - value) >
                    AppEnvironment.FloatingComparisonTolerance)
                {
                    _bottomRightCornerCanvasTop = value;
                    OnPropertyChanged(nameof(BottomRightCornerCanvasTop));
                }
            }
        }

        /// <summary>
        /// The minimun size of the seleced region
        /// </summary>
        public double MinSelectRegionSize { get; set; }

        /// <summary>
        /// The outer rect. The non-selected region can be represented by the
        /// OuterRect and the SelectedRect.
        /// </summary>
        public Rect OuterRect
        {
            get { return _outerRect; }
            set
            {
                if (_outerRect != value)
                {
                    _outerRect = value;

                    OnPropertyChanged(nameof(OuterRect));
                }
            }
        }

        /// <summary>
        /// The selected region, which is represented by the four corners.
        /// </summary>
        public Rect SelectedRect
        {
            get { return _selectedRect; }
            protected set
            {
                if (_selectedRect != value)
                {
                    _selectedRect = value;

                    OnPropertyChanged(nameof(SelectedRect));
                }
            }
        }

        /// <summary>
        /// The Canvas.Left property of the TopLeft corner.
        /// </summary>
        public double TopLeftCornerCanvasLeft
        {
            get { return _topLeftCornerCanvasLeft; }
            protected set
            {
                if (Math.Abs(_topLeftCornerCanvasLeft - value) >
                    AppEnvironment.FloatingComparisonTolerance)
                {
                    _topLeftCornerCanvasLeft = value;
                    OnPropertyChanged(nameof(TopLeftCornerCanvasLeft));
                }
            }
        }

        /// <summary>
        /// The Canvas.Top property of the TopLeft corner.
        /// </summary>
        public double TopLeftCornerCanvasTop
        {
            get { return _topLeftCornerCanvasTop; }
            protected set
            {
                if (Math.Abs(_topLeftCornerCanvasTop - value) >
                    AppEnvironment.FloatingComparisonTolerance)
                {
                    _topLeftCornerCanvasTop = value;
                    OnPropertyChanged(nameof(TopLeftCornerCanvasTop));
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            NotifyPropertyChanged(propertyName);

            // When the corner is moved, update the SelectedRect.
            if (propertyName == nameof(TopLeftCornerCanvasLeft) ||
                propertyName == nameof(TopLeftCornerCanvasTop) ||
                propertyName == nameof(BottomRightCornerCanvasLeft) ||
                propertyName == nameof(BottomRightCornerCanvasTop))
            {
                SelectedRect = new Rect(
                    TopLeftCornerCanvasLeft,
                    TopLeftCornerCanvasTop,
                    BottomRightCornerCanvasLeft - TopLeftCornerCanvasLeft,
                    BottomRightCornerCanvasTop - _topLeftCornerCanvasTop);
            }
        }

        public void ResetCorner(double topLeftCornerCanvasLeft, double topLeftCornerCanvasTop,
            double bottomRightCornerCanvasLeft, double bottomRightCornerCanvasTop)
        {
            TopLeftCornerCanvasLeft = topLeftCornerCanvasLeft;
            TopLeftCornerCanvasTop = topLeftCornerCanvasTop;
            BottomRightCornerCanvasLeft = bottomRightCornerCanvasLeft;
            BottomRightCornerCanvasTop = bottomRightCornerCanvasTop;
        }

        /// <summary>
        /// Resizes the SelectedRect.
        /// </summary>
        /// <param name="scale">The scaling.</param>
        public void ResizeSelectedRect(double scale)
        {
            if (scale > 1)
            {
                BottomRightCornerCanvasLeft = BottomRightCornerCanvasLeft * scale;
                BottomRightCornerCanvasTop = BottomRightCornerCanvasTop * scale;
                TopLeftCornerCanvasLeft = TopLeftCornerCanvasLeft * scale;
                TopLeftCornerCanvasTop = TopLeftCornerCanvasTop * scale;
            }
            else
            {
                TopLeftCornerCanvasLeft = TopLeftCornerCanvasLeft * scale;
                TopLeftCornerCanvasTop = TopLeftCornerCanvasTop * scale;
                BottomRightCornerCanvasLeft = BottomRightCornerCanvasLeft * scale;
                BottomRightCornerCanvasTop = BottomRightCornerCanvasTop * scale;
            }
        }

        /// <summary>
        /// Update the Canvas.Top and Canvas.Left of the corner.
        /// </summary>
        public void UpdateCorner(string cornerName, double leftUpdate, double topUpdate,
            bool singleCornerMovement = false)
        {
            UpdateCorner(cornerName, leftUpdate, topUpdate,
                MinSelectRegionSize, MinSelectRegionSize, singleCornerMovement);
        }

        /// <summary>
        /// Update the Canvas.Top and Canvas.Left of the corner.
        /// </summary>
        public void UpdateCorner(string cornerName, double leftUpdate, double topUpdate,
            double minWidthSize, double minHeightSize, bool singleCornerMovement = false)
        {
            switch (cornerName)
            {
                case TopLeftCornerName:

                    // Check if current corner is already positioned at one of the boundaries. We want to avoid
                    // any manipulation in this case, because this would change the aspect ratio.
                    if (singleCornerMovement)
                    {
                        if ((Math.Abs(TopLeftCornerCanvasLeft) < AppEnvironment.FloatingComparisonTolerance
                             || Math.Abs(TopLeftCornerCanvasTop) < AppEnvironment.FloatingComparisonTolerance)
                            && (topUpdate < 0 || leftUpdate < 0))
                        {
                            return;
                        }

                        leftUpdate = topUpdate;
                    }

                    TopLeftCornerCanvasLeft = ValidateValue(_topLeftCornerCanvasLeft + leftUpdate,
                        0, _bottomRightCornerCanvasLeft - minWidthSize);
                    TopLeftCornerCanvasTop = ValidateValue(_topLeftCornerCanvasTop + topUpdate,
                        0, _bottomRightCornerCanvasTop - minHeightSize);
                    break;
                case TopRightCornerName:

                    // Check if current corner is already positioned at one of the boundaries. We want to avoid
                    // any manipulation in this case, because this would change the aspect ratio.
                    if (singleCornerMovement)
                    {
                        if ((BottomRightCornerCanvasLeft >= _outerRect.Width
                             || Math.Abs(TopLeftCornerCanvasTop) < AppEnvironment.FloatingComparisonTolerance)
                            && (topUpdate < 0 || leftUpdate > 0))
                        {
                            return;
                        }

                        leftUpdate = -topUpdate;
                    }

                    BottomRightCornerCanvasLeft = ValidateValue(_bottomRightCornerCanvasLeft + leftUpdate,
                        _topLeftCornerCanvasLeft + minWidthSize, _outerRect.Width);
                    TopLeftCornerCanvasTop = ValidateValue(_topLeftCornerCanvasTop + topUpdate,
                        0, _bottomRightCornerCanvasTop - minHeightSize);
                    break;
                case BottomLeftCornerName:

                    // Check if current corner is already positioned at one of the boundaries. We want to avoid
                    // any manipulation in this case, because this would change the aspect ratio.
                    if (singleCornerMovement)
                    {
                        if ((BottomRightCornerCanvasTop >= _outerRect.Height
                             || Math.Abs(TopLeftCornerCanvasLeft) < AppEnvironment.FloatingComparisonTolerance)
                            && (topUpdate > 0 || leftUpdate < 0))
                        {
                            return;
                        }

                        leftUpdate = -topUpdate;
                    }

                    TopLeftCornerCanvasLeft = ValidateValue(_topLeftCornerCanvasLeft + leftUpdate,
                        0, _bottomRightCornerCanvasLeft - minWidthSize);
                    BottomRightCornerCanvasTop = ValidateValue(_bottomRightCornerCanvasTop + topUpdate,
                        _topLeftCornerCanvasTop + minHeightSize, _outerRect.Height);
                    break;
                case BottomRightCornerName:

                    // Check if current corner is already positioned at one of the boundaries. We want to avoid
                    // any manipulation in this case, because this would change the aspect ratio.
                    if (singleCornerMovement)
                    {
                        if ((BottomRightCornerCanvasLeft >= _outerRect.Width
                             || BottomRightCornerCanvasTop >= _outerRect.Height)
                            && (topUpdate > 0 || leftUpdate > 0))
                        {
                            return;
                        }

                        leftUpdate = topUpdate;
                    }

                    BottomRightCornerCanvasLeft = ValidateValue(_bottomRightCornerCanvasLeft + leftUpdate,
                        _topLeftCornerCanvasLeft + minWidthSize, _outerRect.Width);
                    BottomRightCornerCanvasTop = ValidateValue(_bottomRightCornerCanvasTop + topUpdate,
                        _topLeftCornerCanvasTop + minHeightSize, _outerRect.Height);
                    break;
                default:
                    throw new ArgumentException("cornerName: " + cornerName + "  is not recognized.");
            }
        }

        /// <summary>
        /// Update the SelectedRect when it is moved or scaled.
        /// </summary>
        public void UpdateSelectedRect(double scale, double leftUpdate, double topUpdate)
        {
            var width = _bottomRightCornerCanvasLeft - _topLeftCornerCanvasLeft;
            var height = _bottomRightCornerCanvasTop - _topLeftCornerCanvasTop;

            var scaledLeftUpdate = (_bottomRightCornerCanvasLeft - _topLeftCornerCanvasLeft) * (scale - 1) / 2;
            var scaledTopUpdate = (_bottomRightCornerCanvasTop - _topLeftCornerCanvasTop) * (scale - 1) / 2;

            var minWidth = Math.Max(MinSelectRegionSize, width * scale);
            var minHeight = Math.Max(MinSelectRegionSize, height * scale);


            if (scale != 1)
            {
                UpdateCorner(TopLeftCornerName, -scaledLeftUpdate, -scaledTopUpdate);
                UpdateCorner(BottomRightCornerName, scaledLeftUpdate, scaledTopUpdate);
            }

            // Move towards BottomRight: Move BottomRightCorner first, and then move TopLeftCorner.
            if (leftUpdate >= 0 && topUpdate >= 0)
            {
                UpdateCorner(BottomRightCornerName, leftUpdate, topUpdate,
                    minWidth, minHeight);
                UpdateCorner(TopLeftCornerName, leftUpdate, topUpdate,
                    minWidth, minHeight);
            }

            // Move towards TopRight: Move TopRightCorner first, and then move BottomLeftCorner.
            else if (leftUpdate >= 0 && topUpdate < 0)
            {
                UpdateCorner(TopRightCornerName, leftUpdate, topUpdate,
                    minWidth, minHeight);
                UpdateCorner(BottomLeftCornerName, leftUpdate, topUpdate,
                    minWidth, minHeight);
            }

            // Move towards BottomLeft: Move BottomLeftCorner first, and then move TopRightCorner.
            else if (leftUpdate < 0 && topUpdate >= 0)
            {
                UpdateCorner(BottomLeftCornerName, leftUpdate, topUpdate,
                    minWidth, minHeight);
                UpdateCorner(TopRightCornerName, leftUpdate, topUpdate,
                    minWidth, minHeight);
            }

            // Move towards TopLeft: Move TopLeftCorner first, and then move BottomRightCorner.
            else if (leftUpdate < 0 && topUpdate < 0)
            {
                UpdateCorner(TopLeftCornerName, leftUpdate, topUpdate,
                    minWidth, minHeight);
                UpdateCorner(BottomRightCornerName, leftUpdate, topUpdate,
                    minWidth, minHeight);
            }
        }

        private double ValidateValue(double tempValue, double from, double to)
        {
            if (tempValue < from)
            {
                tempValue = from;
            }

            if (tempValue > to)
            {
                tempValue = to;
            }

            return tempValue;
        }
    }
}