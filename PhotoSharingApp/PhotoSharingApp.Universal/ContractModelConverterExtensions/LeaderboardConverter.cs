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
using PhotoSharingApp.Portable.DataContracts;
using PhotoSharingApp.Universal.Models;

namespace PhotoSharingApp.Universal.ContractModelConverterExtensions
{
    /// <summary>
    /// Helper class to convert between <see cref="Leaderboard" />
    /// and <see cref="LeaderboardContract" /> classes.
    /// </summary>
    public static class LeaderboardConverter
    {
        /// <summary>
        /// Converts a LeaderboardContract object
        /// </summary>
        /// <param name="leaderboardContract">The leaderboardContract</param>
        /// <returns>A Leaderboard object</returns>
        public static Leaderboard ToDataModel(this LeaderboardContract leaderboardContract)
        {
            return new Leaderboard
            {
                MostGoldCategories = leaderboardContract.MostGoldCategories.Select(e => e.ToDataModel()).ToList(),
                MostGoldPhotos = leaderboardContract.MostGoldPhotos.Select(e => e.ToDataModel()).ToList(),
                MostGoldUsers = leaderboardContract.MostGoldUsers.Select(e => e.ToDataModel()).ToList(),
                MostGivingUsers = leaderboardContract.MostGivingUsers.Select(e => e.ToDataModel()).ToList()
            };
        }
    }
}