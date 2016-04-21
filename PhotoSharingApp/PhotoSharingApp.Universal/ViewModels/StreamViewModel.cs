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
using System.Threading.Tasks;
using PhotoSharingApp.Portable.DataContracts;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.ComponentModel;
using PhotoSharingApp.Universal.Extensions;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Views;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The ViewModel for the photo stream view.
    /// </summary>
    public class StreamViewModel : ViewModelBase
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

        private bool _isBusy;

        /// <summary>
        /// The navigation facade
        /// </summary>
        private readonly INavigationFacade _navigationFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamViewModel" /> class.
        /// </summary>
        /// <param name="navigationFacade">The navigation facade.</param>
        /// <param name="photoService">The photo service.</param>
        /// <param name="authEnforcementHandler">the authentication enforcement handler</param>
        /// <param name="dialogService">The dialog service.</param>
        public StreamViewModel(INavigationFacade navigationFacade, IPhotoService photoService,
            IAuthEnforcementHandler authEnforcementHandler, IDialogService dialogService)
        {
            _navigationFacade = navigationFacade;
            _authEnforcementHandler = authEnforcementHandler;
            _dialogService = dialogService;

            Photos = new IncrementalLoadingCollection<Photo>(s =>
            {
                Func<Task<PagedResponse<Photo>>> f = async () =>
                {
                    var stream = await photoService.GetPhotosForCategoryId(Category.Id, s);

                    if (SelectedPhotoThumbnail != null
                        && SelectedPhoto == null)
                    {
                        SelectedPhoto = stream.Items.FindPhotoForThumbnail(SelectedPhotoThumbnail);
                    }

                    return stream;
                };

                return f();
            }, async () => await _dialogService.ShowGenericServiceErrorNotification());

            // Initialize commands
            RefreshCommand = new RelayCommand(OnRefresh);
            GotoCameraCommand = new RelayCommand(OnGotoCamera);
            GiveGoldCommand = new RelayCommand<Photo>(OnGiveGold);
            PhotoSelectedCommand = new RelayCommand<Photo>(OnPhotoSelected);
            ContributeCommand = new RelayCommand(OnGotoCamera);
            UserSelectedCommand = new RelayCommand<User>(OnUserSelected);
        }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
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
        /// Gets the contribute command.
        /// </summary>
        public RelayCommand ContributeCommand { get; private set; }

        /// <summary>
        /// Gets give gold command.
        /// </summary>
        public RelayCommand<Photo> GiveGoldCommand { get; private set; }

        /// <summary>
        /// Gets the goto camera command.
        /// </summary>
        public RelayCommand GotoCameraCommand { get; private set; }

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
        /// Gets photo selected command.
        /// </summary>
        public RelayCommand<Photo> PhotoSelectedCommand { get; private set; }

        /// <summary>
        /// Gets the refresh command.
        /// </summary>
        public RelayCommand RefreshCommand { get; private set; }

        /// <summary>
        /// Gets or sets the selected photo.
        /// </summary>
        public Photo SelectedPhoto { get; set; }

        /// <summary>
        /// Gets or sets the selected photo thumbnail which
        /// is used to determine the scroll position of the
        /// photo stream.
        /// </summary>
        public PhotoThumbnail SelectedPhotoThumbnail { get; set; }

        /// <summary>
        /// Gets the user selected command.
        /// </summary>
        public RelayCommand<User> UserSelectedCommand { get; private set; }

        /// <summary>
        /// Loads the state.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public async Task LoadState(StreamViewModelArgs args)
        {
            Category = args.Category;

            await base.LoadState();
        }

        /// <summary>
        /// Loads the state.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public async Task LoadState(StreamViewModelThumbnailArgs args)
        {
            Category = args.Category;
            SelectedPhotoThumbnail = args.PhotoThumbnail;

            await base.LoadState();
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
                // User canceled the Sign-in dialog.
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

        /// <summary>
        /// Action to take when a photo has been selected
        /// </summary>
        /// <param name="photo">The photo.</param>
        private void OnPhotoSelected(Photo photo)
        {
            SelectedPhoto = photo;
            _navigationFacade.NavigateToPhotoDetailsView(Category, photo);
        }

        private async void OnRefresh()
        {
            Photos.Clear();
            await Photos.Refresh();
        }

        /// <summary>
        /// Action to take when a user has been selected
        /// </summary>
        /// <param name="user">The user.</param>
        private void OnUserSelected(User user)
        {
            _navigationFacade.NavigateToProfileView(user);
        }
    }
}