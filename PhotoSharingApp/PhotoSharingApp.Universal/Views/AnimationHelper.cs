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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// Helper class for UI animations.
    /// </summary>
    public static class AnimationHelper
    {
        private static void AnimateOpacity(DependencyObject target, double from, double to)
        {
            var opacityAnimation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(500)
            };

            Storyboard.SetTarget(opacityAnimation, target);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");

            var storyboard = new Storyboard();
            storyboard.Children.Add(opacityAnimation);
            storyboard.Begin();
        }

        /// <summary>
        /// Fades in the given dependency object.
        /// </summary>
        /// <param name="target">The target dependency object to fade in.</param>
        public static void FadeIn(DependencyObject target)
        {
            AnimateOpacity(target, 0, 1);
        }
    }
}