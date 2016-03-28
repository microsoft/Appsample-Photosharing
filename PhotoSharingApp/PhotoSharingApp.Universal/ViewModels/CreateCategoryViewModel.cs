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
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Views;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// ViewModel for view that lets the user enter details
    /// for a new category and kicks off the creation process.
    /// </summary>
    public class CreateCategoryViewModel : ViewModelBase
    {
        private List<Category> _categories;
        private Category _category;
        private readonly CategoryMatchFinder _categoryMatchFinder;
        private readonly IDialogService _dialogService;
        private bool _isBusy;
        private readonly IPhotoService _photoService;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="photoService">The photo service.</param>
        /// <param name="dialogService">The dialog service.</param>
        public CreateCategoryViewModel(IPhotoService photoService, IDialogService dialogService)
        {
            _photoService = photoService;
            _dialogService = dialogService;
            _categoryMatchFinder = new CategoryMatchFinder();

            // Initialize lists
            _categories = new List<Category>();

            // We use the Category data model as it
            // handles input validation.
            Category = new Category();

            // We need to be always up-to-date regarding input validation,
            // lets register for model changes
            Category.PropertyChanged += Category_PropertyChanged;
        }

        /// <summary>
        /// Notifies if user can proceed with creating the
        /// category.
        /// </summary>
        public bool CanProceed
        {
            get
            {
                // User can only proceed if there is no
                // data loading going on AND if input validation
                // of data model is fine.
                return !IsBusy
                       && !Category.HasErrors;
            }
        }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
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
        /// Gets or sets the busy state.
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
                    NotifyPropertyChanged(nameof(CanProceed));
                }
            }
        }

        private void Category_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(CanProceed));
        }

        /// <summary>
        /// Starts creating a new category and handles
        /// error messaging.
        /// </summary>
        /// <returns>True, if creation succeeded. Otherwise, false.</returns>
        public async Task<bool> CreateCategory()
        {
            try
            {
                IsBusy = true;
                _categoryMatchFinder.ValidateCategoryAcceptanceCriteria(Category.Name, _categories);
                await _photoService.CreateCategory(Category.Name);

                return true;
            }
            catch (CategoryMatchedException)
            {
                await _dialogService.ShowNotification("CategoryAlreadyExists_Message", "CategoryAlreadyExists_Title");
                return false;
            }
            catch (Exception)
            {
                await _dialogService.ShowGenericServiceErrorNotification();
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Loads the state.
        /// </summary>
        public override async Task LoadState()
        {
            await base.LoadState();

            try
            {
                IsBusy = true;

                _categories = await _photoService.GetCategories();
            }
            catch (ServiceException)
            {
                await _dialogService.ShowNotification("ServiceError_Message", "ServiceError_Title");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}