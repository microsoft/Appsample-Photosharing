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
using System.Collections.ObjectModel;
using PhotoSharingApp.Portable.DataContracts;
using PhotoSharingApp.Universal.ComponentModel;

namespace PhotoSharingApp.Universal.Models
{
    /// <summary>
    /// Represents a photo
    /// </summary>
    public class Photo : ObservableObjectBase
    {
        private int _goldCount;
        private bool _hasUserGivenGold;

        /// <summary>
        /// Gets or sets the annotations.
        /// </summary>
        public ObservableCollection<Annotation> Annotations { get; set; } = new ObservableCollection<Annotation>();

        /// <summary>
        /// A single property to indicate whether or not this user can add gold to the photo
        /// </summary>
        public bool CanGiveGold
        {
            get
            {
                // You can give gold without being signed-in as this triggers
                // the sign-in UI.
                if (AppEnvironment.Instance.CurrentUser == null)
                {
                    return true;
                }

                // You can not give gold when you are the owner of the picture.
                return !User.UserId.Equals(AppEnvironment.Instance.CurrentUser.UserId);
            }
        }

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the category identifier.
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the photo was uploaded
        /// to the service.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the number of up votes.
        /// </summary>
        public int GoldCount
        {
            get { return _goldCount; }
            set
            {
                if (value != _goldCount)
                {
                    _goldCount = value;
                    NotifyPropertyChanged(nameof(GoldCount));
                }
            }
        }

        /// <summary>
        /// Returns true if photo has an active photo status.
        /// False, otherwise.
        /// </summary>
        public bool HasActiveStatus
        {
            get { return Status == PhotoStatus.Active; }
        }

        /// <summary>
        /// Gets or sets the flag if the current user has given
        /// this photo gold already.
        /// </summary>
        public bool HasUserGivenGold
        {
            get { return _hasUserGivenGold; }
            set
            {
                if (value != _hasUserGivenGold)
                {
                    _hasUserGivenGold = value;
                    NotifyPropertyChanged(nameof(HasUserGivenGold));
                }
            }
        }

        /// <summary>
        /// Gets or sets the high resolution image URL.
        /// </summary>
        public string HighResolutionUrl { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Returns the image URL based on device criteria.
        /// </summary>
        public string ImageUrl
        {
            get
            {
                if (AppEnvironment.Instance.IsMobileDeviceFamily)
                {
                    return StandardUrl;
                }

                return HighResolutionUrl;
            }
        }

        /// <summary>
        /// Returns true if this picture is the owner's profile picture
        /// </summary>
        public bool IsProfilePicture
        {
            get { return Id.Equals(User.ProfilePictureId); }
        }

        /// <summary>
        /// Gets or sets the number of comments.
        /// </summary>
        public int NumberOfAnnotations { get; set; }

        /// <summary>
        /// Gets or sets the standard image URL.
        /// </summary>
        public string StandardUrl { get; set; }

        /// <summary>
        /// Gets or sets the photo status.
        /// </summary>
        public PhotoStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail image URL.
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public User User { get; set; }
    }
}