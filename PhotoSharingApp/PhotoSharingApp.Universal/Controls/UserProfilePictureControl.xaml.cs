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
using Windows.UI.Xaml.Controls;

namespace PhotoSharingApp.Universal.Controls
{
    /// <summary>
    /// Control which displays a user's profile picture.
    /// </summary>
    public sealed partial class UserProfilePictureControl : UserControl
    {
        /// <summary>
        /// Identified the ShowBorder dependency property
        /// </summary>
        public static readonly DependencyProperty ShowBorderProperty =
            DependencyProperty.Register("ShowBorder", typeof(object),
                typeof(UserProfilePictureControl), new PropertyMetadata(null));

        /// <summary>
        /// Identified the Source dependency property
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object),
                typeof(UserProfilePictureControl), new PropertyMetadata(null));

        /// <summary>
        /// The constructor
        /// </summary>
        public UserProfilePictureControl()
        {
            InitializeComponent();
            ShowBorder = false;
            layoutRoot.DataContext = this;
        }

        /// <summary>
        /// Gets or sets the ShowBorder which is used as a visibility modifier.
        /// </summary>
        public bool ShowBorder
        {
            get { return (bool) GetValue(ShowBorderProperty); }
            set { SetValue(ShowBorderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Source which is being displayed
        /// </summary>
        public Uri Source
        {
            get { return (Uri) GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
    }
}