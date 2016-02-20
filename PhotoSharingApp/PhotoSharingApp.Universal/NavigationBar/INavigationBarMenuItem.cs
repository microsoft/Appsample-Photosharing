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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace PhotoSharingApp.Universal.NavigationBar
{
    /// <summary>
    /// Defines a menu item for the navigation bar.
    /// </summary>
    public interface INavigationBarMenuItem
    {
        /// <summary>
        /// Gets the arguments that can be passed optionally to the target page.
        /// </summary>
        object Arguments { get; }

        /// <summary>
        /// Gets the type of the destination page.
        /// </summary>
        Type DestPage { get; }

        /// <summary>
        /// Gets the image that is displayed in the navigation bar.
        /// </summary>
        ImageSource Image { get; }

        /// <summary>
        /// Gets the title displayed in the navigation bar.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Gets the positions of the current item in the navigation bar.
        /// </summary>
        NavigationBarItemPosition Position { get; }

        /// <summary>
        /// Gets the symbol that is displayed in the navigation bar.
        /// </summary>
        Symbol Symbol { get; }

        /// <summary>
        /// Gets the symbol character that is displayed in the
        /// navigation bar.
        /// </summary>
        char SymbolAsChar { get; }
    }
}