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
using PhotoSharingApp.AppService.Shared.Repositories;

namespace PhotoSharingApp.AppService.Shared.Validation
{
    /// <summary>
    /// Validation class for photo operations.
    /// </summary>
    public class PhotoValidation : IPhotoValidation
    {
        private readonly IRepository _repository;

        /// <summary>
        /// The constructor for creating a new <see cref="PhotoValidation"/> instance
        /// </summary>
        /// <param name="repository">Data layer.</param>
        public PhotoValidation(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Validates that the current user owns a photo.
        /// </summary>
        /// <param name="userRegistration">Account string value from mobile service auth.</param>
        /// <param name="photoId">Id of photo to validate.</param>
        /// <returns>True when user is proven to own photo.</returns>
        public async Task<bool> IsUserPhotoOwner(string userRegistration, string photoId)
        {
            var isUserOwner = false;

            // Get the userId for the user logged in, not what was passed in with the photo if spoofed.
            var currentUser = await _repository.GetUser(string.Empty, userRegistration);

            // Get the photo for the id passed in, not what was passed into the referring call in case it was spoofed.
            var photo = await _repository.GetPhoto(photoId);

            if (currentUser.UserId != null && photo.User != null)
            {
                // Use guid to eliminate any string comparison errors.
                isUserOwner = (new Guid(currentUser.UserId).Equals(new Guid(photo.User.UserId)));
            }

            return isUserOwner;
        }
    }
}