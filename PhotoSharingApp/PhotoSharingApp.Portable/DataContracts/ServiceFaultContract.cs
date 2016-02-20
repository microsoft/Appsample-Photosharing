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

using System.Collections.Generic;

namespace PhotoSharingApp.Portable.DataContracts
{
    /// <summary>
    /// The service fault contract which contains error details.
    /// </summary>
    public class ServiceFaultContract
    {
        /// <summary>
        /// Gets or sets the error code associated with this error.
        /// </summary>
        /// <remarks>
        /// The Source and the Code together form the unique identifier for the fault.
        /// </remarks>
        public ushort Code { get; set; }

        /// <summary>
        /// Gets or sets a human-readable description for this fault.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets an array of details associated with the fault.
        /// </summary>
        public IList<string> Details { get; } = new List<string>();

        /// <summary>
        /// Gets or sets the source of the fault. Generally this will be the service name, although it does not have to
        /// be.
        /// </summary>
        public string Source { get; set; }
    }
}