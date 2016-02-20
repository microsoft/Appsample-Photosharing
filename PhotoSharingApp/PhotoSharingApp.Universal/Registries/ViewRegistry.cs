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

using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.ViewModels;
using PhotoSharingApp.Universal.Views;

namespace PhotoSharingApp.Universal.Registries
{
    /// <summary>
    /// Registry for View-ViewModel associations.
    /// </summary>
    public class ViewRegistry : IRegistry
    {
        /// <summary>
        /// Configures dependencies.
        /// </summary>
        public void Configure()
        {
            NavigationFacade.AddType(typeof(StreamPage), typeof(StreamViewModel));
            NavigationFacade.AddType(typeof(PhotoDetailsPage), typeof(PhotoDetailsViewModel));
            NavigationFacade.AddType(typeof(SettingsPage), typeof(SettingsViewModel));
            NavigationFacade.AddType(typeof(ProfilePage), typeof(ProfileViewModel));
            NavigationFacade.AddType(typeof(SignInPage), typeof(SignInViewModel));
            NavigationFacade.AddType(typeof(AboutPage), typeof(AboutViewModel));
            NavigationFacade.AddType(typeof(UploadPage), typeof(UploadViewModel));
            NavigationFacade.AddType(typeof(CropPage), typeof(CropViewModel));
            NavigationFacade.AddType(typeof(CameraPage), typeof(CameraViewModel));
            NavigationFacade.AddType(typeof(CategoriesPage), typeof(CategoriesViewModel));
            NavigationFacade.AddType(typeof(WelcomePage), typeof(WelcomeViewModel));

#if DEBUG
            NavigationFacade.AddType(typeof(DebugPage), typeof(DebugViewModel));
#endif
        }
    }
}