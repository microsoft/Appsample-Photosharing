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

using System.Threading.Tasks;

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// Defines a service for showing dialogs to the user.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Shows a notification that there is an issue with communicating
        /// to the service.
        /// </summary>
        Task ShowGenericServiceErrorNotification();

        /// <summary>
        /// Shows the notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="useResourceLoader">
        /// if set to <c>true</c> the message and title
        /// parameters specify the resource id.
        /// </param>
        Task ShowNotification(string message, string title, bool useResourceLoader = true);

        /// <summary>
        /// Shows a notification that lets the user choose between yes and no.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="useResourceLoader">
        /// if set to <c>true</c> the message and title
        /// parameters specify the resource id.
        /// </param>
        /// <returns>True, if user has chosen yes. Otherwise, false.</returns>
        Task<bool> ShowYesNoNotification(string message, string title, bool useResourceLoader = true);
    }
}