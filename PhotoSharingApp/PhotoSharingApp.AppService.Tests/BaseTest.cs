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
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.Tests
{
    public abstract class BaseTest
    {
        protected AnnotationContract CreateTestAnnotation(UserContract fromUser, int goldCount, string photoId, string photoOwnerId, string text = "From Unit Test")
        {
            return new AnnotationContract
            {
                From = fromUser,
                GoldCount = goldCount,
                PhotoId = photoId,
                PhotoOwnerId = photoOwnerId,
                Text = text
            };
        }

        protected IapPurchaseContract CreateTestIapPurchase(string userId, int goldIncrement, string productId)
        {
            return new IapPurchaseContract
            {
                IapPurchaseId = Guid.NewGuid().ToString(),
                UserId = userId,
                GoldIncrement = goldIncrement,
                ProductId = productId,
                PurchaseDatetime = DateTime.UtcNow,
                ExpirationDatetime = DateTime.MaxValue
            };
        }

        protected PhotoContract CreateTestPhoto(CategoryContract category, UserContract user, string description = "Test photo", int goldCount = 0)
        {
            return new PhotoContract
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                User = user,
                Description = description,
                OSPlatform = "test",
                HighResolutionUrl = "http://bing.com",
                StandardUrl = "http://bing.com",
                ThumbnailUrl = "http://bing.com",
                NumberOfGoldVotes = goldCount
            };
        }

        protected ReportContract CreateTestReport(string contentId, ContentType contentType, ReportReason reportReason, string reporterId)
        {
            return new ReportContract
            {
                ContentId = contentId,
                ContentType = contentType,
                ReportReason = reportReason,
                ReporterUserId = reporterId
            };
        }
    }
}