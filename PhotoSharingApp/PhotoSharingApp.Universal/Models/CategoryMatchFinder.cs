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
using System.Linq;
using PhotoSharingApp.Universal.Extensions;

namespace PhotoSharingApp.Universal.Models
{
    /// <summary>
    /// Helper class for finding matches of category names.
    /// </summary>
    public class CategoryMatchFinder
    {
        /// <summary>
        /// Determines which categories are matching the given category name.
        /// </summary>
        /// <param name="categoryName">The category name to match.</param>
        /// <param name="categories">The list of categories.</param>
        /// <returns>The matching categories.</returns>
        public IEnumerable<Category> GetMatchingCategories(string categoryName, IEnumerable<Category> categories)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return new List<Category>();
            }

            return categories
                .Where(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
        }


        /// <summary>
        /// Determines if the given category is part of the given list.
        /// </summary>
        /// <param name="categoryName">The category name to match.</param>
        /// <param name="categories">The list that is matched against.</param>
        /// <returns>True, if category is part of the list. Otherwise, false.</returns>
        public bool IsCategoryPartOfList(string categoryName, IEnumerable<Category> categories)
        {
            return GetMatchingCategories(categoryName, categories).Any();
        }

        /// <summary>
        /// Searches for a category name within a set of given categories.
        /// </summary>
        /// <param name="categoryName">The category name to search for.</param>
        /// <param name="categories">The list of categories.</param>
        /// <returns>The found categories.</returns>
        public IEnumerable<Category> SearchCategories(string categoryName, IEnumerable<Category> categories)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return categories;
            }

            return categories
                .Where(c => c.Name.Contains(categoryName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines if the given category is part of the give list.
        /// </summary>
        /// <param name="categoryName">The category name to match.</param>
        /// <param name="categories">The list that is matched against.</param>
        /// <exception cref="CategoryMatchedException">
        /// Thrown when the given category
        /// is part of the list.
        /// </exception>
        public void ValidateCategoryAcceptanceCriteria(string categoryName, IEnumerable<Category> categories)
        {
            if (IsCategoryPartOfList(categoryName, categories))
            {
                throw new CategoryMatchedException();
            }
        }
    }
}