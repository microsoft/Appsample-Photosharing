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
    /// The ViewModel for the categories view.
    /// </summary>
    public class CategoriesViewModel : ViewModelBase
    {
        /// <summary>
        /// The number of hero images to show
        /// </summary>
        private const int NumberOfHeroImages = 5;

        /// <summary>
        /// The auth enforcement handler.
        /// </summary>
        private readonly IAuthEnforcementHandler _authEnforcementHandler;

        private readonly IDialogService _dialogService;

        /// <summary>
        /// The hero iamges
        /// </summary>
        private ObservableCollection<Photo> _heroImages;

        /// <summary>
        /// The timer for scrolling though hero images
        /// </summary>
        private DispatcherTimer _heroImageScrollTimer;

        /// <summary>
        /// True, if ViewModel is busy
        /// </summary>
        private bool _isBusy;

        /// <summary>
        /// The visibility status of the empty data message.
        /// </summary>
        private bool _isEmptyDataMessageVisible;

        /// <summary>
        /// The visibility status of the status container.
        /// </summary>
        private bool _isStatusContainerVisible;

        /// <summary>
        /// True, if user is signed in. Otherwise, false.
        /// </summary>
        private bool _isUserSignedIn;

        /// <summary>
        /// The navigation facade
        /// </summary>
        private readonly INavigationFacade _navigationFacade;

        /// <summary>
        /// The photo service
        /// </summary>
        private readonly IPhotoService _photoService;

        /// <summary>
        /// The current hero image
        /// </summary>
        private Photo _selectedHeroImage;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoriesViewModel" /> class.
        /// </summary>
        /// <param name="navigationFacade">The navigation facade.</param>
        /// <param name="photoService">The photo service.</param>
        /// <param name="authEnforcementHandler">The auth enforcement handler.</param>
        /// <param name="dialogService">The dialog service</param>
        public CategoriesViewModel(INavigationFacade navigationFacade, IPhotoService photoService,
            IAuthEnforcementHandler authEnforcementHandler, IDialogService dialogService)
        {
            _navigationFacade = navigationFacade;
            _photoService = photoService;
            _authEnforcementHandler = authEnforcementHandler;
            _dialogService = dialogService;

            // Initialize collections.
            HeroImages = new ObservableCollection<Photo>();

            // Initialize commands
            CategorySelectedCommand = new RelayCommand<CategoryPreview>(OnCategorySelected);
            PhotoThumbnailSelectedCommand = new RelayCommand<PhotoThumbnail>(OnPhotoThumbnailSelected);
            HeroImageSelectedCommand = new RelayCommand<Photo>(OnHeroImageSelected);
            GiveGoldCommand = new RelayCommand<Photo>(OnGiveGold);
            UserSelectedCommand = new RelayCommand<User>(OnUserSelected);

            IsUserSignedIn = AppEnvironment.Instance.CurrentUser != null;
        }

        /// <summary>
        /// Gets the category selected command.
        /// </summary>
        public RelayCommand<CategoryPreview> CategorySelectedCommand { get; private set; }

        /// <summary>
        /// Gets or sets the favorite categories.
        /// </summary>
        /// <value>
        /// The favorite categories.
        /// </value>
        public IncrementalLoadingCollection<CategoryPreview> FavoriteCategories { get; set; }

        /// <summary>
        /// Gets give gold command.
        /// </summary>
        public RelayCommand<Photo> GiveGoldCommand { get; private set; }

        /// <summary>
        /// Gets or sets the hero images.
        /// </summary>
        public ObservableCollection<Photo> HeroImages
        {
            get { return _heroImages; }
            set
            {
                if (value != _heroImages)
                {
                    _heroImages = value;
                    NotifyPropertyChanged(nameof(HeroImages));
                }
            }
        }

        /// <summary>
        /// Gets the hero image selected command.
        /// </summary>
        public RelayCommand<Photo> HeroImageSelectedCommand { get; private set; }

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
        /// Gets or sets the visibility of the status message that no
        /// data is available.
        /// </summary>
        public bool IsEmptyDataMessageVisible
        {
            get { return _isEmptyDataMessageVisible; }
            set
            {
                if (value != _isEmptyDataMessageVisible)
                {
                    _isEmptyDataMessageVisible = value;
                    NotifyPropertyChanged(nameof(IsEmptyDataMessageVisible));
                }
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the status container.
        /// </summary>
        public bool IsStatusContainerVisible
        {
            get { return _isStatusContainerVisible; }
            set
            {
                if (value != _isStatusContainerVisible)
                {
                    _isStatusContainerVisible = value;
                    NotifyPropertyChanged(nameof(IsStatusContainerVisible));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indication whether the user is signed in.
        /// </summary>
        public bool IsUserSignedIn
        {
            get { return _isUserSignedIn; }
            set
            {
                if (value != _isUserSignedIn)
                {
                    _isUserSignedIn = value;
                    NotifyPropertyChanged(nameof(IsUserSignedIn));
                }
            }
        }

        /// <summary>
        /// Gets or sets the new categories.
        /// </summary>
        /// <value>
        /// The new categories.
        /// </value>
        public IncrementalLoadingCollection<CategoryPreview> NewCategories { get; set; }

        public RelayCommand<PhotoThumbnail> PhotoThumbnailSelectedCommand { get; private set; }

        /// <summary>
        /// Gets or sets the selected category.
        /// </summary>
        public CategoryPreview SelectedCategoryPreview { get; set; }

        /// <summary>
        /// Gets or sets the hero image.
        /// </summary>
        public Photo SelectedHeroImage
        {
            get { return _selectedHeroImage; }
            set
            {
                if (value != _selectedHeroImage)
                {
                    _selectedHeroImage = value;
                    NotifyPropertyChanged(nameof(SelectedHeroImage));

                    // Reset the flip timer to keep it consistent if a
                    // user changes the photo in between automatic flips.
                    if (_heroImageScrollTimer != null)
                    {
                        _heroImageScrollTimer.Stop();
                        _heroImageScrollTimer.Start();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the top categories.
        /// </summary>
        /// <value>
        /// The top categories.
        /// </value>
        public ObservableCollection<CategoryPreview> TopCategories { get; set; } =
            new ObservableCollection<CategoryPreview>();

        /// <summary>
        /// Gets the user selected command.
        /// </summary>
        public RelayCommand<User> UserSelectedCommand { get; private set; }

        /// <summary>
        /// Loads the state.
        /// </summary>
        public override async Task LoadState()
        {
            await base.LoadState();

            IsBusy = true;
            IsStatusContainerVisible = true;

            try
            {
                TopCategories.Clear();

                // Load hero images
                var heroImages = await _photoService.GetHeroImages(NumberOfHeroImages);
                HeroImages.Clear();
                heroImages.ForEach(h => HeroImages.Add(h));

                // Load categories
                var categories =
                    await _photoService.GetTopCategories(AppEnvironment.Instance.CategoryThumbnailsCount);

                IsEmptyDataMessageVisible = !categories.Any();
                IsStatusContainerVisible = !categories.Any();

                foreach (var c in categories)
                {
                    TopCategories.Add(c);

                    // For UI animation purposes, we wait a little until the next
                    // element is inserted.
                    await Task.Delay(200);
                }
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

        private void OnCategorySelected(CategoryPreview categoryPreview)
        {
            SelectedCategoryPreview = categoryPreview;
            _navigationFacade.NavigateToPhotoStream(categoryPreview);
        }

        private async void OnGiveGold(Photo photo)
        {
            try
            {
                await _authEnforcementHandler.CheckUserAuthentication();
                await _navigationFacade.ShowGiveGoldDialog(photo);
            }
            catch (SignInRequiredException)
            {
                // Swallow exception. User canceled the Sign-in dialog.
            }
            catch (ServiceException)
            {
                await _dialogService.ShowGenericServiceErrorNotification();
            }
        }

        private void OnHeroImageSelected(Photo photo)
        {
            _navigationFacade.NavigateToPhotoDetailsView(photo);
        }

        private void OnPhotoThumbnailSelected(PhotoThumbnail photoThumbnail)
        {
            var categoryPreview = TopCategories.SingleOrDefault(c => c.PhotoThumbnails.Contains(photoThumbnail));

            if (categoryPreview != null)
            {
                _navigationFacade.NavigateToPhotoStream(categoryPreview, photoThumbnail);
            }
        }

        private void OnUserSelected(User user)
        {
            _navigationFacade.NavigateToProfileView(user);
        }

        /// <summary>
        /// Starts the hero image slide show.
        /// </summary>
        public void StartHeroImageSlideShow()
        {
            // Only start slideshow if we were able to get
            // any hero images
            if (HeroImages != null && HeroImages.Any())
            {
                _heroImageScrollTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(7) };
                StartHeroImageSlideshowTimer();
            }
        }

        /// <summary>
        /// Starts a timer which cycles through hero images.
        /// </summary>
        private void StartHeroImageSlideshowTimer()
        {
            _heroImageScrollTimer.Start();

            _heroImageScrollTimer.Tick += (s, e) =>
            {
                var selectedIndex = HeroImages.IndexOf(SelectedHeroImage);
                selectedIndex = (selectedIndex + 1) % HeroImages.Count;

                SelectedHeroImage = HeroImages[selectedIndex];
            };
        }

        /// <summary>
        /// Stops hero image slide show.
        /// </summary>
        public void StopHeroImageSlideShow()
        {
            _heroImageScrollTimer?.Stop();
        }
    }
}