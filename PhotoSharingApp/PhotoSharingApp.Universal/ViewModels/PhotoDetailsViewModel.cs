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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PhotoSharingApp.Portable.DataContracts;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Extensions;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Views;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The ViewModel for the photo details view.
    /// </summary>
    public class PhotoDetailsViewModel : ViewModelBase
    {
        /// <summary>
        /// The authentication enforcement handler
        /// </summary>
        private readonly IAuthEnforcementHandler _authEnforcementHandler;

        /// <summary>
        /// The category
        /// </summary>
        private CategoryPreview _category;

        private readonly IDialogService _dialogService;

        /// <summary>
        /// To track if this instance is busy.
        /// </summary>
        private bool _isBusy;

        /// <summary>
        /// Is photo deletion canceled.
        /// </summary>
        private bool _isDeletePhotoCanceled;

        /// <summary>
        /// Is Photo already loaded from service.
        /// </summary>
        private bool _isPhotoLoadedFromService;

        /// <summary>
        /// Is setting the profile photo canceled.
        /// </summary>
        private bool _isSetProfilePhotoCanceled;

        /// <summary>
        /// The Navigation Facade.
        /// </summary>
        private readonly INavigationFacade _navigationFacade;

        /// <summary>
        /// The photo
        /// </summary>
        private Photo _photo;

        /// <summary>
        /// The photo service
        /// </summary>
        private readonly IPhotoService _photoService;

        /// <summary>
        /// Selected annotation
        /// </summary>
        private Annotation _selectedAnnotation;

        /// <summary>
        /// The user
        /// </summary>
        private User _user;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoDetailsViewModel" /> class.
        /// </summary>
        /// <param name="navigationFacade">The navigation facade.</param>
        /// <param name="photoService">The photo service.</param>
        /// <param name="authEnforcementHandler">The auth enforcement handler.</param>
        /// <param name="dialogService">The dialog service.</param>
        public PhotoDetailsViewModel(INavigationFacade navigationFacade, IPhotoService photoService,
            IAuthEnforcementHandler authEnforcementHandler, IDialogService dialogService)
        {
            _navigationFacade = navigationFacade;
            _photoService = photoService;
            _authEnforcementHandler = authEnforcementHandler;
            _dialogService = dialogService;

            // Initialize commands
            GotoCameraCommand = new RelayCommand(OnGotoCamera);
            DeleteAnnotationCommand = new RelayCommand(OnDeleteAnnotation);
            GiveGoldCommand = new RelayCommand(OnGiveGold);
            ReportPhotoCommand = new RelayCommand<ReportReason>(OnReportPhoto);
            ReportAnnotationCommand = new RelayCommand(OnReportAnnotation);
            EditPhotoCommand = new RelayCommand(OnEditPhoto);
            UserSelectedCommand = new RelayCommand<User>(OnUserSelected);
            DeletePhotoCommand = new RelayCommand(OnDeletePhoto);
            CancelDeletePhotoCommand = new RelayCommand(OnCancelDeletePhoto);
            SetProfilePhotoCommand = new RelayCommand(OnSetProfilePhoto);
            CancelSetProfileCommand = new RelayCommand(OnCancelSetProfile);
        }

        /// <summary>
        /// Gets the comments.
        /// </summary>
        /// <value>
        /// The comments.
        /// </value>
        public ObservableCollection<Annotation> Annotations { get; } = new ObservableCollection<Annotation>();

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public CategoryPreview Category
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
        /// Gets the cancel delete photo command.
        /// </summary>
        public RelayCommand CancelDeletePhotoCommand { get; private set; }

        /// <summary>
        /// Gets the cancel set profile photo command.
        /// </summary>
        public RelayCommand CancelSetProfileCommand { get; private set; }

        /// <summary>
        /// Gets the delete Annotation command.
        /// </summary>
        public RelayCommand DeleteAnnotationCommand { get; private set; }

        /// <summary>
        /// Gets the delete photo command.
        /// </summary>
        public RelayCommand DeletePhotoCommand { get; private set; }

        /// <summary>
        /// Gets the edit photo command.
        /// </summary>
        public RelayCommand EditPhotoCommand { get; private set; }

        /// <summary>
        /// Gets the give gold command.
        /// </summary>
        public RelayCommand GiveGoldCommand { get; private set; }

        /// <summary>
        /// Gets the goto camera command.
        /// </summary>
        public RelayCommand GotoCameraCommand { get; private set; }

        /// <summary>
        /// Gets the set profile photo command.
        /// </summary>
        public RelayCommand SetProfilePhotoCommand { get; private set; }

        /// <summary>
        /// Determines if photo can be set as user's profile pic.
        /// </summary>
        public bool CanSetProfilePicture => IsCurrentUsersPhoto && IsPhotoActive;

        /// <summary>
        /// Gets or sets a value indicating if photo has been reported.
        /// </summary>
        public bool HasReported { get; set; }

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
                }
            }
        }

        /// <summary>
        /// Determines if the photo belongs to the current user.
        /// </summary>
        public bool IsCurrentUsersPhoto => AppEnvironment.Instance.CurrentUser != null && AppEnvironment.Instance.CurrentUser.UserId == Photo.User.UserId;

        /// <summary>
        /// Determines if the photo is active.
        /// </summary>
        public bool IsPhotoActive => _photo.HasActiveStatus;

        /// <summary>
        /// Determines if user canceled setting a profile photo.
        /// </summary>
        public bool IsSetProfilePhotoCanceled
        {
            get
            {
                return _isSetProfilePhotoCanceled;
            }
            set
            {
                _isSetProfilePhotoCanceled = value;
                NotifyPropertyChanged(nameof(IsSetProfilePhotoCanceled));

                // When IsDeletePhotoCanceled is set to true, the CloseMenuFlyoutAction is triggered. 
                // We then re-set it to false, making the CloseMenuFlyoutAction possible to trigger again.
                if (value)
                {
                    IsSetProfilePhotoCanceled = false;
                }
            }
        }

        /// <summary>
        /// Determines if user canceled deleting a photo.
        /// </summary>
        public bool IsDeletePhotoCanceled
        {
            get
            {
                return _isDeletePhotoCanceled;
            }
            set
            {
                _isDeletePhotoCanceled = value;
                NotifyPropertyChanged(nameof(IsDeletePhotoCanceled));

                // When IsDeletePhotoCanceled is set to true, the CloseMenuFlyoutAction is triggered. 
                // We then re-set it to false, making the CloseMenuFlyoutAction possible to trigger again.
                if (value)
                {
                    IsDeletePhotoCanceled = false;
                }
            }
        }

        /// <summary>
        /// Determines if user can delete selected annotation.
        /// </summary>
        public bool IsUserAbleToDeleteAnnotation
        {
            get
            {
                if (_selectedAnnotation == null || AppEnvironment.Instance.CurrentUser == null)
                {
                    return false;
                }
                return string.Equals(_selectedAnnotation.From.UserId, AppEnvironment.Instance.CurrentUser.UserId);
            }
        }

        /// <summary>
        /// Determines if user can report selected annotation.
        /// </summary>
        public bool IsUserAbleToReportAnnotation
        {
            get
            {
                if (_selectedAnnotation == null || AppEnvironment.Instance.CurrentUser == null)
                {
                    return false;
                }
                return !string.Equals(_selectedAnnotation.From.UserId, AppEnvironment.Instance.CurrentUser.UserId);
            }
        }

        /// <summary>
        /// Determines if user can report the photo.
        /// </summary>
        public bool IsUserAbleToReportPhoto
        {
            get
            {
                if (AppEnvironment.Instance.CurrentUser == null ||
                    AppEnvironment.Instance.CurrentUser.UserId == _photo.User.UserId)
                {
                    return false;
                }

                foreach (var report in _photo.Reports)
                {
                    if (report.ReporterUserId == AppEnvironment.Instance.CurrentUser.UserId)
                    {
                        return false;
                    }
                }

                if (HasReported == true)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Determines if user can update the photo.
        /// </summary>
        public bool IsUserAbleToUpdatePhoto
        {
            get
            {
                if (AppEnvironment.Instance.CurrentUser == null)
                {
                    return false;
                }

                return string.Equals(Photo.User.UserId, AppEnvironment.Instance.CurrentUser.UserId);
            }
        }

        /// <summary>
        /// Gets or sets the photo.
        /// </summary>
        /// <value>
        /// The photo.
        /// </value>
        public Photo Photo
        {
            get { return _photo; }
            set
            {
                if (value != _photo)
                {
                    _photo = value;
                    NotifyPropertyChanged(nameof(Photo));
                }
            }
        }

        /// <summary>
        /// Gets the report Annotation command.
        /// </summary>
        /// <value>
        /// The report Annotation command.
        /// </value>
        public RelayCommand ReportAnnotationCommand { get; private set; }

        /// <summary>
        /// Gets the report photo command.
        /// </summary>
        /// <value>
        /// The report photo command.
        /// </value>
        public RelayCommand<ReportReason> ReportPhotoCommand { get; private set; }

        /// <summary>
        /// List of report reasons.
        /// </summary>
        public ReportReason[] ReportReasons { get; } =
            Enum.GetValues(typeof(ReportReason)).Cast<ReportReason>().ToArray();

        /// <summary>
        /// Gets or sets selected annotation
        /// </summary>
        public Annotation SelectedAnnotation
        {
            get { return _selectedAnnotation; }
            set
            {
                _selectedAnnotation = value;
                NotifyPropertyChanged(nameof(IsUserAbleToDeleteAnnotation));
                NotifyPropertyChanged(nameof(IsUserAbleToReportAnnotation));
            }
        }

        /// <summary>
        /// Gets the current user;
        /// </summary>
        public User User
        {
            get { return _user; }
            private set
            {
                if (value != _user)
                {
                    _user = value;
                    NotifyPropertyChanged(nameof(User));
                }
            }
        }

        /// <summary>
        /// Gets the user selected command.
        /// </summary>
        public RelayCommand<User> UserSelectedCommand { get; private set; }

        /// <summary>
        /// Load the state.
        /// </summary>
        /// <param name="args">The PhotoDetailsViewModelArgs.</param>
        public async Task LoadState(PhotoDetailsViewModelArgs args)
        {
            await base.LoadState();

            Category = args.Category;
            Photo = args.Photo;
            _isPhotoLoadedFromService = false;

            await ShowDetails();
        }

        /// <summary>
        /// Load the state.
        /// </summary>
        /// <param name="args">The PhotoDetailsViewModelPhotoIdArgs.</param>
        public async Task LoadState(PhotoDetailsViewModelPhotoIdArgs args)
        {
            await base.LoadState();

            Photo = await _photoService.GetPhotoDetails(args.PhotoId);
            _isPhotoLoadedFromService = true;

            await ShowDetails();
        }

        private void OnCancelDeletePhoto()
        {
            IsDeletePhotoCanceled = true;
        }


        private void OnCancelSetProfile()
        {
            IsSetProfilePhotoCanceled = true;
        }

        private async void OnDeleteAnnotation()
        {
            try
            {
                var deleteAnnotation =
                    await _dialogService.ShowYesNoNotification("DeleteComment_Message", "DeleteComment_Title");

                if (deleteAnnotation)
                {
                    await _photoService.RemoveAnnotation(_selectedAnnotation);
                    Annotations.Remove(_selectedAnnotation);
                }
            }
            catch (ServiceException)
            {
                await _dialogService.ShowNotification("DeleteCommentErrorMessage", "GenericErrorTitle");
            }
        }

        private async void OnDeletePhoto()
        {
            if (AppEnvironment.Instance.CurrentUser.ProfilePictureId == Photo.Id)
            {
                await _dialogService.ShowNotification("DeleteProfilePicture_Message", "DeleteProfilePicture_Title");
                return;
            }

            try
            {
                IsBusy = true;
                await _photoService.DeletePhoto(Photo);

                _navigationFacade.NavigateToProfileView();
            }
            catch (ServiceException)
            {
                await _dialogService.ShowGenericServiceErrorNotification();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnEditPhoto()
        {
            _navigationFacade.NavigateToUploadView(Photo, Category.ToCategory());
        }

        private async void OnGiveGold()
        {
            try
            {
                await _authEnforcementHandler.CheckUserAuthentication();

                var annotation = await _navigationFacade.ShowGiveGoldDialog(_photo);
                if (annotation != null)
                {
                    Annotations.Insert(0, annotation);
                }
            }
            catch (SignInRequiredException)
            {
                //Swallow exception. User canceled the Sign-in dialog.
            }
            catch (ServiceException)
            {
                await _dialogService.ShowGenericServiceErrorNotification();
            }
        }

        private void OnGotoCamera()
        {
            _navigationFacade.NavigateToCameraView(Category);
        }

        private async void OnSetProfilePhoto()
        {
            try
            {
                IsBusy = true;
                await _photoService.UpdateUserProfilePhoto(Photo);

                _navigationFacade.NavigateToProfileView();

                User = AppEnvironment.Instance.CurrentUser;
            }
            catch (ServiceException)
            {
                await _dialogService.ShowGenericServiceErrorNotification();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnReportAnnotation()
        {
            try
            {
                IsBusy = true;

                await _authEnforcementHandler.CheckUserAuthentication();
                var result = await _dialogService.ShowYesNoNotification("ReportContent_Message", "ReportContent_Title");

                if (result)
                {
                    await _photoService.ReportAnnotation(_selectedAnnotation);
                }
            }
            catch (SignInRequiredException)
            {
                // Swallow exception. User canceled the Sign-in dialog.
            }
            catch (ServiceException)
            {
                await _dialogService.ShowGenericServiceErrorNotification();
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async void OnReportPhoto(ReportReason reason)
        {
            try
            {
                IsBusy = true;

                await _authEnforcementHandler.CheckUserAuthentication();
                var result = await _dialogService.ShowYesNoNotification("ReportContent_Message", "ReportContent_Title");
                if (result)
                {
                    await _photoService.ReportPhoto(_photo, reason);
                    HasReported = true;
                    NotifyPropertyChanged(nameof(IsUserAbleToReportPhoto));
                }
            }

            catch (SignInRequiredException)
            {
                // Swallow exception.  User canceled the Sign-in dialog.
            }
            catch (ServiceException)
            {
                await _dialogService.ShowGenericServiceErrorNotification();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnUserSelected(User user)
        {
            _navigationFacade.NavigateToProfileView(user);
        }

        /// <summary>
        /// Load details for a photo.
        /// </summary>
        private async Task ShowDetails()
        {
            try
            {
                var photoWithAnnotations = _photo;

                // Avoid making the service call again if it's already been made.
                if (!_isPhotoLoadedFromService)
                {
                    photoWithAnnotations = await _photoService.GetPhotoDetails(Photo.Id);

                    // Update photo gold count.
                    Photo.GoldCount = photoWithAnnotations.GoldCount;
                }

                // Some views will pass the photo without a corresponding
                // category instance, so we need to update it here.
                if (Category == null)
                {
                    Category = photoWithAnnotations.ExtractCategoryPreview();
                }

                var annotations = photoWithAnnotations.Annotations.OrderByDescending(p => p.CreatedTime);

                Annotations.Clear();
                foreach (var annotation in annotations)
                {
                    Annotations.Add(annotation);
                }

                Photo.Reports.Clear();
                foreach (var report in photoWithAnnotations.Reports)
                {
                    Photo.Reports.Add(report);
                }

                NotifyPropertyChanged(nameof(IsUserAbleToReportPhoto));
            }
            catch (ServiceException)
            {
                await _dialogService.ShowGenericServiceErrorNotification();
            }
        }
    }
}