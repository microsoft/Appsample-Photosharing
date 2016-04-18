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
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using PhotoSharingApp.AppService.Shared.Context;
using PhotoSharingApp.AppService.Shared.Models.DocumentDB;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Shared.Repositories
{
    /// <summary>
    /// The DocumentDB data layer class.
    /// </summary>
    public class DocumentDbRepository : IRepository
    {
        private const int PhotoStreamPageSize = 100;
        private const string SystemUserId = "ffffffff-ffff-ffff-ffff-ffffffffffff";

        // Stored Procedures
        private const string TransferGoldStoredProcedureScriptFileName = @"Models\DocumentDB\js\transferGoldStoredProcedure.js";
        private const string TransferGoldStoredProcedureId = "transferGold";
        private const string GetRecentPhotosForCategoriesStoredProcedureScriptFileName = @"Models\DocumentDB\js\getRecentPhotosForCategoriesStoredProcedure.js";
        private const string GetRecentPhotosForCategoriesStoredProcedureId = "getRecentPhotosForCategories";

        private readonly string _currentDocumentVersion;

        private readonly DocumentClient _documentClient;
        private readonly string _documentCollectionId;

        private readonly string _documentDataBaseId;

        private readonly int _firstProfilePhotoUpdateGoldIncrement;
        private readonly int _newUserGoldBalance;

        /// <summary>
        /// The <see cref="DocumentDbRepository" /> constructor.
        /// </summary>
        /// <param name="environmentDefinition">The specified environment definition.</param>
        public DocumentDbRepository(EnvironmentDefinitionBase environmentDefinition)
        {
            _newUserGoldBalance = environmentDefinition.NewUserGoldBalance;
            _firstProfilePhotoUpdateGoldIncrement = environmentDefinition.FirstProfilePhotoUpdateGoldAward;

            var documentDbStorage = environmentDefinition.DocumentDbStorage;
            _documentDataBaseId = documentDbStorage.DataBaseId;
            _documentCollectionId = documentDbStorage.CollectionId;

            try
            {
                _documentClient = new DocumentClient(new Uri(documentDbStorage.EndpointUrl),
                    documentDbStorage.AuthorizationKey);
            }
            catch (UriFormatException)
            {
                throw new DataLayerException(DataLayerError.InvalidConfiguration,
                    $"Attempted to create the DocumentClient with EndpointUrl {documentDbStorage.EndpointUrl}" +
                    $" and AuthorizationKey {documentDbStorage.AuthorizationKey} failed");
            }

            _currentDocumentVersion = BaseDocument.DocumentVersionIdentifier;
        }

        /// <summary>
        /// Checks if database and collection exist.
        /// </summary>
        /// <returns>True, if both exist. False, otherwise.</returns>
        public bool CheckIfDatabaseAndCollectionExist()
        {
            var database =
                _documentClient.CreateDatabaseQuery()
                    .Where(db => db.Id == _documentDataBaseId)
                    .AsEnumerable()
                    .FirstOrDefault();

            // The database doesn't exist.
            if (database == null)
            {
                return false;
            }

            var documentCollection =
                _documentClient.CreateDocumentCollectionQuery(database.SelfLink)
                    .Where(c => c.Id == _documentCollectionId)
                    .AsEnumerable()
                    .FirstOrDefault();

            // The collection doesn't exist.
            if (documentCollection == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a new category with the provided name.
        /// </summary>
        /// <param name="name">The category name to be created.</param>
        /// <returns>The created category.</returns>
        public async Task<CategoryContract> CreateCategory(string name)
        {
            var document = _documentClient.CreateDocumentQuery<CategoryDocument>(DocumentCollectionUri)
                .Where(d => d.DocumentType == CategoryDocument.DocumentTypeIdentifier)
                .Where(d => d.DocumentVersion == _currentDocumentVersion)
                .Where(c => c.Name == name)
                .AsEnumerable().FirstOrDefault();

            if (document != null)
            {
                throw new DataLayerException(DataLayerError.DuplicateKeyInsert, $"Category with name {name} already exists");
            }

            var categoryDocument = new CategoryDocument
            {
                Name = name
            };

            var result = await _documentClient.CreateDocumentAsync(DocumentCollectionUri, categoryDocument);

            categoryDocument.Id = result.Resource.Id;

            return categoryDocument.ToContract();
        }

        private async Task<DocumentCollection> CreateDocumentDbCollection(Database database)
        {
            try
            {
                return await _documentClient.CreateDocumentCollectionAsync(database.SelfLink, new DocumentCollection
                {
                    Id = _documentCollectionId
                });
            }
            catch (Exception)
            {
                throw new DataLayerException(DataLayerError.Unknown, $"Failed to create DocumentDB Collection {_documentCollectionId}");
            }
        }

        private async Task<Database> CreateDocumentDbDatabase()
        {
            try
            {
                return await _documentClient.CreateDatabaseAsync(new Database
                {
                    Id = _documentDataBaseId
                });
            }
            catch (Exception)
            {
                throw new DataLayerException(DataLayerError.Unknown, $"Failed to create DocumentDB Database {_documentDataBaseId}");
            }
        }

        private async Task CreateGetRecentPhotosForCategoriesStoredProcedure(string serverPath)
        {
            var sproc = new StoredProcedure
            {
                Id = GetRecentPhotosForCategoriesStoredProcedureId,
                Body = File.ReadAllText(Path.Combine(serverPath, GetRecentPhotosForCategoriesStoredProcedureScriptFileName))
            };

            await _documentClient.UpsertStoredProcedureAsync(DocumentCollectionUri, sproc);
        }

        /// <summary>
        /// Processes a list of photo json documents and creates <see cref="PhotoContract" /> for them,
        /// as well as fetching and setting the proper <see cref="UserContract" /> objects for PhotoContract.User
        /// and AnnotationContract.From fields.
        /// </summary>
        /// <param name="photoDocuments"></param>
        /// <returns>A list of photo contracts.</returns>
        private async Task<IList<PhotoContract>> CreatePhotoContractsAndLoadUserData(IList<PhotoDocument> photoDocuments)
        {
            // Retrieve all the user documents for the user ids of the photo owners and annotation authors.
            var userDocumentsForPhotosAndAnnotations =
                await GetAllUserDocumentsFromIdList(photoDocuments.Select(p => p.UserId)
                    .Concat(photoDocuments.SelectMany(p => p.Annotations).Select(a => a.From)).ToList());

            // Pass in the collection of users so ToContract can set the proper user objects for PhotoContract and the AnnotationContracts.
            return
                photoDocuments.Select(photoDocument => photoDocument.ToContract(userDocumentsForPhotosAndAnnotations))
                    .ToList();
        }

        private async Task CreateTransferGoldStoredProcedure(string serverPath)
        {
            var sproc = new StoredProcedure
            {
                Id = TransferGoldStoredProcedureId,
                Body = File.ReadAllText(Path.Combine(serverPath, TransferGoldStoredProcedureScriptFileName))
            };

            await _documentClient.UpsertStoredProcedureAsync(DocumentCollectionUri, sproc);
        }

        /// <summary>
        /// Inserts a new user record in the database.
        /// </summary>
        /// <param name="registrationReference">The Azure Mobile Service user id.</param>
        /// <returns>Updated user object.</returns>
        public async Task<UserContract> CreateUser(string registrationReference)
        {
            //Create the new user document with default values and starting gold balance.
            var userDocument = new UserDocument
            {
                GoldBalance = 0,
                RegistrationReference = registrationReference,
                CreatedAt = new DateDocument
                {
                    Date = DateTime.UtcNow
                },
                ModifiedAt = new DateDocument
                {
                    Date = DateTime.UtcNow
                },
                GoldGiven = 0
            };

            var createdUserId =
                (await _documentClient.CreateDocumentAsync(DocumentCollectionUri, userDocument)).Resource.Id;

            // Handle gold balance changes and create transaction record
            await ExecuteGoldTransactionSproc(createdUserId, SystemUserId, _newUserGoldBalance,
                GoldTransactionType.WelcomeGoldTransaction);

            userDocument.Id = createdUserId;
            userDocument.GoldBalance = _newUserGoldBalance;

            return userDocument.ToContract();
        }

        /// <summary>
        /// Deletes an annotation.
        /// </summary>
        /// <param name="annotationId">Id of annotation to be deleted.</param>
        /// <param name="userRegistrationReference">userRegistrationReference of annotation to be deleted.</param>
        public async Task DeleteAnnotation(string annotationId, string userRegistrationReference)
        {
            var photoDocument = GetParentPhotoDocument(annotationId);

            photoDocument.Annotations = photoDocument.Annotations.Where(a => a.Id != annotationId).ToList();

            await ReplacePhotoDocument(photoDocument);
        }

        /// <summary>
        /// Deletes all the data for the provided photo id.
        /// </summary>
        /// <param name="photoId">Id of the photo to be deleted.</param>
        /// <param name="userRegistrationReference">Azure Mobile Service user id.</param>
        public async Task DeletePhoto(string photoId, string userRegistrationReference)
        {
            try
            {
                var photoToDelete = GetPhotoDocument(photoId);
                var photoOwner = GetUserDocumentByUserId(photoToDelete.UserId);

                if (photoOwner.RegistrationReference != userRegistrationReference)
                {
                    throw new DataLayerException(
                        DataLayerError.NotFound,
                        $"User={userRegistrationReference} doesn't own the photo, invalid DeletePhoto request.");
                }

                if (photoOwner.ProfilePhotoId == photoId)
                {
                    throw new DataLayerException(
                        DataLayerError.NotFound,
                        $"Photo={photoId} is currently a profile picture and cannot be deleted.");
                }

                await _documentClient.DeleteDocumentAsync(
                    UriFactory.CreateDocumentUri(
                        _documentDataBaseId,
                        _documentCollectionId,
                        photoId));
            }
            catch (DocumentClientException ex)
            {
                throw new DataLayerException(DataLayerError.Unknown, ex.Message, ex);
            }
        }

        /// <summary>
        /// Initiates the process of releasing resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources.
        /// </summary>
        /// <param name="disposing">If we need to release resources or not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _documentClient.Dispose();
            }
        }

        private string DocumentCollectionUri
        {
            get { return $"dbs/{_documentDataBaseId}/colls/{_documentCollectionId}"; }
        }

        private async Task<GoldTransactionDocument> ExecuteGoldTransactionSproc(string toUserId, string fromUserId,
            int goldValue, GoldTransactionType transactionType, string photoId = null)
        {
            try
            {
                return
                    await
                        _documentClient.ExecuteStoredProcedureAsync<GoldTransactionDocument>(
                            TransferGoldStoredProcedureUri,
                            toUserId, fromUserId, goldValue, transactionType, photoId, (fromUserId == SystemUserId),
                            _currentDocumentVersion);
            }
            catch (Exception ex)
            {
                throw new DataLayerException(DataLayerError.FailedGoldTransaction,
                    $"An error occured during the gold transaction \"{ex.Message}\", any unfinished changes have been rolled back");
            }
        }

        private async Task<IList<CategoryDocument>> GetAllCategoryDocuments()
        {
            var query = _documentClient.CreateDocumentQuery<CategoryDocument>(DocumentCollectionUri)
                .Where(d => d.DocumentType == CategoryDocument.DocumentTypeIdentifier)
                .Where(d => d.DocumentVersion == _currentDocumentVersion)
                .AsDocumentQuery();

            var documentResponse = await query.ExecuteNextAsync<CategoryDocument>();
            return documentResponse.ToList();
        }

        private async Task<IList<UserDocument>> GetAllUserDocumentsFromIdList(ICollection<string> listOfUserIds)
        {
            listOfUserIds = listOfUserIds.Distinct().ToList();

            var query = _documentClient.CreateDocumentQuery<UserDocument>(DocumentCollectionUri)
                .Where(d => d.DocumentType == UserDocument.DocumentTypeIdentifier)
                .Where(d => d.DocumentVersion == _currentDocumentVersion)
                .Where(u => listOfUserIds.Contains(u.Id))
                .AsDocumentQuery();

            var documentResponse = await query.ExecuteNextAsync<UserDocument>();

            return documentResponse.ToList();
        }

        private AnnotationDocument GetAnnotationDocument(string annotationId)
        {
            var photoDocument = GetParentPhotoDocument(annotationId);

            var annotationDocument = photoDocument.Annotations.FirstOrDefault(a => a.Id == annotationId);

            if (annotationDocument == null)
            {
                throw new DataLayerException(DataLayerError.NotFound, $"No annotation with id {annotationId} found");
            }

            return annotationDocument;
        }

        /// <summary>
        /// Fetches all the categories and sorts them by name.
        /// </summary>
        /// <returns>List interface of CategoryContract sorted by name.</returns>
        public async Task<IList<CategoryContract>> GetCategories()
        {
            return (await GetAllCategoryDocuments()).Select(c => c.ToContract()).OrderBy(c => c.Name).ToList();
        }

        /// <summary>
        /// Retrieves all the categories that have atleast one photo and also retrieves
        /// number of provided thumbnails for the photos in each category.
        /// </summary>
        /// <param name="numberOfThumbnails">Max number of thumbnails per category.</param>
        /// <returns>List interface of CategoryPreviewContract.</returns>
        public async Task<IList<CategoryPreviewContract>> GetCategoriesPreview(int numberOfThumbnails)
        {
            var results = new List<CategoryPreviewContract>();

            var photosQuery = await _documentClient.ExecuteStoredProcedureAsync<List<PhotoDocument>>(
                    GetRecentPhotosForCategoriesStoredProcedureUri,
                    numberOfThumbnails,
                    _currentDocumentVersion);

            
            var mostRecentCategoryPhotos = photosQuery.Response;

            if (mostRecentCategoryPhotos != null && mostRecentCategoryPhotos.Any())
            {
                // Create a collection of all categories represented in the list of photos we received
                var allCategories =
                    mostRecentCategoryPhotos.GroupBy(p => p.CategoryId)
                        .Select(group => new CategoryContract { Id = group.First().CategoryId, Name = group.First().CategoryName });

                // Create a CategoryPreviewContract for each category represented
                // to contain its photos
                foreach (var category in allCategories)
                {
                    var photoDocuments = mostRecentCategoryPhotos.Where(p => p.CategoryId == category.Id).ToList();

                    if (photoDocuments.Any())
                    {
                        results.Add(new CategoryPreviewContract
                        {
                            Id = category.Id,
                            Name = category.Name,
                            PhotoThumbnails = photoDocuments.Select(p => new PhotoThumbnailContract
                            {
                                CreatedAt = p.CreatedDateTime.Date,
                                ImageUrl = p.ThumbnailUrl
                            }).OrderByDescending(ptc => ptc.CreatedAt).ToList()
                        });
                    }
                }
            }

            return results;
        }

        private CategoryDocument GetCategoryDocument(string categoryId)
        {
            var category = _documentClient.CreateDocumentQuery<CategoryDocument>(DocumentCollectionUri)
                .Where(d => d.DocumentType == CategoryDocument.DocumentTypeIdentifier)
                .Where(d => d.DocumentVersion == _currentDocumentVersion)
                .Where(c => c.Id == categoryId)
                .AsEnumerable().FirstOrDefault();

            if (category == null)
            {
                throw new DataLayerException(DataLayerError.NotFound, $"No category with id {categoryId} found");
            }

            return category;
        }

        /// <summary>
        /// Fetches the photo stream data for a provided category.
        /// </summary>
        /// <param name="categoryId">The category id.</param>
        /// <param name="continuationToken">Continuation token from previous <see cref="PagedResponse{TContract}" />.</param>
        /// <returns>List of photos up to the page size.</returns>
        public async Task<PagedResponse<PhotoContract>> GetCategoryPhotoStream(string categoryId,
            string continuationToken = null)
        {
            var feedOptions = new FeedOptions
            {
                MaxItemCount = PhotoStreamPageSize,
                RequestContinuation = continuationToken
            };

            var query = _documentClient.CreateDocumentQuery<PhotoDocument>(DocumentCollectionUri,
                feedOptions)
                .Where(d => d.DocumentType == PhotoDocument.DocumentTypeIdentifier)
                .Where(d => d.DocumentVersion == _currentDocumentVersion)
                .Where(p => p.CategoryId == categoryId)
                .Where(p => p.Status == PhotoStatus.Active)
                .OrderByDescending(p => p.CreatedDateTime.Epoch)
                .AsDocumentQuery();

            var documentResponse = await query.ExecuteNextAsync<PhotoDocument>();

            var photoContracts = await CreatePhotoContractsAndLoadUserData(documentResponse.ToList());

            var result = new PagedResponse<PhotoContract>
            {
                Items = photoContracts,
                ContinuationToken = documentResponse.ResponseContinuation
            };

            return result;
        }

        /// <summary>
        /// Gets hero photos.
        /// </summary>
        /// <param name="count">The number of hero photos.</param>
        /// <param name="daysOld">The number of days old the photos can be.</param>
        /// <returns>List interface of hero photos.</returns>
        public async Task<IList<PhotoContract>> GetHeroPhotos(int count, int daysOld)
        {
            var feedOptions = new FeedOptions
            {
                MaxItemCount = count
            };

            var cutOffDate = new DateDocument
            {
                Date = DateTime.Now.AddDays(-daysOld)
            };

            var query = _documentClient.CreateDocumentQuery<PhotoDocument>(DocumentCollectionUri,
                feedOptions)
                .Where(d => d.DocumentType == PhotoDocument.DocumentTypeIdentifier)
                .Where(d => d.DocumentVersion == _currentDocumentVersion)
                .Where(p => p.Status == PhotoStatus.Active)
                .Where(p => p.CreatedDateTime.Epoch >= cutOffDate.Epoch)
                .OrderByDescending(p => p.GoldCount)
                .AsDocumentQuery();

            var documentResponse = await query.ExecuteNextAsync<PhotoDocument>();

            return await CreatePhotoContractsAndLoadUserData(documentResponse.ToList());
        }

        private async Task<IList<LeaderboardEntryContract<UserContract>>> GetHighestGivingUsers(int count)
        {
            var feedOptions = new FeedOptions
            {
                MaxItemCount = count
            };

            var userQuery = _documentClient.CreateDocumentQuery<UserDocument>(DocumentCollectionUri,
                feedOptions)
                .Where(d => d.DocumentType == UserDocument.DocumentTypeIdentifier)
                .Where(d => d.DocumentVersion == _currentDocumentVersion)
                .Where(u => u.Id != SystemUserId)
                .OrderByDescending(u => u.GoldGiven)
                .AsDocumentQuery();

            var userDocumentResponse = await userQuery.ExecuteNextAsync<UserDocument>();

            var rank = 1;
            var mostGivingUsers = userDocumentResponse.Select(u => new LeaderboardEntryContract<UserContract>
            {
                Model = u.ToContract(),
                Value = u.GoldGiven,
                Rank = rank++
            }).ToList();

            return mostGivingUsers;
        }

        private async Task<IList<LeaderboardEntryContract<CategoryContract>>> GetHighestNetWorthCategories(int count)
        {
            var allPhotosQuery = _documentClient.CreateDocumentQuery<PhotoDocument>(DocumentCollectionUri)
                    .Where(d => d.DocumentType == PhotoDocument.DocumentTypeIdentifier)
                    .Where(d => d.DocumentVersion == _currentDocumentVersion)
                    .AsDocumentQuery();

            var allPhotos = await allPhotosQuery.ExecuteNextAsync<PhotoDocument>();

            var allCategories = allPhotos.GroupBy(p => p.CategoryId).Select(g => new CategoryContract
            {
                Id = g.Key,
                Name = g.FirstOrDefault()?.CategoryName
            });

            var allCategoryWorths = new List<LeaderboardEntryContract<CategoryContract>>();

            foreach (var categoryContract in allCategories)
            {
                var categoryWorth = allPhotos.Where(p => p.CategoryId == categoryContract.Id).Sum(p => p.GoldCount);

                allCategoryWorths.Add(new LeaderboardEntryContract<CategoryContract>
                {
                    Model = categoryContract,
                    Value = categoryWorth
                });
            }

            var mostGoldCategories = allCategoryWorths.OrderByDescending(e => e.Value).Take(count).ToList();
            var rank = 1;
            foreach (var leaderboardEntryContract in mostGoldCategories)
            {
                leaderboardEntryContract.Rank = rank++;
            }

            return mostGoldCategories;
        }

        private async Task<IList<LeaderboardEntryContract<PhotoContract>>> GetHighestNetWorthPhotos(int count)
        {
            var feedOptions = new FeedOptions
            {
                MaxItemCount = count
            };

            var photoQuery = _documentClient.CreateDocumentQuery<PhotoDocument>(DocumentCollectionUri,
                feedOptions)
                .Where(d => d.DocumentType == PhotoDocument.DocumentTypeIdentifier)
                .Where(d => d.DocumentVersion == _currentDocumentVersion)
                .Where(p => p.Status == PhotoStatus.Active)
                .OrderByDescending(p => p.GoldCount)
                .AsDocumentQuery();

            var photoDocumentResponse = await photoQuery.ExecuteNextAsync<PhotoDocument>();
            var photoContracts = await CreatePhotoContractsAndLoadUserData(photoDocumentResponse.ToList());

            var rank = 1;
            var mostGoldPhotos = photoContracts.Select(p => new LeaderboardEntryContract<PhotoContract>
            {
                Model = p,
                Value = p.NumberOfGoldVotes,
                Rank = rank++
            }).ToList();

            return mostGoldPhotos;
        }

        private async Task<IList<LeaderboardEntryContract<UserContract>>> GetHighestNetWorthUser(int count)
        {
            var feedOptions = new FeedOptions
            {
                MaxItemCount = count
            };

            var userQuery = _documentClient.CreateDocumentQuery<UserDocument>(DocumentCollectionUri,
                feedOptions)
                .Where(d => d.DocumentType == UserDocument.DocumentTypeIdentifier)
                .Where(d => d.DocumentVersion == _currentDocumentVersion)
                .Where(u => u.Id != SystemUserId)
                .OrderByDescending(u => u.GoldBalance)
                .AsDocumentQuery();

            var userDocumentResponse = await userQuery.ExecuteNextAsync<UserDocument>();

            var rank = 1;
            var mostGoldUsers = userDocumentResponse.Select(u => new LeaderboardEntryContract<UserContract>
            {
                Model = u.ToContract(),
                Value = u.GoldBalance,
                Rank = rank++
            }).ToList();

            return mostGoldUsers;
        }

        /// <summary>
        /// Gets the leaderboard data.
        /// </summary>
        /// <param name="mostGoldCategoriesCount">Count of categories.</param>
        /// <param name="mostGoldPhotosCount">Count of photos.</param>
        /// <param name="mostGoldUsersCount">Count of wealthiest users.</param>
        /// <param name="mostGivingUsersCount">Count of most giving users.</param>
        /// <returns>The leaderboard data.</returns>
        public async Task<LeaderboardContract> GetLeaderboard(int mostGoldCategoriesCount, int mostGoldPhotosCount,
            int mostGoldUsersCount, int mostGivingUsersCount)
        {
            return new LeaderboardContract
            {
                MostGoldCategories = await GetHighestNetWorthCategories(mostGoldCategoriesCount),
                MostGoldPhotos = await GetHighestNetWorthPhotos(mostGoldPhotosCount),
                MostGoldUsers = await GetHighestNetWorthUser(mostGoldUsersCount),
                MostGivingUsers = await GetHighestGivingUsers(mostGivingUsersCount)
            };
        }

        private PhotoDocument GetParentPhotoDocument(string annotationId)
        {
            var photoQuery = _documentClient.CreateDocumentQuery<PhotoDocument>(DocumentCollectionUri)
                .Where(d => d.DocumentType == PhotoDocument.DocumentTypeIdentifier)
                .Where(d => d.DocumentVersion == _currentDocumentVersion)
                .SelectMany(p => p.Annotations
                    .Where(a => a.Id == annotationId)
                    .Select(a => new { photo = p }
                ))
                .AsEnumerable().FirstOrDefault();

            var photoDocument = photoQuery?.photo;

            if (photoDocument == null)
            {
                throw new DataLayerException(DataLayerError.NotFound, $"No annotation with id {annotationId} found");
            }

            return photoDocument;
        }

        /// <summary>
        /// Gets the photo data for provided photo id.
        /// </summary>
        /// <param name="id">The photo id.</param>
        /// <returns>The requested photo.</returns>
        public async Task<PhotoContract> GetPhoto(string id)
        {
            var document = GetPhotoDocument(id);

            return (await CreatePhotoContractsAndLoadUserData(new List<PhotoDocument> { document })).FirstOrDefault();
        }

        private PhotoDocument GetPhotoDocument(string id)
        {
            var document = _documentClient.CreateDocumentQuery<PhotoDocument>(DocumentCollectionUri)
                .Where(d => d.DocumentType == PhotoDocument.DocumentTypeIdentifier)
                .Where(d => d.DocumentVersion == _currentDocumentVersion)
                .Where(r => r.Id == id)
                .AsEnumerable().FirstOrDefault();

            if (document == null)
            {
                throw new DataLayerException(DataLayerError.NotFound, $"No photo with id {id} found");
            }

            return document;
        }

        private string GetRecentPhotosForCategoriesStoredProcedureUri
        {
            get { return $"{StoredProceduresUri}/{GetRecentPhotosForCategoriesStoredProcedureId}"; }
        }

        /// <summary>
        /// Gets the user by an existing app user id OR registrationReference
        /// from Azure Mobile Services auth mechanism as the userId may not be known
        /// at time of entry.
        /// </summary>
        /// <param name="userId">The app user id.</param>
        /// <param name="registrationReference">[Optional] The Azure App Service user id. Default value is null.</param>
        /// <returns>UserContract</returns>
        public Task<UserContract> GetUser(string userId, string registrationReference = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    return Task.FromResult(GetUserDocumentByUserId(userId).ToContract());
                }

                return Task.FromResult(GetUserDocumentByRegistrationReference(registrationReference).ToContract());
            }
            catch (DataLayerException e)
            {
                // If the user does not exist return a blank user.
                if (e.Error == DataLayerError.NotFound)
                {
                    return Task.FromResult(new UserContract());
                }

                throw new DataLayerException(DataLayerError.Unknown, "Unknown error occured");
            }
        }

        private UserDocument GetUserDocumentByRegistrationReference(string registrationReference)
        {
            var user = _documentClient.CreateDocumentQuery<UserDocument>(DocumentCollectionUri)
                .Where(d => d.DocumentType == UserDocument.DocumentTypeIdentifier)
                .Where(d => d.DocumentVersion == _currentDocumentVersion)
                .Where(u => u.RegistrationReference == registrationReference)
                .AsEnumerable().FirstOrDefault();

            if (user == null)
            {
                throw new DataLayerException(DataLayerError.NotFound, $"No user with registrationReference {registrationReference} found");
            }

            return user;
        }

        private UserDocument GetUserDocumentByUserId(string userId)
        {
            var user = _documentClient.CreateDocumentQuery<UserDocument>(DocumentCollectionUri)
                .Where(d => d.DocumentType == UserDocument.DocumentTypeIdentifier)
                .Where(d => d.DocumentVersion == _currentDocumentVersion)
                .Where(u => u.Id == userId)
                .AsEnumerable().FirstOrDefault();

            if (user == null)
            {
                throw new DataLayerException(DataLayerError.NotFound, $"No user with id {userId} found");
            }

            return user;
        }

        /// <summary>
        /// Fetches the photo stream data for a specified user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="continuationToken">Last captured ticks in the form of a string.</param>
        /// <param name="includeNonActivePhotos">By default, false. If true, non-active photos are included.</param>
        /// <returns>List of photos up to the page size.</returns>
        public async Task<PagedResponse<PhotoContract>> GetUserPhotoStream(string userId, string continuationToken, bool includeNonActivePhotos = false)
        {
            var feedOptions = new FeedOptions
            {
                MaxItemCount = PhotoStreamPageSize,
                RequestContinuation = continuationToken
            };

            var query = _documentClient.CreateDocumentQuery<PhotoDocument>(DocumentCollectionUri,
                feedOptions)
                .Where(d => d.DocumentType == PhotoDocument.DocumentTypeIdentifier)
                .Where(d => d.DocumentVersion == _currentDocumentVersion)
                .Where(p => p.UserId == userId);

            if (!includeNonActivePhotos)
            {
                query = query
                    .Where(p => p.Status == PhotoStatus.Active);
            }

            var documentQuery = query
                .OrderByDescending(p => p.CreatedDateTime.Epoch)
                .AsDocumentQuery();

            var documentResponse = await documentQuery.ExecuteNextAsync<PhotoDocument>();

            var photoContracts = await CreatePhotoContractsAndLoadUserData(documentResponse.ToList());

            var result = new PagedResponse<PhotoContract>
            {
                Items = photoContracts,
                ContinuationToken = documentResponse.ResponseContinuation
            };

            return result;
        }

        /// <summary>
        /// Checks if the defined document database and collection exists
        /// and initializes them if they don't.
        /// </summary>
        public async Task InitializeDatabaseIfNotExisting(string serverPath)
        {
            var database = _documentClient.CreateDatabaseQuery()
                .Where(db => db.Id == _documentDataBaseId)
                .AsEnumerable()
                .FirstOrDefault();

            if (database == null)
            {
                database = await CreateDocumentDbDatabase();
            }

            var documentCollection = _documentClient.CreateDocumentCollectionQuery(database.SelfLink)
                .Where(c => c.Id == _documentCollectionId)
                .AsEnumerable()
                .FirstOrDefault();

            if (documentCollection == null)
            {
                await CreateDocumentDbCollection(database);
            }

            await InitializeStoredProceduresIfNotExisting(serverPath);
        }

        private async Task InitializeStoredProceduresIfNotExisting(string serverPath)
        {
            try
            {
                var sprocs = _documentClient.CreateStoredProcedureQuery(StoredProceduresUri).AsEnumerable();

                if (!sprocs.Any(s => s.Id == TransferGoldStoredProcedureId))
                {
                    await CreateTransferGoldStoredProcedure(serverPath);
                }

                if (!sprocs.Any(s => s.Id == GetRecentPhotosForCategoriesStoredProcedureId))
                {
                    await CreateGetRecentPhotosForCategoriesStoredProcedure(serverPath);
                }
            }
            catch (IOException ex)
            {
                throw new DataLayerException(DataLayerError.NotFound, "The file given for a stored procedure could not be located.", ex);
            }
            catch (Exception ex)
            {
                throw new DataLayerException(DataLayerError.Unknown, "An unknown error occured while inserting a stored procedure.", ex);
            }
        }

        /// <summary>
        /// Inserts the annotation object and performs the required gold transactions.
        /// </summary>
        /// <param name="annotationContract">Annotation to be inserted.</param>
        /// <returns>AnnotationContract.</returns>
        public async Task<AnnotationContract> InsertAnnotation(AnnotationContract annotationContract)
        {
            var annotationDocument = AnnotationDocument.CreateFromContract(annotationContract);

            annotationDocument.Id = Guid.NewGuid().ToString();
            annotationContract.Id = annotationDocument.Id;
            annotationDocument.CreatedDateTime = new DateDocument
            {
                Date = DateTime.UtcNow
            };

            var photoDocument = PhotoDocument.CreateFromContract(await GetPhoto(annotationContract.PhotoId));

            if (photoDocument.Annotations.Any(a => a.Id == annotationContract.Id))
            {
                throw new DataLayerException(DataLayerError.DuplicateKeyInsert, $"Annotation with Id={annotationContract.Id} already exists");
            }

            photoDocument.Annotations.Add(annotationDocument);
            photoDocument.GoldCount += annotationContract.GoldCount;

            // Handle gold balance changes and create transaction record
            await
                ExecuteGoldTransactionSproc(photoDocument.UserId, annotationContract.From.UserId,
                    annotationContract.GoldCount,
                    GoldTransactionType.PhotoGoldTransaction, annotationContract.PhotoId);

            await ReplacePhotoDocument(photoDocument);

            return annotationContract;
        }

        /// <summary>
        /// Inserts receipt and adds gold to user.
        /// </summary>
        /// <param name="validatedIapReciept">Validated receipt values.</param>
        /// <returns>User object containing new gold balance.</returns>
        public async Task<UserContract> InsertIapPurchase(IapPurchaseContract validatedIapReciept)
        {
            var document = _documentClient.CreateDocumentQuery<IapPurchaseDocument>(DocumentCollectionUri)
                .Where(d => d.DocumentType == IapPurchaseDocument.DocumentTypeIdentifier)
                .Where(d => d.DocumentVersion == _currentDocumentVersion)
                .Where(i => i.Id == validatedIapReciept.IapPurchaseId)
                .AsEnumerable().FirstOrDefault();

            if (document != null)
            {
                throw new DataLayerException(DataLayerError.DuplicateKeyInsert,
                    $"Iap Purchase with Id={validatedIapReciept.IapPurchaseId} already exists");
            }

            if (validatedIapReciept.GoldIncrement > 0)
            {
                // Handle gold balance changes and create transaction record
                await
                    ExecuteGoldTransactionSproc(validatedIapReciept.UserId, SystemUserId,
                        validatedIapReciept.GoldIncrement,
                        GoldTransactionType.IapGoldTransaction);
            }

            var iapPurchaseDocument = IapPurchaseDocument.CreateFromContract(validatedIapReciept);
            await _documentClient.CreateDocumentAsync(DocumentCollectionUri, iapPurchaseDocument);

            return await GetUser(validatedIapReciept.UserId);
        }

        /// <summary>
        /// Insert the photo object into storage.
        /// </summary>
        /// <param name="photo">Photo Object.</param>
        /// <param name="goldIncrement">Gold to award for new photo.</param>
        /// <returns>New PhotoContract with updated user balance.</returns>
        public async Task<PhotoContract> InsertPhoto(PhotoContract photo, int goldIncrement)
        {
            var document = _documentClient.CreateDocumentQuery<PhotoDocument>(DocumentCollectionUri)
                .Where(d => d.DocumentType == PhotoDocument.DocumentTypeIdentifier)
                .Where(d => d.DocumentVersion == _currentDocumentVersion)
                .Where(u => u.Id == photo.Id)
                .AsEnumerable().FirstOrDefault();

            if (document != null)
            {
                throw new DataLayerException(DataLayerError.DuplicateKeyInsert, $"Photo with Id={photo.Id} already exists");
            }

            var photoDocument = PhotoDocument.CreateFromContract(photo);
            photoDocument.Status = PhotoStatus.Active;
            photoDocument.CreatedDateTime.Date = DateTime.UtcNow;
            photoDocument.ModifiedAt = photoDocument.CreatedDateTime;

            // Set the category name on insert
            photoDocument.CategoryName = GetCategoryDocument(photo.CategoryId).Name;

            var resourceResponse = await _documentClient.CreateDocumentAsync(DocumentCollectionUri, photoDocument);

            // Handle gold balance changes and create transaction record
            await ExecuteGoldTransactionSproc(photo.User.UserId, SystemUserId, goldIncrement,
                GoldTransactionType.PhotoGoldTransaction, resourceResponse.Resource.Id);

            return await GetPhoto(resourceResponse.Resource.Id);
        }

        /// <summary>
        /// Inserts a report into Report table.
        /// </summary>
        /// <param name="reportContract">The report being inserted.</param>
        /// <param name="userRegistrationReference">Azure Mobile Service user id who is reporting it.</param>
        /// <returns>The inserted Report.</returns>
        public async Task<ReportContract> InsertReport(ReportContract reportContract, string userRegistrationReference)
        {
            reportContract.Id = Guid.NewGuid().ToString();
            reportContract.ReporterUserId = GetUserDocumentByRegistrationReference(userRegistrationReference).Id;
            reportContract.CreatedDateTime = DateTime.UtcNow;

            var reportDocument = ReportDocument.CreateFromContract(reportContract);

            switch (reportContract.ContentType)
            {
                case ContentType.Photo:
                    var photoDocument = GetPhotoDocument(reportContract.ContentId);
                    photoDocument.Report = reportDocument;
                    await ReplacePhotoDocument(photoDocument);
                    break;
                case ContentType.Annotation:
                    var annotationDocument = GetAnnotationDocument(reportContract.ContentId);
                    annotationDocument.Report = reportDocument;
                    await UpdateAnnotationDocument(annotationDocument);
                    break;
                default:
                    throw new DataLayerException(DataLayerError.Unknown, "Unknown report content type");
            }

            return reportContract;
        }

        /// <summary>
        /// Forces a reinitalization of the database by recreating the document database and collection.
        /// </summary>
        public async Task ReinitializeDatabase(string serverPath)
        {
            var database =
                _documentClient.CreateDatabaseQuery()
                    .Where(db => db.Id == _documentDataBaseId)
                    .AsEnumerable()
                    .FirstOrDefault();

            if (database != null)
            {
                await _documentClient.DeleteDatabaseAsync(database.SelfLink);
            }

            database = await CreateDocumentDbDatabase();
            await CreateDocumentDbCollection(database);

            await CreateTransferGoldStoredProcedure(serverPath);
            await CreateGetRecentPhotosForCategoriesStoredProcedure(serverPath);
        }

        private async Task<PhotoDocument> ReplacePhotoDocument(PhotoDocument photoDocument)
        {
            try
            {
                // Set the category name
                photoDocument.CategoryName = GetCategoryDocument(photoDocument.CategoryId).Name;
                photoDocument.ModifiedAt = new DateDocument
                {
                    Date = DateTime.UtcNow
                };

                var response = await _documentClient.ReplaceDocumentAsync(
                    UriFactory.CreateDocumentUri(
                        _documentDataBaseId,
                        _documentCollectionId,
                        photoDocument.Id),
                    photoDocument);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new DataLayerException(DataLayerError.Unknown, response.StatusCode.ToString());
                }
            }
            catch (DocumentClientException ex)
            {
                throw new DataLayerException(DataLayerError.NotFound, ex.Message, ex);
            }

            return photoDocument;
        }

        private async Task<UserDocument> ReplaceUserDocument(UserDocument userDocument)
        {
            var existingUser = GetUserDocumentByRegistrationReference(userDocument.RegistrationReference);

            // First time profile pic update awards user some gold (value found in environment definitions)
            var firstTimeProfileGoldAward = (existingUser.ProfilePhotoId == null && userDocument.ProfilePhotoId != null);

            // Persist the user's GoldGiven value since it is not currently present in userContracts
            if (userDocument.GoldGiven == 0 && existingUser.GoldGiven != 0)
            {
                userDocument.GoldGiven = existingUser.GoldGiven;
            }

            try
            {
                userDocument.ModifiedAt = new DateDocument
                {
                    Date = DateTime.UtcNow
                };

                var response = await _documentClient.ReplaceDocumentAsync(
                    UriFactory.CreateDocumentUri(
                        _documentDataBaseId,
                        _documentCollectionId,
                        userDocument.Id),
                    userDocument);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new DataLayerException(DataLayerError.Unknown, response.StatusCode.ToString());
                }
            }
            catch (DocumentClientException ex)
            {
                throw new DataLayerException(DataLayerError.NotFound, ex.Message, ex);
            }

            if (firstTimeProfileGoldAward)
            {
                // Handle gold balance changes and create transaction record
                await ExecuteGoldTransactionSproc(userDocument.Id, SystemUserId, _firstProfilePhotoUpdateGoldIncrement,
                    GoldTransactionType.FirstProfilePicUpdateTransaction);
            }

            return userDocument;
        }

        private string StoredProceduresUri
        {
            get { return $"dbs/{_documentDataBaseId}/colls/{_documentCollectionId}/sprocs"; }
        }

        private string TransferGoldStoredProcedureUri
        {
            get { return $"{StoredProceduresUri}/{TransferGoldStoredProcedureId}"; }
        }

        private async Task UpdateAnnotationDocument(AnnotationDocument annotationDocument)
        {
            var photoDocument = GetParentPhotoDocument(annotationDocument.Id);

            var indexToUpdate = photoDocument.Annotations.FindIndex(a => a.Id == annotationDocument.Id);

            if (indexToUpdate < 0)
            {
                throw new DataLayerException(DataLayerError.NotFound,
                    $"No annotation with id {annotationDocument.Id} was actully present within the returned photoDocument");
            }

            photoDocument.Annotations[indexToUpdate] = annotationDocument;

            await ReplacePhotoDocument(photoDocument);
        }

        /// <summary>
        /// Updates an existing photo object's category and description fields.
        /// </summary>
        /// <param name="photoContract">Photo object.</param>
        /// <returns>PhotoContract containing updated data.</returns>
        public async Task<PhotoContract> UpdatePhoto(PhotoContract photoContract)
        {
            var photoDocument = GetPhotoDocument(photoContract.Id);

            photoDocument.CategoryId = photoContract.CategoryId;
            photoDocument.Description = photoContract.Description;

            await ReplacePhotoDocument(photoDocument);

            return photoContract;
        }

        /// <summary>
        /// Updates the status of the stored photo object.
        /// </summary>
        /// <param name="photoContract">Photo object.</param>
        /// <returns>PhotoContract containing updated data.</returns>
        public async Task<PhotoContract> UpdatePhotoStatus(PhotoContract photoContract)
        {
            var photoDocument = GetPhotoDocument(photoContract.Id);

            photoDocument.Status = photoContract.Status;

            await ReplacePhotoDocument(photoDocument);

            return photoContract;
        }

        /// <summary>
        /// Updates the user profile picture.  User gold balance is also updated if it is the first time
        /// the user is updating their profile picture.
        /// </summary>
        /// <param name="userContract">
        /// We need the whole user object as it is assumed the client will have given the photoId and
        /// url.
        /// </param>
        /// <returns>A new UserContract object.</returns>
        public async Task<UserContract> UpdateUser(UserContract userContract)
        {
            await ReplaceUserDocument(UserDocument.CreateFromContract(userContract));

            return userContract;
        }
    }
}