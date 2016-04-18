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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using PhotoSharingApp.Portable.DataContracts;
using PhotoSharingApp.Universal.Extensions;
using PhotoSharingApp.Universal.Models;

namespace PhotoSharingApp.Universal.Services
{
    /// <summary>
    /// This is a dummy service implementation that returns static
    /// data. This data can be used for development purposes in order
    /// to have data available at design-time for layouting. This data
    /// can also be used to simulate fetching data from a real service.
    /// </summary>
    public class PhotoDummyService : IPhotoService
    {
        private readonly Random _random;
        private List<User> _sampleUsers;
        private bool _showError = true;

        /// <summary>
        /// Intialize static sample data for Categories, Users,
        /// Photos, Annotations and Leaderboards for using
        /// dummy service.
        /// </summary>
        public PhotoDummyService()
        {
            _random = new Random();

            InitSampleUsers();
            InitUser();
            InitTopCategories();
            InitCategories();
            InitPhotoStreams();
            InitAnnotations();
            InitLeaderboardData();

            AppEnvironment.Instance.CurrentUser = User;
        }

        /// <summary>
        /// Gets the annotations.
        /// </summary>
        public List<Annotation> Annotations { get; private set; }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        public List<Category> Categories { get; private set; }

        /// <summary>
        /// Gets or sets if error simulation mode
        /// is enabled.
        /// </summary>
        public bool IsErrorSimulationEnabled { get; set; }

        /// <summary>
        /// Gets the leaderboard data.
        /// </summary>
        public Leaderboard LeaderboardData { get; private set; }

        /// <summary>
        /// Gets the photo streams.
        /// </summary>
        public List<PhotoStream> PhotoStreams { get; private set; }

        /// <summary>
        /// Gets the top categories.
        /// </summary>
        public List<CategoryPreview> TopCategories { get; private set; }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        public User User { get; private set; }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <returns>The category.</returns>
        public async Task<Category> CreateCategory(string categoryName)
        {
            await SimulateWaitAndError();

            var categoryPreview = new CategoryPreview
            {
                Id = Guid.NewGuid().ToString(),
                Name = categoryName,
                PhotoThumbnails = new List<PhotoThumbnail>()
            };

            TopCategories.Add(categoryPreview);

            return categoryPreview.ToCategory();
        }

        /// <summary>
        /// Deletes the photo.
        /// </summary>
        /// <param name="photo">The photo.</param>
        public async Task DeletePhoto(Photo photo)
        {
            await SimulateWaitAndError();

            var category = PhotoStreams.FirstOrDefault(c => c.CateogoryId == photo.CategoryId);

            var photoInCategory = category?.Photos.FirstOrDefault(p => p.CategoryId == photo.CategoryId);
            if (photoInCategory != null)
            {
                category.Photos.Remove(photoInCategory);
            }
        }

        /// <summary>
        /// Fulfills gold on the service.
        /// </summary>
        /// <param name="productReceipt">The product receipt.</param>
        /// <returns>The user with an updated gold balance.</returns>
        public async Task<User> FulfillGold(string productReceipt)
        {
            await SimulateWaitAndError();

            User.GoldBalance += 5;

            return User;
        }

        /// <summary>
        /// Gets the available authentication providers.
        /// </summary>
        /// <returns>The authentication providers.</returns>
        public List<MobileServiceAuthenticationProvider> GetAvailableAuthenticationProviders()
        {
            return new List<MobileServiceAuthenticationProvider>();
        }

        /// <summary>
        /// Retrieves all categories without thumbnails.
        /// </summary>
        /// <returns>The categories.</returns>
        public async Task<List<Category>> GetCategories()
        {
            await SimulateWaitAndError();

            return Categories;
        }

        /// <summary>
        /// Gets the configuration data.
        /// </summary>
        /// <returns>The configuration data.</returns>
        public async Task<Config> GetConfig()
        {
            await SimulateWaitAndError();

            return new DefaultConfig();
        }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <returns>The details for the user that is logged in.</returns>
        public async Task<User> GetCurrentUser()
        {
            await SimulateWaitAndError();

            return User;
        }

        /// <summary>
        /// Retrieves hero images.
        /// </summary>
        /// <param name="count">Number of hero images.</param>
        /// <returns>The images.</returns>
        public async Task<List<Photo>> GetHeroImages(int count)
        {
            await SimulateWaitAndError();

            var category = await GetTopCategories(0, null);
            var stream = await GetPhotosForCategoryId(category.First()?.Id);

            return stream.Items.Take(count).ToList();
        }

        /// <summary>
        /// Gets the leaderboard statistical data
        /// </summary>
        /// <returns>The leaderboard</returns>
        public async Task<Leaderboard> GetLeaderboardData(int mostGoldCategoriesCount, int mostGoldPhotosCount,
            int mostGoldUsersCount, int mostGivingUsersCount)
        {
            await SimulateWaitAndError();

            return LeaderboardData;
        }

        /// <summary>
        /// Gets the photo details for the given photo id.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <returns>The photo.</returns>
        public async Task<Photo> GetPhotoDetails(string photoId)
        {
            await SimulateWaitAndError();

            var photo = PhotoStreams.SelectMany(s => s.Photos)
                .FirstOrDefault(p => p.Id == photoId);

            photo.Annotations = new ObservableCollection<Annotation>(Annotations);

            return photo;
        }

        /// <summary>
        /// Gets photos for the given category id.
        /// </summary>
        /// <param name="categoryId">The identifier.</param>
        /// <param name="continuationToken">Optional. The continuation token. By default, null.</param>
        /// <returns>The photos.</returns>
        public async Task<PagedResponse<Photo>> GetPhotosForCategoryId(string categoryId,
            string continuationToken = null)
        {
            await SimulateWaitAndError();

            var stream = PhotoStreams.FirstOrDefault(s => s.CateogoryId == categoryId);

            if (stream != null)
            {
                // Category Id found, return photos
                return new PagedResponse<Photo>
                {
                    ContinuationToken = null,
                    Items = stream.Photos
                };
            }

            // Category Id was not found, return empty list.
            return new PagedResponse<Photo>
            {
                ContinuationToken = null,
                Items = new List<Photo>()
            };
        }

        /// <summary>
        /// Gets photos uploaded by the current user.
        /// </summary>
        /// <param name="continuationToken">Optional. The continuation token. By default, null.</param>
        /// <returns>The photos.</returns>
        public async Task<PagedResponse<Photo>> GetPhotosForCurrentUser(string continuationToken = null)
        {
            await SimulateWaitAndError();

            var stream = PhotoStreams.FirstOrDefault();

            if (stream != null)
            {
                // Found the first photo stream, return photos
                return new PagedResponse<Photo>
                {
                    ContinuationToken = null,
                    Items = stream.Photos
                };
            }

            // First photo stream was null, return empty list.
            return new PagedResponse<Photo>
            {
                ContinuationToken = null,
                Items = new List<Photo>()
            };
        }

        /// <summary>
        /// Gets photos uploaded by the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="continuationToken">Optional. The continuation token. By default, null.</param>
        /// <returns>The photos.</returns>
        public Task<PagedResponse<Photo>> GetPhotosForUser(User user, string continuationToken = null)
        {
            return GetPhotosForCurrentUser(continuationToken);
        }

        /// <summary>
        /// Retrieves top categories with thumbnails.
        /// </summary>
        /// <param name="categoryThumbnailsCount">The number of thumbnails per each category.</param>
        /// <param name="continuationToken">Optional. The continuation token. By default, null.</param>
        /// <returns>The category list.</returns>
        public async Task<List<CategoryPreview>> GetTopCategories(int categoryThumbnailsCount, string continuationToken = null)
        {
            await SimulateWaitAndError();

            if (!string.IsNullOrEmpty(continuationToken))
            {
                return new List<CategoryPreview>();
            }

            return TopCategories.Select(c => new CategoryPreview
            {
                Id = c.Id,
                Name = c.Name,
                PhotoThumbnails = c.PhotoThumbnails.Take(categoryThumbnailsCount).ToList()
            }).ToList();
        }

        private TimeSpan GetWaitTime()
        {
            return TimeSpan.Zero;
        }

        private void InitAnnotations()
        {
            Annotations = new List<Annotation>();

            var comment = new Annotation
            {
                CreatedTime = DateTime.Now.AddMinutes(-2),
                From = new User
                {
                    ProfilePictureUrl = "https://canaryappstorage.blob.core.windows.net/dummy-container/a2_tn.jpg"
                },
                Text = "I love it!",
                Id = Guid.NewGuid().ToString(),
                GoldCount = 1
            };
            Annotations.Add(comment);

            var secondComment = new Annotation
            {
                CreatedTime = DateTime.Now.AddHours(-1),
                From = new User
                {
                    ProfilePictureUrl = "https://canaryappstorage.blob.core.windows.net/dummy-container/a7_tn.jpg"
                },
                Text = "Wow! :-)",
                Id = Guid.NewGuid().ToString(),
                GoldCount = 1
            };
            Annotations.Add(secondComment);
        }

        private void InitCategories()
        {
            Categories = TopCategories.Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
        }

        private void InitLeaderboardData()
        {
            var allPhotos = PhotoStreams.SelectMany(s => s.Photos).ToList();
            var users = _sampleUsers;

            var leaderboard = new Leaderboard
            {
                MostGoldCategories = new List<LeaderboardEntry<Category>>
                {
                    new LeaderboardEntry<Category>
                    {
                        Model = Categories[_random.Next(Categories.Count)],
                        Rank = 1,
                        Value = 94
                    },
                    new LeaderboardEntry<Category>
                    {
                        Model = Categories[_random.Next(Categories.Count)],
                        Rank = 2,
                        Value = 79
                    },
                    new LeaderboardEntry<Category>
                    {
                        Model = Categories[_random.Next(Categories.Count)],
                        Rank = 3,
                        Value = 47
                    }
                },
                MostGoldPhotos = new List<LeaderboardEntry<Photo>>
                {
                    new LeaderboardEntry<Photo>
                    {
                        Model = allPhotos[_random.Next(allPhotos.Count)],
                        Rank = 1,
                        Value = 38
                    },
                    new LeaderboardEntry<Photo>
                    {
                        Model = allPhotos[_random.Next(allPhotos.Count)],
                        Rank = 2,
                        Value = 29
                    },
                    new LeaderboardEntry<Photo>
                    {
                        Model = allPhotos[_random.Next(allPhotos.Count)],
                        Rank = 3,
                        Value = 21
                    }
                },
                MostGoldUsers = new List<LeaderboardEntry<User>>
                {
                    new LeaderboardEntry<User>
                    {
                        Model = users[_random.Next(users.Count)],
                        Rank = 1,
                        Value = 68
                    },
                    new LeaderboardEntry<User>
                    {
                        Model = users[_random.Next(users.Count)],
                        Rank = 2,
                        Value = 54
                    },
                    new LeaderboardEntry<User>
                    {
                        Model = users[_random.Next(users.Count)],
                        Rank = 3,
                        Value = 37
                    }
                },
                MostGivingUsers = new List<LeaderboardEntry<User>>
                {
                    new LeaderboardEntry<User>
                    {
                        Model = users[_random.Next(users.Count)],
                        Rank = 1,
                        Value = 34
                    },
                    new LeaderboardEntry<User>
                    {
                        Model = users[_random.Next(users.Count)],
                        Rank = 2,
                        Value = 27
                    },
                    new LeaderboardEntry<User>
                    {
                        Model = users[_random.Next(users.Count)],
                        Rank = 3,
                        Value = 17
                    }
                }
            };

            LeaderboardData = leaderboard;
        }

        private void InitPhotoStreams()
        {
            PhotoStreams = new List<PhotoStream>();

            var captions = new[]
            {
                "Yay!",
                "Having a good day!",
                "Enjoying... :)",
                "Isn't that a beautiful shot?",
                "I like it this way!",
                "Love it!",
                "Look at this..."
            };

            var users = _sampleUsers;
            var userIndex = 0;

            PhotoStreams.AddRange(TopCategories.Select(c =>
            {
                var createdAtMinutes = 45;

                return new PhotoStream
                {
                    CateogoryId = c.Id,
                    Photos = c.PhotoThumbnails.Select(thumb => new Photo
                    {
                        Id = Guid.NewGuid().ToString(),
                        Caption = captions[_random.Next(captions.Length)],
                        ThumbnailUrl = thumb.ImageUrl,
                        HighResolutionUrl = thumb.ImageUrl.Replace("_tn", ""),
                        StandardUrl = thumb.ImageUrl.Replace("_tn", ""),
                        CreatedAt = DateTime.Now.AddMinutes(createdAtMinutes -= 55),
                        User = users[userIndex++%users.Count],
                        GoldCount = _random.Next(10),
                        NumberOfAnnotations = 2,
                        CategoryId = c.Id,
                        CategoryName = c.Name,
                        Status = PhotoStatus.Active
                    })
                        .OrderByDescending(p => p.CreatedAt)
                        .ToList()
                };
            }));
        }

        private void InitSampleUsers()
        {
            _sampleUsers = new List<User>();
            for (var i = 1; i <= 11; i++)
            {
                _sampleUsers.Add(new User
                {
                    ProfilePictureUrl =
                        $"https://canaryappstorage.blob.core.windows.net/dummy-container/a{i}_tn.jpg",
                    UserId = Guid.NewGuid().ToString()
                });
            }
        }

        private void InitTopCategories()
        {
            // My shoes
            var shoesThumbnails = new List<PhotoThumbnail>();
            for (var i = 1; i <= 8; i++)
            {
                shoesThumbnails.Add(new PhotoThumbnail
                {
                    ImageUrl = $"https://canaryappstorage.blob.core.windows.net/dummy-container/shoes{i}_tn.jpg"
                });
            }

            // My coffee
            var coffeeThumbnails = new List<PhotoThumbnail>();
            for (var i = 1; i <= 11; i++)
            {
                coffeeThumbnails.Add(new PhotoThumbnail
                {
                    ImageUrl = $"https://canaryappstorage.blob.core.windows.net/dummy-container/coffee{i}_tn.jpg"
                });
            }

            // My city
            var cityThumbnails = new List<PhotoThumbnail>();
            for (var i = 7; i <= 55; i++)
            {
                cityThumbnails.Add(new PhotoThumbnail
                {
                    ImageUrl = $"https://canaryappstorage.blob.core.windows.net/dummy-container/city{i}_tn.jpg"
                });
            }

            // My food
            var foodThumbnails = new List<PhotoThumbnail>();
            for (var i = 1; i <= 43; i++)
            {
                foodThumbnails.Add(new PhotoThumbnail
                {
                    ImageUrl = $"https://canaryappstorage.blob.core.windows.net/dummy-container/food{i}_tn.jpg"
                });
            }

            var categoryList = new List<CategoryPreview>
            {
                new CategoryPreview
                {
                    Name = "Food",
                    PhotoThumbnails = foodThumbnails,
                    Id = Guid.NewGuid().ToString()
                },
                new CategoryPreview
                {
                    Name = "City",
                    PhotoThumbnails = cityThumbnails,
                    Id = Guid.NewGuid().ToString()
                },
                new CategoryPreview
                {
                    Name = "Coffee",
                    PhotoThumbnails = coffeeThumbnails,
                    Id = Guid.NewGuid().ToString()
                },
                new CategoryPreview
                {
                    Name = "Shoes",
                    PhotoThumbnails = shoesThumbnails,
                    Id = Guid.NewGuid().ToString()
                }
            };

            TopCategories = categoryList;
        }

        private void InitUser()
        {
            User = new User
            {
                ProfilePictureUrl = "https://canaryappstorage.blob.core.windows.net/dummy-container/a11_tn.jpg",
                GoldBalance = 50,
                UserId = "a71a39ee-8d81-4d1a-8209-903a2444acf5"
            };
        }

        /// <summary>
        /// Posts the annotation.
        /// </summary>
        /// <param name="photo">The photo.</param>
        /// <param name="annotationText">The text.</param>
        /// <param name="goldCount">The amount of gold being given.</param>
        /// <returns>The annotation including the id.</returns>
        public async Task<Annotation> PostAnnotation(Photo photo, string annotationText, int goldCount)
        {
            await SimulateWaitAndError();

            var annotation = new Annotation
            {
                CreatedTime = DateTime.Now,
                Id = Guid.NewGuid().ToString(),
                Text = annotationText,
                From = AppEnvironment.Instance.CurrentUser,
                GoldCount = goldCount
            };

            Annotations.Add(annotation);
            photo.NumberOfAnnotations++;
            photo.GoldCount += annotation.GoldCount;

            if (AppEnvironment.Instance.CurrentUser != null)
            {
                AppEnvironment.Instance.CurrentUser.GoldBalance -= annotation.GoldCount;
            }

            return annotation;
        }

        /// <summary>
        /// Removes the Annotation.
        /// </summary>
        /// <param name="annotation">The Annotation.</param>
        public async Task RemoveAnnotation(Annotation annotation)
        {
            await SimulateWaitAndError();

            var commentFromList = Annotations.SingleOrDefault(c => c.Id.Equals(annotation.Id));
            if (commentFromList != null)
            {
                Annotations.Remove(commentFromList);
            }
        }

        /// <summary>
        /// Reports the Annotation as inappropriate.
        /// </summary>
        /// <param name="annotation">The Annotation.</param>
        public async Task ReportAnnotation(Annotation annotation)
        {
            await SimulateWaitAndError();
        }

        /// <summary>
        /// Reports the photo as inappropriate.
        /// </summary>
        /// <param name="photo">The photo.</param>
        /// <param name="reportReason">The reason for the report.</param>
        public async Task ReportPhoto(Photo photo, ReportReason reportReason)
        {
            await SimulateWaitAndError();
        }

        /// <summary>
        /// Checks if credentials are available and not expired,
        /// which are then used to restore the sign in status.
        /// The user is not actively taken to a login page.
        /// </summary>
        public Task RestoreSignInStatusAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Signs the user in.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public Task SignInAsync(MobileServiceAuthenticationProvider provider)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Signs the user out.
        /// </summary>
        public Task SignOutAsync()
        {
            AppEnvironment.Instance.CurrentUser = null;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Simulates an error in every second call.
        /// Can be disabled by setting IsErrorSimulationEnabled variable to false.
        /// </summary>
        private async Task SimulateWaitAndError()
        {
            if (IsErrorSimulationEnabled)
            {
                // We are only throwing an exception in every second
                // service call to keep the ability to 
                // navigate through the app to different pages 
                // (photo stream, photo details).
                _showError = !_showError;

                if (_showError)
                {
                    throw new ServiceException("Test Exception");
                }
            }

            await Task.Delay(GetWaitTime());
        }

        /// <summary>
        /// Updates the photo.
        /// </summary>
        /// <param name="photo">The photo.</param>
        public async Task UpdatePhoto(Photo photo)
        {
            await SimulateWaitAndError();
        }

        /// <summary>
        /// Updates the current user's profile photo
        /// </summary>
        /// <param name="photo">The new profile photo</param>
        /// <returns>The updated user</returns>
        public async Task<User> UpdateUserProfilePhoto(Photo photo)
        {
            await SimulateWaitAndError();

            AppEnvironment.Instance.CurrentUser.ProfilePictureUrl = photo.ThumbnailUrl;

            return AppEnvironment.Instance.CurrentUser;
        }

        /// <summary>
        /// Uploads the photo.
        /// </summary>
        /// <param name="stream">The memory stream.</param>
        /// <param name="localPath">The local path.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="categoryId">The id of the assocaited category.</param>
        /// <returns>The uploaded photo.</returns>
        public async Task<Photo> UploadPhoto(Stream stream, string localPath, string caption, string categoryId)
        {
            await SimulateWaitAndError();

            var photo = new Photo
            {
                Caption = caption,
                ThumbnailUrl = localPath,
                HighResolutionUrl = localPath,
                StandardUrl = localPath,
                CreatedAt = DateTime.Now,
                User = AppEnvironment.Instance.CurrentUser
            };

            PhotoStreams.FirstOrDefault(s => s.CateogoryId == categoryId).Photos.Insert(0, photo);

            AppEnvironment.Instance.CurrentUser.GoldBalance++;

            return photo;
        }
    }
}