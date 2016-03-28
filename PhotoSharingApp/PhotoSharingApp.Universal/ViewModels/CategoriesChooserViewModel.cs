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
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Views;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The ViewModel for the category chooser view.
    /// </summary>
    public class CategoriesChooserViewModel : ViewModelBase
    {
        private List<Category> _categories;
        private readonly CategoryMatchFinder _categoryMatchFinder;
        private readonly IDialogService _dialogService;
        private bool _isBusy;
        private readonly IPhotoService _photoService;
        private string _searchText;
        private Category _selectedCategory;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="photoService">The photo service.</param>
        /// <param name="dialogService">The dialog service.</param>
        public CategoriesChooserViewModel(IPhotoService photoService, IDialogService dialogService)
        {
            _photoService = photoService;
            _dialogService = dialogService;
            _categoryMatchFinder = new CategoryMatchFinder();

            // Initialize lists
            Categories = new List<Category>();

            // Initialize commands
            AddCategoryCommand = new RelayCommand(OnAddCategory, () => CanAddCategory);
        }

        /// <summary>
        /// Gets the add category command.
        /// </summary>
        public RelayCommand AddCategoryCommand { get; }

        /// <summary>
        /// Returns whether the user is able to add the
        /// category name entered specified in the search box.
        /// </summary>
        public bool CanAddCategory
        {
            get
            {
                var category = new Category
                {
                    Name = SearchText
                };

                var isDuplicate = _categoryMatchFinder.IsCategoryPartOfList(SearchText, Categories);

                return !category.HasErrors
                       && !isDuplicate;
            }
        }

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        private List<Category> Categories
        {
            get { return _categories; }
            set
            {
                if (value != _categories)
                {
                    _categories = value;
                    RefreshFilteredList();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filtered categories.
        /// </summary>
        public ObservableCollection<Category> FilteredCategories { get; set; } = new ObservableCollection<Category>();

        /// <summary>
        /// Gets or sets a status indicating if work is in progress.
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
        /// Gets or sets the search text.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (value != _searchText)
                {
                    _searchText = value;
                    NotifyPropertyChanged(nameof(SearchText));

                    RefreshFilteredList();

                    AddCategoryCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// The selected category.
        /// Preferrable, UI validation.
        /// </summary>
        public Category SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                if (value != _selectedCategory)
                {
                    _selectedCategory = value;
                    NotifyPropertyChanged(nameof(SelectedCategory));
                }
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

                Categories = await _photoService.GetCategories();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnAddCategory()
        {
            try
            {
                // Ask user if category should be created.
                var confirmationResult = await
                    _dialogService.ShowYesNoNotification("CreateCategoryRequestConfirmation_Message",
                        "CreateCategoryRequestConfirmation_Title");

                if (confirmationResult)
                {
                    IsBusy = true;

                    // Validate category name.
                    _categoryMatchFinder.ValidateCategoryAcceptanceCriteria(SearchText, Categories);
                    var category = await _photoService.CreateCategory(SearchText);

                    // The created category needs to be added to the view
                    // and pre-selected so that the user can continue immediately.
                    Categories.Add(category);
                    RefreshFilteredList(category);
                    SelectedCategory = category;
                }
            }
            catch (CategoryMatchedException)
            {
                await _dialogService.ShowNotification("CategoryAlreadyExists_Message", "CategoryAlreadyExists_Title");
            }
            catch (Exception)
            {
                await _dialogService.ShowGenericServiceErrorNotification();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void RefreshFilteredList(Category category = null)
        {
            FilteredCategories.Clear();

            // Filter only the specific Category
            if (category != null)
            {
                FilteredCategories.Add(category);
            }
            else
            {
                _categoryMatchFinder.SearchCategories(SearchText, Categories)
                    .ForEach(c => FilteredCategories.Add(c));
            }
        }
    }
}