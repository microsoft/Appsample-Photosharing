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
using System.IO;
using System.Threading.Tasks;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Extensions;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Views;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The ViewModel for the upload view.
    /// </summary>
    public class UploadViewModel : ViewModelBase
    {
        /// <summary>
        /// The authentication enforcement handler.
        /// </summary>
        private readonly IAuthEnforcementHandler _authEnforcementHandler;

        /// <summary>
        /// The bitmap image.
        /// </summary>
        private BitmapImage _bitmapImage;

        /// <summary>
        /// The category
        /// </summary>
        private Category _category;

        /// <summary>
        /// The comment
        /// </summary>
        private string _comment;

        private readonly IDialogService _dialogService;

        /// <summary>
        /// Specifies the editing mode
        /// </summary>
        private EditingMode _editingMode;

        /// <summary>
        /// Specifies if work is in progress
        /// </summary>
        private bool _isBusy;

        /// <summary>
        /// The navigation facade
        /// </summary>
        private readonly INavigationFacade _navigationFacade;

        /// <summary>
        /// The photo service
        /// </summary>
        private readonly IPhotoService _photoService;

        /// <summary>
        /// The event handler for handling a successful upload.
        /// </summary>
        private readonly IUploadFinishedHandler _uploadFinishedHandler;

        /// <summary>
        /// The writeable bitmap of the photo to upload.
        /// </summary>
        private WriteableBitmap _writeableBitmap;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadViewModel" /> class.
        /// </summary>
        /// <param name="navigationFacade">The navigation facade.</param>
        /// <param name="photoService">The photo service.</param>
        /// <param name="authEnforcementHandler">Authentication enforcement handler.</param>
        /// <param name="uploadFinishedHandler">The handler that is called when the upload finished.</param>
        /// <param name="dialogService">The dialog service.</param>
        public UploadViewModel(INavigationFacade navigationFacade, IPhotoService photoService,
            IAuthEnforcementHandler authEnforcementHandler, IUploadFinishedHandler uploadFinishedHandler,
            IDialogService dialogService)
        {
            _navigationFacade = navigationFacade;
            _photoService = photoService;
            _authEnforcementHandler = authEnforcementHandler;
            _uploadFinishedHandler = uploadFinishedHandler;
            _dialogService = dialogService;

            // Initialize commands
            UploadCommand = new RelayCommand(OnUpload, () => !IsBusy);
            ChooseCategoryCommand = new RelayCommand(OnChooseCategory, () => !IsBusy);
        }

        /// <summary>
        /// Gets or sets the photo as <see cref="BitmapImage" />.
        /// </summary>
        public BitmapImage BitmapImage
        {
            get { return _bitmapImage; }
            set
            {
                if (value != _bitmapImage)
                {
                    _bitmapImage = value;
                    NotifyPropertyChanged(nameof(BitmapImage));
                    NotifyPropertyChanged(nameof(ImageSource));
                }
            }
        }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public Category Category
        {
            get { return _category; }
            set
            {
                if (value != _category)
                {
                    _category = value;
                    NotifyPropertyChanged(nameof(Category));
                }
            }
        }

        /// <summary>
        /// Gets the choose category command.
        /// </summary>
        public RelayCommand ChooseCategoryCommand { get; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (value != _comment)
                {
                    _comment = value;
                    NotifyPropertyChanged(nameof(Comment));
                }
            }
        }

        /// <summary>
        /// Gets the image source that is being displayed
        /// in the UI.
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                if (WriteableBitmap != null)
                {
                    return WriteableBitmap;
                }

                return BitmapImage;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (value != _isBusy)
                {
                    _isBusy = value;
                    NotifyPropertyChanged(nameof(IsBusy));

                    UploadCommand.RaiseCanExecuteChanged();
                    ChooseCategoryCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the photo.
        /// </summary>
        public Photo Photo { get; set; }

        /// <summary>
        /// Gets the upload command.
        /// </summary>
        /// <value>
        /// The upload command.
        /// </value>
        public RelayCommand UploadCommand { get; }

        /// <summary>
        /// Gets or sets the photo as <see cref="WriteableBitmap" />.
        /// </summary>
        public WriteableBitmap WriteableBitmap
        {
            get { return _writeableBitmap; }
            set
            {
                if (value != _writeableBitmap)
                {
                    _writeableBitmap = value;
                    NotifyPropertyChanged(nameof(WriteableBitmap));
                    NotifyPropertyChanged(nameof(ImageSource));
                }
            }
        }

        /// <summary>
        /// Loads the view model state.
        /// </summary>
        /// <param name="uploadViewModelEditPhotoArgs">The arguments.</param>
        public async Task LoadState(UploadViewModelEditPhotoArgs uploadViewModelEditPhotoArgs)
        {
            await base.LoadState();

            _editingMode = EditingMode.Update;

            Category = uploadViewModelEditPhotoArgs.Category;
            Photo = uploadViewModelEditPhotoArgs.Photo;
            Comment = Photo.Caption;

            BitmapImage = new BitmapImage(new Uri(Photo.ImageUrl));
        }

        /// <summary>
        /// Loads the state.
        /// </summary>
        /// <param name="args">The view model arguments.</param>
        public async Task LoadState(UploadViewModelArgs args)
        {
            await base.LoadState();

            _editingMode = EditingMode.New;

            Category = args.Category?.ToCategory();

            try
            {
                // The user needs to be signed-in
                await _authEnforcementHandler.CheckUserAuthentication();

                // When navigating directly from the main window, the user has not
                // selected a category yet, so we need to do this now.
                if (Category == null)
                {
                    Category = await _navigationFacade.ShowCategoryChooserDialog();
                }

                // Here is some synchronization issue going on,
                // when we first set the Photo which propagates to the UI
                // via data binding, and immediately open a ContentDialog via ShowCategoryChooserDialog.
                // We would get random component initialization exceptions from XAML
                // without any further details. This however is only noticeable when
                // the app is launched as share target.
                // As a solution, we assign the selected photo after the chooser dialog 
                // has been awaited.
                WriteableBitmap = args.Image;
            }
            catch (SignInRequiredException)
            {
                await _dialogService.ShowNotification("AuthenticationRequired_Message", "AuthenticationRequired_Title");
                _navigationFacade.GoBack();
            }
        }

        private async void OnChooseCategory()
        {
            var selectedCategory = await _navigationFacade.ShowCategoryChooserDialog();

            if (selectedCategory != null)
            {
                Category = selectedCategory;
            }
        }

        private async void OnUpload()
        {
            if (_editingMode == EditingMode.New)
            {
                await UploadNewPhoto();
            }
            else
            {
                await UpdatePhoto();
            }
        }

        private async Task UpdatePhoto()
        {
            try
            {
                // The user needs to be signed-in
                await _authEnforcementHandler.CheckUserAuthentication();

                // If the current category is not selected, prompt
                // the user to select one.
                if (Category == null)
                {
                    Category = await _navigationFacade.ShowCategoryChooserDialog();

                    // If category is still not selected (User can always cancel),
                    // we need to cancel the upload.
                    if (Category == null)
                    {
                        throw new CategoryRequiredException();
                    }
                }

                IsBusy = true;

                Photo.Caption = Comment;
                Photo.CategoryId = Category.Id;

                await _photoService.UpdatePhoto(Photo);

                _navigationFacade.NavigateToPhotoDetailsView(Category.ToCategoryPreview(), Photo);
            }
            catch (CategoryRequiredException)
            {
                // Swallow exception. User canceled selecting a category.
            }
            catch (SignInRequiredException)
            {
                // Swallow exception. User canceled the Sign-in dialog.
            }
            catch (ServiceException)
            {
                await _dialogService.ShowNotification("GenericError_Message", "GenericError_Title");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Uploads the photo.
        /// </summary>
        private async Task UploadNewPhoto()
        {
            try
            {
                // The user needs to be signed-in
                await _authEnforcementHandler.CheckUserAuthentication();

                // If the current category is not selected, prompt
                // the user to select one.
                if (Category == null)
                {
                    Category = await _navigationFacade.ShowCategoryChooserDialog();

                    // If category is still not selected (User can always cancel),
                    // we need to cancel the upload.
                    if (Category == null)
                    {
                        throw new CategoryRequiredException();
                    }
                }

                IsBusy = true;

                var file =
                    await
                        ApplicationData.Current.TemporaryFolder.CreateFileAsync("myPhoto.jpg",
                            CreationCollisionOption.ReplaceExisting);

                await WriteableBitmap.SaveToStorageFile(file);

                var stream = await file.OpenStreamForReadAsync();

                var uploadedPhoto = await _photoService.UploadPhoto(stream, file.Path, Comment, Category.Id);

                // Refresh gold balance
                AppEnvironment.Instance.CurrentUser = uploadedPhoto.User;

                await _uploadFinishedHandler.OnUploadFinished(Category);
            }
            catch (CategoryRequiredException)
            {
                // User canceled selecting a category.
            }
            catch (SignInRequiredException)
            {
                // User canceled the Sign-in dialog.
            }
            catch (ServiceException)
            {
                await _dialogService.ShowNotification("GenericError_Message", "GenericError_Title");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}