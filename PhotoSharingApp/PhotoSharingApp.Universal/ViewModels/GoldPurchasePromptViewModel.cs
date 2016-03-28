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
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Store;
using PhotoSharingApp.Universal.Views;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The ViewModel for the gold purchase prompt view.
    /// </summary>
    public class GoldPurchasePromptViewModel : ViewModelBase
    {
        private User _currentUser;
        private readonly IDialogService _dialogService;
        private readonly ILicensingFacade _licensingFacade;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="licensingFacade"></param>
        /// <param name="dialogService">The dialog service.</param>
        public GoldPurchasePromptViewModel(ILicensingFacade licensingFacade,
            IDialogService dialogService)
        {
            _licensingFacade = licensingFacade;
            _dialogService = dialogService;

            // Initialize commands
            BuyGoldCommand = new RelayCommand(OnBuyGold);

            // Get current user as UI will bind directly to it.
            CurrentUser = AppEnvironment.Instance.CurrentUser;
        }

        /// <summary>
        /// Gets the buy gold command.
        /// </summary>
        public RelayCommand BuyGoldCommand { get; }

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        public User CurrentUser
        {
            get { return _currentUser; }
            set
            {
                if (value != _currentUser)
                {
                    _currentUser = value;
                    NotifyPropertyChanged(nameof(CurrentUser));
                }
            }
        }

        private async void OnBuyGold()
        {
            try
            {
                await _licensingFacade.PurchaseGold(InAppPurchases.Gold);
                await _dialogService.ShowNotification("PurchaseSuccess_Message", "PurchaseSuccess_Title");
            }
            catch (SignInRequiredException)
            {
                // User canceled the Sign-in dialog.
            }
            catch (Exception)
            {
                await _dialogService.ShowNotification("PurchaseError_Message", "PurchaseError_Title");
            }
        }
    }
}