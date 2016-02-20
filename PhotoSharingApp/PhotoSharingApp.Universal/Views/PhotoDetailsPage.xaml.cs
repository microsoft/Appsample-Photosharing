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
using PhotoSharingApp.Universal.Serialization;
using PhotoSharingApp.Universal.Unity;
using PhotoSharingApp.Universal.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// The page that shows details for a selected photo.
    /// </summary>
    public sealed partial class PhotoDetailsPage : Page
    {
        private PhotoDetailsViewModel _viewModel;

        public PhotoDetailsPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var navigationArgs = e.Parameter.ToString();

            var loadData = e.NavigationMode != NavigationMode.Back;
            _viewModel = ServiceLocator.Current.GetInstance<PhotoDetailsViewModel>(loadData);
            DataContext = _viewModel;

            if (loadData)
            {
                if (navigationArgs.Contains(typeof(PhotoDetailsViewModelArgs).Name))
                {
                    var photoDetailsViewModelArgs =
                        SerializationHelper.Deserialize<PhotoDetailsViewModelArgs>(navigationArgs);
                    await _viewModel.LoadState(photoDetailsViewModelArgs);
                }
                else if (navigationArgs.Contains(typeof(PhotoDetailsViewModelPhotoIdArgs).Name))
                {
                    var photoDetailsViewModelIdArgs =
                        SerializationHelper.Deserialize<PhotoDetailsViewModelPhotoIdArgs>(navigationArgs);
                    await _viewModel.LoadState(photoDetailsViewModelIdArgs);
                }
            }
        }
    }
}