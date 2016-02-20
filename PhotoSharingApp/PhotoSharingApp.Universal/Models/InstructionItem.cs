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

namespace PhotoSharingApp.Universal.Models
{
    /// <summary>
    /// Represents an instructional item that can be displayed in the welcome screen
    /// experience for new users.
    /// </summary>
    public class InstructionItem
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="headerText">The header text.</param>
        /// <param name="contentText">The content text.</param>
        /// <param name="image">The image Uri.</param>
        /// <param name="targetPage">The target page type.</param>
        public InstructionItem(string headerText, string contentText,
            Uri image, Type targetPage = null)
        {
            HeaderText = headerText;
            ContentText = contentText;
            Image = image;
            TargetPage = targetPage;
        }

        /// <summary>
        /// Gets or sets the content text.
        /// </summary>
        public string ContentText { get; set; }

        /// <summary>
        /// Gets or sets the header text.
        /// </summary>
        public string HeaderText { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public Uri Image { get; set; }

        /// <summary>
        /// Gets or sets the optional target page.
        /// </summary>
        public Type TargetPage { get; set; }
    }
}