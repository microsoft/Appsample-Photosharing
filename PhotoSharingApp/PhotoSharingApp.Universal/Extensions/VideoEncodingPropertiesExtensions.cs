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
using PhotoSharingApp.Universal.Camera;
using Windows.Media.MediaProperties;

namespace PhotoSharingApp.Universal.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="VideoEncodingProperties" /> class.
    /// </summary>
    public static class VideoEncodingPropertiesExtensions
    {
        /// <summary>
        /// The 16/9 aspect ratio
        /// </summary>
        public const double AspectRatio16To9 = 16.0 / 9.0;

        /// <summary>
        /// The 4/3 aspect ratio
        /// </summary>
        public const double AspectRatio4To3 = 4.0 / 3.0;

        /// <summary>
        /// Gets the aspect ratio for the given <see cref="IVideoEncodingProperties" /> instance.
        /// </summary>
        /// <param name="encodingProperties">The encoding properties.</param>
        /// <returns>The Aspect ratio.</returns>
        public static AspectRatio GetAspectRatio(this VideoEncodingProperties encodingProperties)
        {
            return GetAspectRatio(encodingProperties.Width, encodingProperties.Height);
        }

        /// <summary>
        /// Gets the aspect ratio.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The Aspect ratio.</returns>
        public static AspectRatio GetAspectRatio(double width, double height)
        {
            var ratio = width / height;

            if (Math.Abs(ratio - AspectRatio16To9) <= 0.1)
            {
                return AspectRatio.Ratio16To9;
            }

            if (Math.Abs(ratio - AspectRatio4To3) <= 0.1)
            {
                return AspectRatio.Ratio4To3;
            }

            return AspectRatio.Unknown;
        }
    }
}