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

namespace PhotoSharingApp.Portable.DataContracts
{
    /// <summary>
    /// The data contract used for reporting content.
    /// </summary>
    public class ReportContract
    {
        /// <summary>
        /// Determines if the report needs to be addressed.
        /// </summary>
        public bool Active = true;

        /// <summary>
        /// The Id of the photo or annotation.
        /// </summary>
        public string ContentId;

        /// <summary>
        /// The content type.
        /// </summary>
        public ContentType ContentType;

        /// <summary>
        /// The timestamp it was reported.
        /// </summary>
        public DateTime CreatedDateTime;

        /// <summary>
        /// Report Id.
        /// </summary>
        public string Id;

        /// <summary>
        /// The user id who reported the content.
        /// </summary>
        public string ReporterUserId;

        /// <summary>
        /// The reason why it was reported.
        /// </summary>
        public ReportReason ReportReason;
    }
}