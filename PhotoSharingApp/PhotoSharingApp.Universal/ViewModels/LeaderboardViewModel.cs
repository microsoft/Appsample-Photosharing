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

using System.Threading.Tasks;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Views;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// ViewModel for Leaderboards view.
    /// </summary>
    public class LeaderboardViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;

        private bool _isBusy;

        private Leaderboard _leaderboard;

        /// <summary>
        /// The navigation facade
        /// </summary>
        private readonly INavigationFacade _navigationFacade;

        /// <summary>
        /// The photo service
        /// </summary>
        private readonly IPhotoService _photoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardViewModel" /> class.
        /// </summary>
        /// <param name="navigationFacade">The navigation facade.</param>
        /// <param name="photoService">The photo service.</param>
        /// <param name="dialogService">The dialog service.</param>
        public LeaderboardViewModel(INavigationFacade navigationFacade, IPhotoService photoService,
            IDialogService dialogService)
        {
            _navigationFacade = navigationFacade;
            _photoService = photoService;
            _dialogService = dialogService;

            PhotoSelectedCommand = new RelayCommand<Photo>(OnPhotoSelected);
            CategorySelectedCommand = new RelayCommand<Category>(OnCategorySelected);
            UserSelectedCommand = new RelayCommand<User>(OnUserSelected);
        }

        /// <summary>
        /// Gets photo selected command.
        /// </summary>
        public RelayCommand<Category> CategorySelectedCommand { get; private set; }

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
        /// The leaderboard data
        /// </summary>
        public Leaderboard Leaderboard
        {
            get { return _leaderboard; }
            private set
            {
                if (value != _leaderboard)
                {
                    _leaderboard = value;
                    NotifyPropertyChanged(nameof(Leaderboard));
                }
            }
        }

        /// <summary>
        /// Gets photo selected command.
        /// </summary>
        public RelayCommand<Photo> PhotoSelectedCommand { get; private set; }

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

            try
            {
                Leaderboard = await _photoService.GetLeaderboardData(5, 5, 5, 5);
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
        /// Action to take when a photo has been selected
        /// </summary>
        /// <param name="category">The category.</param>
        private void OnCategorySelected(Category category)
        {
            _navigationFacade.NavigateToPhotoStream(category);
        }

        /// <summary>
        /// Action to take when a photo has been selected
        /// </summary>
        /// <param name="photo">The photo.</param>
        private void OnPhotoSelected(Photo photo)
        {
            _navigationFacade.NavigateToPhotoDetailsView(photo);
        }

        /// <summary>
        /// Action to take when a user has been selected.
        /// </summary>
        /// <param name="user">The user.</param>
        private void OnUserSelected(User user)
        {
            _navigationFacade.NavigateToProfileView(user);
        }
    }
}