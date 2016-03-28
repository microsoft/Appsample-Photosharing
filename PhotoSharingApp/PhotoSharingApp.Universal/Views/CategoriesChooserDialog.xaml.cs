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

using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// The dialog that allows the user to choose a category.
    /// </summary>
    public sealed partial class CategoriesChooserDialog : ContentDialog
    {
        public CategoriesChooserDialog()
        {
            InitializeComponent();
            ViewModel = ServiceLocator.Current.GetInstance<CategoriesChooserViewModel>();
            DataContext = ViewModel;

            // Register for events
            Loaded += CategoriesChooserDialog_Loaded;
        }

        public CategoriesChooserViewModel ViewModel { get; }

        private void CancelButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ViewModel.SelectedCategory = null;
        }

        private async void CategoriesChooserDialog_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadState();

            if (AppEnvironment.Instance.IsDesktopDeviceFamily)
            {
                // On desktop we set the focus instantly to
                // the search box so that the user is able
                // type search text right away.
                // On mobile, this would expand the virtual keyboard
                // and takes away too much space by default.
                searchTextBox.Focus(FocusState.Keyboard);
            }
        }
    }
}