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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace PhotoSharingApp.Universal.Extensions
{
    /// <summary>
    /// Provides extensions for the <see cref="Enum" /> class.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the display name attribute as a string
        /// </summary>
        /// <param name="en">The enum.</param>
        /// <returns>The display name attribute as a string</returns>
        // Note that this is not a localization-friendly pattern. Use this for debug output or logging for developers, but not for 
        // user-facing text. Enum names are designed for English speakers, and won't make sense to users in other languages. 
        public static string ToDisplayNameAttribute(this Enum en)
        {
            return en.GetType().GetMember(en.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName();
        }
    }
}