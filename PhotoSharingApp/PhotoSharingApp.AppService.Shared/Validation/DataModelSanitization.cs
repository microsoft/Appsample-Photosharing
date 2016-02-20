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

using System.Text.RegularExpressions;
using System.Threading;

namespace PhotoSharingApp.AppService.Shared.Validation
{
    /// <summary>
    /// Various data sanitization functions.
    /// </summary>
    public static class DataModelSanitization
    {
        /// <summary>
        /// Formats string to TitleCase.
        /// </summary>
        /// <param name="input">String to modify.</param>
        /// <returns>Modified string.</returns>
        public static string ToTitleCase(this string input)
        {
            if (input == null)
            {
                return null;
            }

            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }

        /// <summary>
        /// Remove leading/trailing spaces and
        /// replace multiple white spaces with one white space.
        /// </summary>
        /// <param name="input">String to modify.</param>
        /// <returns>Modified string.</returns>
        public static string TrimAndReplaceMultipleWhitespaces(this string input)
        {
            if (input == null)
            {
                return null;
            }

            return Regex.Replace(input.Trim(), @"\s+", " ");
        }
    }
}