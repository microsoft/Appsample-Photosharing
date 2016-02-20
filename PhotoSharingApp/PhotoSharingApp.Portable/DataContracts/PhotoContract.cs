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

namespace PhotoSharingApp.Portable.DataContracts
{
    /// <summary>
    /// The photo data contract.
    /// </summary>
    public class PhotoContract
    {
        /// <summary>
        /// The associated annotations of the photo.
        /// </summary>
        public List<AnnotationContract> Annotations { get; set; } = new List<AnnotationContract>();

        /// <summary>
        /// The category Id.
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// The category name.
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// The timestamp when the photo has been created
        /// in the system.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The photo description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The high-resolution image url.
        /// </summary>
        public string HighResolutionUrl { get; set; }

        /// <summary>
        /// The photo Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The timestamp when the photo has been modified
        /// in the system.
        /// </summary>
        public DateTime ModifiedAt { get; set; }

        /// <summary>
        /// The photo's number of annotations.
        /// </summary>
        public int NumberOfAnnotations { get; set; }

        /// <summary>
        /// The photo's number of gold pieces.
        /// </summary>
        public int NumberOfGoldVotes { get; set; }

        /// <summary>
        /// The operating system platform the photo has been
        /// uploaded from.
        /// </summary>
        public string OSPlatform { get; set; }

        /// <summary>
        /// The photo rank in the leaderboards.
        /// </summary>
        public long Rank { get; set; }

        /// <summary>
        /// The standard-sized image url.
        /// </summary>
        public string StandardUrl { get; set; }

        /// <summary>
        /// The photo status.
        /// </summary>
        public PhotoStatus Status { get; set; }

        /// <summary>
        /// The thumbnail url.
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// The photo owner.
        /// </summary>
        public UserContract User { get; set; }
    }
}