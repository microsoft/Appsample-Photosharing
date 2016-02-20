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

using System.Collections.Generic;
using PhotoSharingApp.Universal.ComponentModel;

namespace PhotoSharingApp.Universal.Models
{
    /// <summary>
    /// Represents the current leaderboard standings
    /// </summary>
    public class Leaderboard : ObservableObjectBase
    {
        /// <summary>
        /// The user entries for most gold given
        /// </summary>
        public List<LeaderboardEntry<User>> MostGivingUsers { get; set; } = new List<LeaderboardEntry<User>>();

        /// <summary>
        /// The category entries for highest net worth
        /// </summary>
        public List<LeaderboardEntry<Category>> MostGoldCategories { get; set; } = new List<LeaderboardEntry<Category>>();

        /// <summary>
        /// The photo entries for highest net worth
        /// </summary>
        public List<LeaderboardEntry<Photo>> MostGoldPhotos { get; set; } = new List<LeaderboardEntry<Photo>>();

        /// <summary>
        /// The user entries for highest net worth
        /// </summary>
        public List<LeaderboardEntry<User>> MostGoldUsers { get; set; } = new List<LeaderboardEntry<User>>();
    }
}