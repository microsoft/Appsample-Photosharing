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

using Newtonsoft.Json;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Shared.Models.DocumentDB
{
    /// <summary>
    /// The report json document.
    /// </summary>
    public class ReportDocument : BaseDocument
    {
        /// <summary>
        /// Determines if the report needs to be addressed.
        /// </summary>
        [JsonProperty(PropertyName = "Active")]
        public bool Active = true;

        /// <summary>
        /// The timestamp it was reported.
        /// </summary>
        [JsonProperty(PropertyName = "CreatedDateTime")]
        public DateDocument CreatedDateTime;

        /// <summary>
        /// The user id who reported the content.
        /// </summary>
        [JsonProperty(PropertyName = "ReporterUserId")]
        public string ReporterUserId;

        /// <summary>
        /// The reason why it was reported.
        /// </summary>
        [JsonProperty(PropertyName = "ReportReason")]
        public ReportReason ReportReason;

        /// <summary>
        /// Creates a <see cref="ReportDocument" /> from a <see cref="ReportContract" />.
        /// </summary>
        /// <returns>The <see cref="ReportDocument" />.</returns>
        public static ReportDocument CreateFromContract(ReportContract contract)
        {
            return new ReportDocument
            {
                Id = contract.Id,
                Active = contract.Active,
                ReporterUserId = contract.ReporterUserId,
                ReportReason = contract.ReportReason,
                CreatedDateTime = new DateDocument
                {
                    Date = contract.CreatedDateTime
                }
            };
        }

        /// <summary>
        /// Creates a <see cref="ReportContract" /> from this document.
        /// </summary>
        /// <param name="contentId">The id of the content this report is for.</param>
        /// <param name="contentType">The content type this report is for.</param>
        /// <returns>The <see cref="ReportContract" />.</returns>
        public ReportContract ToContract(string contentId, ContentType contentType)
        {
            return new ReportContract
            {
                Active = Active,
                CreatedDateTime = CreatedDateTime.Date,
                Id = Id,
                ReportReason = ReportReason,
                ReporterUserId = ReporterUserId,
                ContentId = contentId,
                ContentType = contentType
            };
        }
    }
}