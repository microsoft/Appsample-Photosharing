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
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using PhotoSharingApp.Universal.Extensions;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Serialization;
using PhotoSharingApp.Universal.ViewModels;
using PhotoSharingApp.Universal.Views;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace PhotoSharingApp.Universal.Facades
{
    /// <summary>
    /// Encapsulates page navigation.
    /// </summary>
    public class NavigationFacade : INavigationFacade
    {
        /// <summary>
        /// The mappings between views and their view models
        /// </summary>
        private static readonly Dictionary<Type, Type> ViewViewModelDictionary = new Dictionary<Type, Type>();

        /// <summary>
        /// The current frame.
        /// </summary>
        private Frame _frame;

        /// <summary>
        /// Determines if back navigation
        /// </summary>
        public bool CanGoBack
        {
            get { return _frame.CanGoBack; }
        }

        /// <summary>
        /// Adds the specified types to the association list.
        /// </summary>
        /// <param name="view">The view type.</param>
        /// <param name="viewModel">The ViewModel type.</param>
        /// <exception cref="System.ArgumentException">The ViewModel has already been added and is only allowed once.</exception>
        public static void AddType(Type view, Type viewModel)
        {
            if (ViewViewModelDictionary.ContainsKey(viewModel))
            {
                throw new ArgumentException("The ViewModel has already been added and is only allowed once.");
            }

            ViewViewModelDictionary.Add(viewModel, view);
        }

        /// <summary>
        /// Makes sure a frame is available that can be used
        /// for navigation.
        /// </summary>
        private void EnsureNavigationFrameIsAvailable()
        {
            var content = Window.Current.Content;

            // The default state is that we expect to have the
            // AppShell as a hosting view for content
            if (content is AppShell)
            {
                var appShell = content as AppShell;
                _frame = appShell.AppFrame;
            }

            // We can also have a simple frame when the user
            // chooses to use the share target contract to share
            // photos from the Windows photos app.
            else if (content is Frame)
            {
                var frameShell = content as Frame;
                _frame = frameShell;
            }
            else
            {
                throw new ArgumentException("Window.Current.Content");
            }
        }

        /// <summary>
        /// Goes back in the navigation stack for the specified
        /// number of steps.
        /// </summary>
        /// <param name="steps">The steps. By default: 1.</param>
        public void GoBack(int steps = 1)
        {
            EnsureNavigationFrameIsAvailable();

            if (steps > 1)
            {
                RemoveBackStackFrames(steps - 1);
            }

            if (_frame.CanGoBack)
            {
                _frame.GoBack();
            }
        }

        /// <summary>
        /// Checks if the specified type is a photo upload related page.
        /// </summary>
        /// <param name="pageType">The type.</param>
        /// <returns>Returns true if the specified type is a photo upload related page.</returns>
        private static bool IsTypePhotoUploadRelated(Type pageType)
        {
            return pageType != null &&
                   (pageType == typeof(CameraPage) || pageType == typeof(CropPage) || pageType == typeof(UploadPage));
        }

        /// <summary>
        /// Navigates to the specified view model type.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="parameter">The parameter. Optional.</param>
        /// <param name="serializeParameter">The serialized parameter. Optional.</param>
        private void Navigate(Type viewModelType, object parameter = null, bool serializeParameter = true)
        {
            var view = ViewViewModelDictionary[viewModelType];

            if (view == null)
            {
                throw new ArgumentException("The specified ViewModel could not be found.");
            }

            // Navigation has to be different if the view is a SettingsFlyout 
            // so this is checked here using reflection
            if (view.GetTypeInfo().IsSubclassOf(typeof(SettingsFlyout)))
            {
                // Create instance and show SettingsFlyout
                var flyout = (SettingsFlyout)Activator.CreateInstance(view);
                flyout.ShowIndependent();
            }
            else
            {
                // This is the navigation logic for views that are not
                // inherited from SettingsFlyout
                EnsureNavigationFrameIsAvailable();

                if (parameter == null)
                {
                    _frame.Navigate(view);
                }
                else
                {
                    if (serializeParameter)
                    {
                        var serialized = SerializationHelper.Serialize(parameter);
                        _frame.Navigate(view, serialized);
                    }
                    else
                    {
                        _frame.Navigate(view, parameter);
                    }
                }
            }
        }

        /// <summary>
        /// Navigates to the about view.
        /// </summary>
        public void NavigateToAboutView()
        {
            Navigate(typeof(AboutViewModel));
        }

        /// <summary>
        /// Navigates to the camera view.
        /// </summary>
        /// <param name="category">The category.</param>
        public void NavigateToCameraView(CategoryPreview category)
        {
            Navigate(typeof(CameraViewModel), new CameraViewModelArgs(category));
        }

        /// <summary>
        /// Navigates to the camera view.
        /// </summary>
        public void NavigateToCameraView()
        {
            Navigate(typeof(CameraViewModel), new CameraViewModelArgs(null));
        }

        /// <summary>
        /// Navigates to the categories view.
        /// </summary>
        public void NavigateToCategoriesView()
        {
            Navigate(typeof(CategoriesViewModel));
        }

        /// <summary>
        /// Navigates to the crop view.
        /// </summary>
        /// <param name="file">The photo to crop.</param>
        /// <param name="category">The selected category for the photo.</param>
        public void NavigateToCropView(StorageFile file, CategoryPreview category)
        {
            Navigate(typeof(CropViewModel), new CropViewModelArgs
            {
                Category = category,
                StorageFile = file
            }, false);
        }

        /// <summary>
        /// Navigates to the crop view.
        /// </summary>
        /// <param name="file">The photo to crop.</param>
        public void NavigateToCropView(StorageFile file)
        {
            Navigate(typeof(CropViewModel), new CropViewModelArgs
            {
                Category = null,
                StorageFile = file
            }, false);
        }

        /// <summary>
        /// Navigates to the photo details view.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="photo">The photo.</param>
        public void NavigateToPhotoDetailsView(CategoryPreview category, Photo photo)
        {
            Navigate(typeof(PhotoDetailsViewModel), new PhotoDetailsViewModelArgs(category, photo));
        }

        /// <summary>
        /// Navigates to the photo details view.
        /// </summary>
        /// <param name="photo">The photo.</param>
        public void NavigateToPhotoDetailsView(Photo photo)
        {
            Navigate(typeof(PhotoDetailsViewModel), new PhotoDetailsViewModelArgs(null, photo));
        }

        /// <summary>
        /// Navigates to the photo details view.
        /// </summary>
        /// <param name="photoId">The photoId of the photo to navigate to.</param>
        public void NavigateToPhotoDetailsView(string photoId)
        {
            Navigate(typeof(PhotoDetailsViewModel), new PhotoDetailsViewModelPhotoIdArgs { PhotoId = photoId });
        }

        /// <summary>
        /// Navigates to the photo stream view.
        /// </summary>
        /// <param name="category">The category preview instance.</param>
        public void NavigateToPhotoStream(CategoryPreview category)
        {
            Navigate(typeof(StreamViewModel), new StreamViewModelArgs(category));
        }

        /// <summary>
        /// Navigates to the photo stream view.
        /// </summary>
        /// <param name="category">The category.</param>
        public void NavigateToPhotoStream(Category category)
        {
            NavigateToPhotoStream(category.ToCategoryPreview());
        }

        /// <summary>
        /// Navigates to the photo stream view.
        /// </summary>
        /// <param name="categoryPreview">The category preview instance.</param>
        /// <param name="photoThumbnail">
        /// The photo thumbnail that will determine the scroll position.
        /// </param>
        public void NavigateToPhotoStream(CategoryPreview categoryPreview, PhotoThumbnail photoThumbnail)
        {
            Navigate(typeof(StreamViewModel), new StreamViewModelThumbnailArgs(categoryPreview, photoThumbnail));
        }

        /// <summary>
        /// Navigates to the signed-in user's profile view.
        /// </summary>
        public void NavigateToProfileView()
        {
            Navigate(typeof(ProfileViewModel));
        }

        /// <summary>
        /// Navigates to the given user's profile view.
        /// </summary>
        /// <param name="user">The user to show the profile view for.</param>
        public void NavigateToProfileView(User user)
        {
            Navigate(typeof(ProfileViewModel), new ProfileViewModelArgs(user));
        }

        /// <summary>
        /// Navigates to the sign-in view.
        /// </summary>
        public void NavigateToSignInView()
        {
            Navigate(typeof(SignInViewModel));
        }

        /// <summary>
        /// Navigates to the upload view.
        /// </summary>
        /// <param name="image">The image to upload.</param>
        /// <param name="category">The category to upload the image into.</param>
        public void NavigateToUploadView(WriteableBitmap image, CategoryPreview category)
        {
            Navigate(typeof(UploadViewModel), new UploadViewModelArgs
            {
                Category = category,
                Image = image
            }, false);
        }

        /// <summary>
        /// Navigates to the upload view to update data of
        /// the existing photo.
        /// </summary>
        /// <param name="photo">The photo to update.</param>
        /// <param name="category">The associated category.</param>
        public void NavigateToUploadView(Photo photo, Category category)
        {
            Navigate(typeof(UploadViewModel), new UploadViewModelEditPhotoArgs
            {
                Photo = photo,
                Category = category
            }, false);
        }

        /// <summary>
        /// Navigates to the Welcome View page.
        /// </summary>
        public void NavigateToWelcomeView()
        {
            Navigate(typeof(WelcomeViewModel));
        }

        /// <summary>
        /// Displays a dialog that lets the user pick
        /// a category.
        /// Removes the specified number of frames from the back stack.
        /// </summary>
        /// <param name="numberOfFrames">The number of frames.</param>
        public void RemoveBackStackFrames(int numberOfFrames)
        {
            EnsureNavigationFrameIsAvailable();

            var framesToRemove = numberOfFrames;
            framesToRemove = Math.Min(framesToRemove, _frame.BackStackDepth);

            while (framesToRemove > 0)
            {
                _frame.BackStack.RemoveAt(_frame.BackStackDepth - 1);
                framesToRemove--;
            }
        }

        /// <summary>
        /// Removes the frames associated with uploading a photo from the back stack.
        /// </summary>
        /// <param name="categoryIdNavigatedTo">The category id that was navigated to after photo upload.</param>
        public void RemoveUploadPhotoFramesFromBackStack(string categoryIdNavigatedTo)
        {
            EnsureNavigationFrameIsAvailable();

            var framesToRemove = 0;

            foreach (var frameToTest in _frame.BackStack.Reverse())
            {
                if (IsTypePhotoUploadRelated(frameToTest.SourcePageType))
                {
                    framesToRemove++;
                }
                else
                {
                    // Check if the frame before photo upload began matches the current category stream page
                    if (frameToTest.SourcePageType == typeof(StreamPage))
                    {
                        try
                        {
                            var serializedArgs =
                                SerializationHelper.Deserialize<StreamViewModelArgs>(frameToTest.Parameter.ToString());

                            if (serializedArgs != null && serializedArgs.Category.Id.Equals(categoryIdNavigatedTo))
                            {
                                framesToRemove++;
                            }
                        }
                        catch (SerializationException)
                        {
                            // Swallow exception. Args were of different type than expected.
                        }
                    }

                    // Nothing else should be removed
                    break;
                }
            }

            RemoveBackStackFrames(framesToRemove);
        }

        /// <summary>
        /// Displays a dialog that lets the user pick
        /// a category.
        /// </summary>
        /// <returns>The category. If dialog is being canceled, null is returned.</returns>
        public async Task<Category> ShowCategoryChooserDialog()
        {
            var dialog = new CategoriesChooserDialog();
            await dialog.ShowAsync();

            return dialog.ViewModel.SelectedCategory;
        }

        /// <summary>
        /// Displays a dialog that lets the user create
        /// a new category.
        /// </summary>
        public async Task ShowCreateCategoryDialog()
        {
            var dialog = new CreateCategoryDialog();
            await dialog.ShowAsync();
        }

        /// <summary>
        /// Displays a dialog that lets the user give gold
        /// </summary>
        /// <returns>The annotation. If dialog is being canceled, null is returned.</returns>
        public async Task<Annotation> ShowGiveGoldDialog(Photo photo)
        {
            var dialog = new GiveGoldDialog(photo);
            await dialog.ShowAsync();

            return dialog.ViewModel.Annotation;
        }
    }
}