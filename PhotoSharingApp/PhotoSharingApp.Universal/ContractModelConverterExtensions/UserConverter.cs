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

using PhotoSharingApp.Portable.DataContracts;
using PhotoSharingApp.Universal.Models;

namespace PhotoSharingApp.Universal.ContractModelConverterExtensions
{
    /// <summary>
    /// Helper class to convert between <see cref="User" />
    /// and <see cref="UserContract" /> classes.
    /// </summary>
    public static class UserConverter
    {
        /// <summary>
        /// Converts the given data model to a data contract.
        /// </summary>
        /// <param name="user">The data model.</param>
        /// <returns>The data contract.</returns>
        public static UserContract ToDataContract(this User user)
        {
            return new UserContract
            {
                UserId = user.UserId,
                ProfilePhotoId = user.ProfilePictureId,
                ProfilePhotoUrl = user.ProfilePictureUrl,
                GoldBalance = user.GoldBalance,
                RegistrationReference = user.RegistrationReference,
                UserCreated = user.UserCreated,
                UserModified = user.UserModified
            };
        }

        /// <summary>
        /// Converts the given data contract to a data model.
        /// </summary>
        /// <param name="userContract">The data contract.</param>
        /// <returns>The data model.</returns>
        public static User ToDataModel(this UserContract userContract)
        {
            return new User
            {
                UserId = userContract.UserId,
                ProfilePictureId = userContract.ProfilePhotoId,
                ProfilePictureUrl = userContract.ProfilePhotoUrl,
                GoldBalance = userContract.GoldBalance,
                RegistrationReference = userContract.RegistrationReference,
                UserCreated = userContract.UserCreated,
                UserModified = userContract.UserModified
            };
        }
    }
}