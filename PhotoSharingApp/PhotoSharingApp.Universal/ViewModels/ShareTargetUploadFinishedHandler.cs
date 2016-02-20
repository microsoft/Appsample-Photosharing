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
using System.Threading.Tasks;
using PhotoSharingApp.Universal.Models;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The handler that is being actived when a photo upload
    /// has been finished successfully when the app is in the
    /// share target mode.
    /// </summary>
    public class ShareTargetUploadFinishedHandler : IUploadFinishedHandler
    {
        private readonly Action _uploadFinishedAction;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="uploadFinishedAction">The action to execute on successful upload.</param>
        public ShareTargetUploadFinishedHandler(Action uploadFinishedAction)
        {
            _uploadFinishedAction = uploadFinishedAction;
        }

        /// <summary>
        /// Called when a photo has been uploaded successfully.
        /// </summary>
        /// <param name="category">The associated category.</param>
        public Task OnUploadFinished(Category category)
        {
            _uploadFinishedAction.Invoke();

            return Task.CompletedTask;
        }
    }
}