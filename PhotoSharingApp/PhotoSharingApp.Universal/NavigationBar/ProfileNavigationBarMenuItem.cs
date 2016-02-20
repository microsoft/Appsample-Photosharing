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
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Views;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace PhotoSharingApp.Universal.NavigationBar
{
    /// <summary>
    /// Navigation bar menu item that navigates to the
    /// <see cref="ProfilePage" />.
    /// </summary>
    public class ProfileNavigationBarMenuItem : NavigationBarMenuItemBase,
        INavigationBarMenuItem
    {
        public ProfileNavigationBarMenuItem()
        {
            AppEnvironment.Instance.CurrentUserChanged += CurrentUserChanged;
        }

        /// <summary>
        /// Gets the type of the destination page.
        /// </summary>
        public Type DestPage
        {
            get { return AppEnvironment.Instance.CurrentUser == null ? typeof(SignInPage) : typeof(ProfilePage); }
        }

        /// <summary>
        /// Gets the image that is displayed in the navigation bar.
        /// </summary>
        public override ImageSource Image
        {
            get
            {
                if (AppEnvironment.Instance.CurrentUser?.ProfilePictureUrl == null)
                {
                    return null;
                }

                return new BitmapImage(new Uri(AppEnvironment.Instance.CurrentUser.ProfilePictureUrl));
            }
        }

        /// <summary>
        /// Gets the title displayed in the navigation bar.
        /// </summary>
        public string Label
        {
            get
            {
                return AppEnvironment.Instance.CurrentUser == null ? "Sign in" : "My Profile";
            }
        }

        /// <summary>
        /// Gets the symbol that is displayed in the navigation bar.
        /// </summary>
        public override Symbol Symbol
        {
            get { return Symbol.Contact; }
        }

        void CurrentUserChanged(object sender, User e)
        {
            // Notify UI that user has changed.
            NotifyPropertyChanged(nameof(Image));
            NotifyPropertyChanged(nameof(Symbol));
            NotifyPropertyChanged(nameof(Label));
            NotifyPropertyChanged(nameof(SymbolAsChar));
        }
    }
}