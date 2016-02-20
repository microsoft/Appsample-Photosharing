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
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using PhotoSharingApp.Universal.Extensions;
using PhotoSharingApp.Universal.Tests.Attributes;
using Windows.UI.Xaml.Controls;

namespace PhotoSharingApp.Universal.Tests.Extensions
{
    [TestClass]
    public class VisualTreeHelperExtensionsTests
    {
        [UITestMethod]
        public void FindDescendantsForNullParamTest()
        {
            // Act & Verify
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var descendants = VisualTreeHelperExtensions
                    .FindDescendants<Panel>(null)
                    .ToList();
            });
        }

        [UITestMethod]
        public void FindDescendantsForOnlyParentElementTest()
        {
            // Arrange
            var grid = new Grid();

            // Act
            var descendants = VisualTreeHelperExtensions
                .FindDescendants<Panel>(grid)
                .ToList();

            // Verify
            Assert.IsNotNull(descendants);
            Assert.AreEqual(1, descendants.Count);
            Assert.AreEqual(grid, descendants.First());
        }

        [UITestMethod]
        public void FindDescendantsTest()
        {
            // Arrange
            var grid = new Grid();
            var grid2 = new Grid();
            grid.Children.Add(grid2);

            // Act
            var descendants = VisualTreeHelperExtensions
                .FindDescendants<Panel>(grid)
                .ToList();

            // Verify
            Assert.IsNotNull(descendants);
            Assert.AreEqual(2, descendants.Count);
            Assert.AreEqual(grid, descendants.First());
            Assert.AreEqual(grid2, descendants.Skip(1).First());
        }
    }
}