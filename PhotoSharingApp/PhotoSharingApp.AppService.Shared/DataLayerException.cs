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
using System.Runtime.Serialization;

namespace PhotoSharingApp.AppService.Shared
{
    /// <summary>
    /// Exception thrown from the repository.
    /// </summary>
    [Serializable]
    public class DataLayerException : Exception
    {
        private const string SerializationErrorName = "error";

        /// <summary>
        /// Constructor for DataLayerException.
        /// </summary>
        /// <param name="error">The DataLayerError.</param>
        /// <param name="message">The message.</param>
        public DataLayerException(DataLayerError error, string message)
            : base(message)
        {
            Error = error;
        }

        /// <summary>
        /// Constructor for DataLayerException.
        /// </summary>
        /// <param name="error">The DataLayerError.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DataLayerException(DataLayerError error, string message, Exception innerException)
            : base(message, innerException)
        {
            Error = error;
        }

        /// <summary>
        /// Gets the error code indicating the cause of the failure.
        /// </summary>
        public DataLayerError Error { get; }

        /// <summary>
        /// Get serialized object data.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Serialized stream.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(SerializationErrorName, Error);
        }
    }
}