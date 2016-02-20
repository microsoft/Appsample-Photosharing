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
using PhotoSharingApp.Universal.ComponentModel;

namespace PhotoSharingApp.Universal.Models
{
    /// <summary>
    /// Represents a category.
    /// </summary>
    public class Category : ObservableObjectBase
    {
        private string _id;
        private string _name;

        /// <summary>
        /// Gets the data model validation result.
        /// Returns true, if it contains one or more errors.
        /// Otherwise, false.
        /// </summary>
        public bool HasErrors
        {
            get
            {
                return string.IsNullOrWhiteSpace(_name)
                       || _name.Length < 2
                       || _name.StartsWith("my", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public string Id
        {
            get { return _id; }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged(nameof(Id));
                }
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged(nameof(Name));

                    // The name has changed, so we need to update the
                    // object's validation status.
                    NotifyPropertyChanged(nameof(HasErrors));
                }
            }
        }
    }
}