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

using System.Threading.Tasks;
using PhotoSharingApp.Universal.Models;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace PhotoSharingApp.Universal.Facades
{
    /// <summary>
    /// Encapsulates page navigation.
    /// </summary>
    public interface INavigationFacade
    {
        /// <summary>
        /// Goes back to the previews view(s).
        /// </summary>
        /// <param name="steps">Number of views to go back.</param>
        void GoBack(int steps = 1);

        /// <summary>
        /// Navigates to the about view.
        /// </summary>
        void NavigateToAboutView();

        /// <summary>
        /// Navigates to the camera view.
        /// </summary>
        /// <param name="category">The category.</param>
        void NavigateToCameraView(CategoryPreview category);

        /// <summary>
        /// Navigates to the camera view.
        /// </summary>
        void NavigateToCameraView();

        /// <summary>
        /// Navigates to the categories view.
        /// </summary>
        void NavigateToCategoriesView();

        /// <summary>
        /// Navigates to the crop view.
        /// </summary>
        /// <param name="file">The photo to crop.</param>
        /// <param name="category">The selected category for the photo.</param>
        void NavigateToCropView(StorageFile file, CategoryPreview category);

        /// <summary>
        /// Navigates to the crop view.
        /// </summary>
        /// <param name="file">The photo to crop.</param>
        void NavigateToCropView(StorageFile file);

        /// <summary>
        /// Navigates to the photo details view.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="photo">The photo.</param>
        void NavigateToPhotoDetailsView(CategoryPreview category, Photo photo);

        /// <summary>
        /// Navigates to the photo details view.
        /// </summary>
        /// <param name="photo">The photo.</param>
        void NavigateToPhotoDetailsView(Photo photo);

        /// <summary>
        /// Navigates to the photo details view.
        /// </summary>
        /// <param name="photoId">The photoId.</param>
        void NavigateToPhotoDetailsView(string photoId);

        /// <summary>
        /// Navigates to the photo stream view.
        /// </summary>
        /// <param name="category">The category preview instance.</param>
        void NavigateToPhotoStream(CategoryPreview category);

        /// <summary>
        /// Navigates to the photo stream view.
        /// </summary>
        /// <param name="category">The category.</param>
        void NavigateToPhotoStream(Category category);

        /// <summary>
        /// Navigates to the photo stream view.
        /// </summary>
        /// <param name="categoryPreview">The category preview instance.</param>
        /// <param name="photoThumbnail">The photo thumbnail that will determine the scroll position.</param>
        void NavigateToPhotoStream(CategoryPreview categoryPreview, PhotoThumbnail photoThumbnail);

        /// <summary>
        /// Navigates to the signed-in user's profile view.
        /// </summary>
        void NavigateToProfileView();

        /// <summary>
        /// Navigates to the given user's profile view.
        /// </summary>
        /// <param name="user">The user to show the profile view for.</param>
        void NavigateToProfileView(User user);

        /// <summary>
        /// Navigates to the sign-in view.
        /// </summary>
        void NavigateToSignInView();

        /// <summary>
        /// Navigates to the upload view.
        /// </summary>
        /// <param name="image">The image to upload.</param>
        /// <param name="category">The category to upload the image into.</param>
        void NavigateToUploadView(WriteableBitmap image, CategoryPreview category);

        /// <summary>
        /// Navigates to the upload view to update data of
        /// the existing photo.
        /// </summary>
        /// <param name="photo">The photo to update.</param>
        /// <param name="category">The associated category.</param>
        void NavigateToUploadView(Photo photo, Category category);

        /// <summary>
        /// Navigates to the Welcome view.
        /// </summary>
        void NavigateToWelcomeView();

        /// <summary>
        /// Removes the specified number of frames from the back stack.
        /// </summary>
        /// <param name="numberOfFrames">The number of frames.</param>
        void RemoveBackStackFrames(int numberOfFrames);

        /// <summary>
        /// Removes the frames associated with uploading a photo from the back stack.
        /// </summary>
        /// <param name="categoryIdNavigatedTo">The category id that was navigated to after photo upload.</param>
        void RemoveUploadPhotoFramesFromBackStack(string categoryIdNavigatedTo);

        /// <summary>
        /// Displays a dialog that lets the user pick
        /// a category.
        /// </summary>
        /// <returns>The category. If dialog is being canceled, null is returned.</returns>
        Task<Category> ShowCategoryChooserDialog();

        /// <summary>
        /// Displays a dialog that lets the user create
        /// a new category.
        /// </summary>
        Task ShowCreateCategoryDialog();

        /// <summary>
        /// Displays a dialog that lets the user give gold
        /// </summary>
        /// <returns>The annotation. If dialog is being canceled, null is returned.</returns>
        Task<Annotation> ShowGiveGoldDialog(Photo photo);
    }
}