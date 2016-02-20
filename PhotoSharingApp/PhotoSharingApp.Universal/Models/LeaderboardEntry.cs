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

using PhotoSharingApp.Universal.ComponentModel;

namespace PhotoSharingApp.Universal.Models
{
    /// <summary>
    /// Represents the current leaderboard standings
    /// </summary>
    public class LeaderboardEntry<T> : ObservableObjectBase where T : class
    {
        private T _model;
        private long _rank;
        private int _value;

        /// <summary>
        /// The model that this entry is for
        /// </summary>
        public T Model
        {
            get { return _model; }
            set
            {
                if (value != _model)
                {
                    _model = value;
                    NotifyPropertyChanged(nameof(Model));
                }
            }
        }

        /// <summary>
        /// The leaderboard rank for this entry
        /// </summary>
        public long Rank
        {
            get { return _rank; }
            set
            {
                if (value != _rank)
                {
                    _rank = value;
                    NotifyPropertyChanged(nameof(Rank));
                }
            }
        }

        /// <summary>
        /// The entry value
        /// </summary>
        public int Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    NotifyPropertyChanged(nameof(Value));
                }
            }
        }
    }
}