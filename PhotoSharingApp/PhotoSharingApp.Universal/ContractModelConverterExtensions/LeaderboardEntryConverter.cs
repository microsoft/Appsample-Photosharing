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
    /// Helper class to convert between <see cref="LeaderboardEntry{T}" />
    /// and <see cref="LeaderboardEntryContract{T}" /> classes.
    /// </summary>
    public static class LeaderboardEntryConverter
    {
        /// <summary>
        /// Converts a LeaderboardEntryContract object containing CategoryContract objects
        /// </summary>
        /// <param name="leaderboardEntryContract">The leaderboardEntryContract</param>
        /// <returns>A LeaderboardEntry object containing Category objects</returns>
        public static LeaderboardEntry<Category> ToDataModel(
            this LeaderboardEntryContract<CategoryContract> leaderboardEntryContract)
        {
            return new LeaderboardEntry<Category>
            {
                Model = leaderboardEntryContract.Model.ToDataModel(),
                Rank = leaderboardEntryContract.Rank,
                Value = leaderboardEntryContract.Value
            };
        }

        /// <summary>
        /// Converts a LeaderboardEntryContract object containing PhotoContract objects
        /// </summary>
        /// <param name="leaderboardEntryContract">The leaderboardEntryContract</param>
        /// <returns>A LeaderboardEntry object containing Photo objects</returns>
        public static LeaderboardEntry<Photo> ToDataModel(
            this LeaderboardEntryContract<PhotoContract> leaderboardEntryContract)
        {
            return new LeaderboardEntry<Photo>
            {
                Model = leaderboardEntryContract.Model.ToDataModel(),
                Rank = leaderboardEntryContract.Rank,
                Value = leaderboardEntryContract.Value
            };
        }

        /// <summary>
        /// Converts a LeaderboardEntryContract object containing UserContract objects
        /// </summary>
        /// <param name="leaderboardEntryContract">The leaderboardEntryContract</param>
        /// <returns>A LeaderboardEntry object containing User objects</returns>
        public static LeaderboardEntry<User> ToDataModel(
            this LeaderboardEntryContract<UserContract> leaderboardEntryContract)
        {
            return new LeaderboardEntry<User>
            {
                Model = leaderboardEntryContract.Model.ToDataModel(),
                Rank = leaderboardEntryContract.Rank,
                Value = leaderboardEntryContract.Value
            };
        }
    }
}