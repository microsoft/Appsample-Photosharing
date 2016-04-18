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
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using PhotoSharingApp.Portable.DataContracts;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.ComponentModel;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Views;
using Windows.UI.Xaml;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// ViewModel for Profile view.
    /// </summary>
    public class ProfileViewModel : ViewModelBase
    {
        private bool _arePhotosEmpty;
        private User _user;
        private readonly IDialogService _dialogService;
        private int _goldBalance;
        private bool _isBusy;
        private readonly INavigationFacade _navigationFacade;
        private int _photosCount;
        private readonly IPhotoService _photoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileViewModel" /> class.
        /// </summary>
        /// <param name="navigationFacade">The navigation facade.</param>
        /// <param name="photoService">The photo service.</param>
        /// <param name="dialogService">The dialog service.</param>
        public ProfileViewModel(INavigationFacade navigationFacade, IPhotoService photoService,
            IDialogService dialogService)
        {
            _navigationFacade = navigationFacade;
            _photoService = photoService;
            _dialogService = dialogService;

            Photos = new IncrementalLoadingCollection<Photo>(s =>
            {
                Func<Task<PagedResponse<Photo>>> f = async () =>
                {
                    if (IsShowingCurrentUser)
                    {
                        var stream = await photoService.GetPhotosForCurrentUser(s);
                        return stream;
                    }

                    return await photoService.GetPhotosForUser(User, s);
                };
                return f();
            }, async () => await _dialogService.ShowGenericServiceErrorNotification(),
                OnPhotosLoadingFinished);

            // Photos collection is being loaded asynchronously, so we need to
            // watch it to see if the user has any pictures uploaded already.
            Photos.CollectionChanged += PhotosCollectionChanged;

            // Initialize commands
            PhotoSelectedCommand = new RelayCommand<Photo>(OnPhotoSelected);
            DeletePhotoCommand = new RelayCommand<Photo>(OnDeletePhoto);
            SetProfilePhotoCommand = new RelayCommand<Photo>(OnSetProfilePhoto);

            // Before pictures are retrieved from the service,
            // we want to prevent an initial notification showing up
            // that the user has no pictures.
            ArePhotosEmpty = false;
        }

        /// <summary>
        /// Returns true, if the user is shown that is signed in. Otherwise, false.
        /// </summary>
        public bool IsShowingCurrentUser => User == AppEnvironment.Instance.CurrentUser;

        /// <summary>
        /// Gets or sets whether photos are available.
        /// </summary>
        public bool ArePhotosEmpty
        {
            get { return _arePhotosEmpty; }
            set
            {
                if (value != _arePhotosEmpty)
                {
                    _arePhotosEmpty = value;
                    NotifyPropertyChanged(nameof(ArePhotosEmpty));
                }
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
        /// Gets the delete photo command.
        /// </summary>
        public RelayCommand<Photo> DeletePhotoCommand { get; private set; }

        /// <summary>
        /// Gets the gold balance
        /// </summary>
        public int GoldBalance
        {
            get { return _goldBalance; }
            private set
            {
                if (value != _goldBalance)
                {
                    _goldBalance = value;
                    NotifyPropertyChanged(nameof(GoldBalance));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
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
        /// Gets or sets the photos.
        /// </summary>
        public IncrementalLoadingCollection<Photo> Photos { get; set; }

        /// <summary>
        /// Gets or sets the photos count.
        /// </summary>
        public int PhotosCount
        {
            get { return _photosCount; }
            set
            {
                if (value != _photosCount)
                {
                    _photosCount = value;
                    NotifyPropertyChanged(nameof(PhotosCount));
                }
            }
        }

        /// <summary>
        /// Gets photo selected command.
        /// </summary>
        public RelayCommand<Photo> PhotoSelectedCommand { get; private set; }

        /// <summary>
        /// Gets the delete photo command.
        /// </summary>
        public RelayCommand<Photo> SetProfilePhotoCommand { get; private set; }

        /// <summary>
        /// Loads the state.
        /// </summary>
        public override async Task LoadState()
        {
            await base.LoadState();

            User = AppEnvironment.Instance.CurrentUser;

            if (User != null)
            {
                RunCounterAnimations();
            }
        }

        /// <summary>
        /// Loads the state.
        /// </summary>
        /// <param name="viewModelArgs">The arguments.</param>
        public async Task LoadState(ProfileViewModelArgs viewModelArgs)
        {
            await base.LoadState();

            User = viewModelArgs.User;
            RunCounterAnimations();
        }

        private async void OnDeletePhoto(Photo photo)
        {
            if (AppEnvironment.Instance.CurrentUser.ProfilePictureId == photo.Id)
            {
                await _dialogService.ShowNotification("DeleteProfilePicture_Message", "DeleteProfilePicture_Title");
                return;
            }

            try
            {
                IsBusy = true;
                await _photoService.DeletePhoto(photo);
                await OnRefresh();
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

        /// <summary>
        /// Action to take when a photo has been selected
        /// </summary>
        /// <param name="photo">the photo.</param>
        private void OnPhotoSelected(Photo photo)
        {
            _navigationFacade.NavigateToPhotoDetailsView(photo);
        }

        private void OnPhotosLoadingFinished()
        {
            ArePhotosEmpty = !Photos.Any();
        }

        private async Task OnRefresh()
        {
            await Photos.Refresh();
            PhotosCount = Photos.Count;
        }

        private async void OnSetProfilePhoto(Photo photo)
        {
            try
            {
                IsBusy = true;
                await _photoService.UpdateUserProfilePhoto(photo);

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

        private void PhotosCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ArePhotosEmpty = !Photos.Any();
        }

        private void RunCounterAnimations()
        {
            var dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
            dispatcherTimer.Tick += (s, e) =>
            {
                var needToProceed = false;

                // Photos counter animation
                if (PhotosCount < Photos.Count)
                {
                    PhotosCount++;
                    needToProceed = true;
                }

                // Gold counter animation
                if (GoldBalance < User.GoldBalance)
                {
                    needToProceed = true;
                    GoldBalance += 1;

                    if (GoldBalance > User.GoldBalance)
                    {
                        GoldBalance = User.GoldBalance;
                    }
                }

                if (!needToProceed)
                {
                    dispatcherTimer.Stop();
                }
            };

            dispatcherTimer.Start();
        }
    }
}