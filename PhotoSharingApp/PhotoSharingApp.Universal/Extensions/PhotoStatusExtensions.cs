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

using PhotoSharingApp.Portable.DataContracts;
using Windows.ApplicationModel.Resources;

namespace PhotoSharingApp.Universal.Extensions
{
    /// <summary>
    /// Provides extensions for the <see cref="PhotoStatus" /> class.
    /// </summary>
    public static class PhotoStatusExtensions
    {
        /// <summary>
        /// Returns a human readable string from a <see cref="PhotoStatus" /> value.
        /// </summary>
        /// <param name="photoStatus">The PhotoStatus object.</param>
        /// <returns>The human readable string.</returns>
        public static string ToReadableString(this PhotoStatus photoStatus)
        {
            var resourceLoader = ResourceLoader.GetForCurrentView();
            switch (photoStatus)
            {
                case PhotoStatus.Active:
                    return resourceLoader.GetString("PhotoStatus_Active");
                case PhotoStatus.Deleted:
                    return resourceLoader.GetString("PhotoStatus_Deleted");
                case PhotoStatus.DoesntFitCategory:
                    return resourceLoader.GetString("PhotoStatus_DoesntFitCategory");
                case PhotoStatus.Hidden:
                    return resourceLoader.GetString("PhotoStatus_Hidden");
                case PhotoStatus.ObjectionableContent:
                    return resourceLoader.GetString("PhotoStatus_ObjectionableContent");
                case PhotoStatus.UnderReview:
                    return resourceLoader.GetString("PhotoStatus_UnderReview");
                default:
                    return resourceLoader.GetString("PhotoStatus_Unknown");
            }
        }
    }
}