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

using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using PhotoSharingApp.Universal.Models;

namespace PhotoSharingApp.Universal.Tests.Models
{
    [TestClass]
    public class CategoryMatchFinderTests
    {
        private CategoryMatchFinder _categoryMatchFinder;

        [TestMethod]
        public void GetMatchingCategoriesPartialLowercaseTest()
        {
            // Arrange
            var categoryName = "ho";

            var categories = new[]
            {
                "Dog",
                "House"
            }.Select(c => new Category { Name = c });

            // Act
            var result = _categoryMatchFinder.SearchCategories(categoryName, categories);

            // Verify
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void IsCategoryPartOfListLowercaseTest()
        {
            // Arrange
            var categoryName = "dog";

            var categories = new[]
            {
                "Dog",
                "Cat"
            }.Select(c => new Category { Name = c });

            // Act
            var result = _categoryMatchFinder.IsCategoryPartOfList(categoryName, categories);

            // Verify
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsCategoryPartOfListNotExistingTest()
        {
            // Arrange
            var categoryName = "Test";

            var categories = new[]
            {
                "Dog",
                "Cat"
            }.Select(c => new Category { Name = c });

            // Act
            var result = _categoryMatchFinder.IsCategoryPartOfList(categoryName, categories);

            // Verify
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SearchCategoryEmptyStringTest()
        {
            // Arrange
            var categoryName = string.Empty;

            var categories = new[]
            {
                "Dog",
                "House"
            }.Select(c => new Category { Name = c });

            // Act
            var result = _categoryMatchFinder.SearchCategories(categoryName, categories);

            // Verify
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void SearchCategoryNullTest()
        {
            // Arrange
            string categoryName = null;

            var categories = new[]
            {
                "Dog",
                "House"
            }.Select(c => new Category { Name = c });

            // Act
            var result = _categoryMatchFinder.SearchCategories(categoryName, categories);

            // Verify
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void SearchCategoryPartialLowercaseTest()
        {
            // Arrange
            var categoryName = "ho";

            var categories = new[]
            {
                "Dog",
                "House"
            }.Select(c => new Category { Name = c });

            // Act
            var result = _categoryMatchFinder.SearchCategories(categoryName, categories);

            // Verify
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void SearchCategoryTest()
        {
            // Arrange
            var categoryName = "House";

            var categories = new[]
            {
                "Dog",
                "House"
            }.Select(c => new Category { Name = c });

            // Act
            var result = _categoryMatchFinder.SearchCategories(categoryName, categories);

            // Verify
            Assert.AreEqual(1, result.Count());
        }

        [TestInitialize]
        public void TestInit()
        {
            _categoryMatchFinder = new CategoryMatchFinder();
        }

        [TestMethod]
        public void ValidateCategoryAcceptanceCriteriaLowercaseTest()
        {
            // Arrange
            var categoryName = "dog";

            var categories = new[]
            {
                "Dog",
                "Cat"
            }.Select(c => new Category { Name = c });

            // Act & Verify
            Assert.ThrowsException<CategoryMatchedException>(
                () => _categoryMatchFinder.ValidateCategoryAcceptanceCriteria(categoryName, categories));
        }

        [TestMethod]
        public void ValidateCategoryAcceptanceCriteriaNotExistingTest()
        {
            // Arrange
            var categoryName = "Test";

            var categories = new[]
            {
                "Dog",
                "Cat"
            }.Select(c => new Category { Name = c });

            // Act & Verify
            _categoryMatchFinder.ValidateCategoryAcceptanceCriteria(categoryName, categories);
        }
    }
}