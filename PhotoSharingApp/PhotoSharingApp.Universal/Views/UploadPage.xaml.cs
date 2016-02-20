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
using PhotoSharingApp.Universal.ViewModels;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// The page that allows the user to upload a photo with a description.
    /// </summary>
    public sealed partial class UploadPage : Page
    {
        private object _navigatedToArgs;
        private readonly UploadViewModel _viewModel;

        public UploadPage()
        {
            InitializeComponent();

            _viewModel = ServiceLocator.Current.GetInstance<UploadViewModel>();
            DataContext = _viewModel;

            Loaded += UploadPage_Loaded;

            // We need to prevent the virtual keyboard covering the textbox control
            InputPane.GetForCurrentView().Showing += (s, args) =>
            {
                RenderTransform = new TranslateTransform { Y = -args.OccludedRect.Height };
                args.EnsuredFocusedElementInView = true;
            };

            InputPane.GetForCurrentView().Hiding += (s, args) =>
            {
                RenderTransform = new TranslateTransform { Y = 0 };
                args.EnsuredFocusedElementInView = false;
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _navigatedToArgs = e.Parameter;
        }

        private async void UploadPage_Loaded(object sender, RoutedEventArgs e)
        {
            // The ViewModel may display a ContentDialog for choosing the category,
            // and we need to do this after this page is fully loaded, otherwise
            // we can get some layout issues where the commandbar will be visible while
            // contentdialog is active.
            if (_navigatedToArgs is UploadViewModelArgs)
            {
                await _viewModel.LoadState((UploadViewModelArgs) _navigatedToArgs);
            }
            else if (_navigatedToArgs is UploadViewModelEditPhotoArgs)
            {
                await _viewModel.LoadState((UploadViewModelEditPhotoArgs) _navigatedToArgs);
            }
        }
    }
}