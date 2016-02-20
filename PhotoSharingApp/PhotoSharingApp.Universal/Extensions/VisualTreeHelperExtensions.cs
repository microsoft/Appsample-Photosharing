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
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace PhotoSharingApp.Universal.Extensions
{
    /// <summary>
    /// Provides helper methods for working with the XAML tree.
    /// </summary>
    public static class VisualTreeHelperExtensions
    {
        /// <summary>
        /// Finds all descendants of the given object.
        /// </summary>
        /// <typeparam name="T">The type of descendants to find.</typeparam>
        /// <param name="dependencyObject">The parent object.</param>
        /// <returns>The collection of descendants.</returns>
        public static IEnumerable<T> FindDescendants<T>(DependencyObject dependencyObject) where T : class
        {
            if (dependencyObject == null)
            {
                throw new ArgumentException(nameof(dependencyObject));
            }

            var queue = new Queue<DependencyObject>();

            if (dependencyObject is T)
            {
                yield return dependencyObject as T;
            }

            queue.Enqueue(dependencyObject);

            while (queue.Any())
            {
                var currentElement = queue.Dequeue();
                var currentCount = VisualTreeHelper.GetChildrenCount(currentElement);

                for (var i = 0; i < currentCount; i++)
                {
                    var currentChild = VisualTreeHelper.GetChild(currentElement, i);

                    if (currentChild is T)
                    {
                        yield return currentChild as T;
                    }

                    queue.Enqueue(currentChild);
                }
            }
        }
    }
}