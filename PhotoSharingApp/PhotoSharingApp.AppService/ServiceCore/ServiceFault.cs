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

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using PhotoSharingApp.Portable.DataContracts;

namespace PhotoSharingApp.AppService.ServiceCore
{
    /// <summary>
    /// Helper class for instantiating errors returned.
    /// from the service.
    /// </summary>
    public sealed class ServiceFault
    {
        /// <summary>
        /// Creates a fault exception with the HTTP status code 400.
        /// </summary>
        /// <param name="source">The source of the fault.</param>
        /// <param name="code">The code of the fault.</param>
        /// <param name="description">A human-readable description of what went wrong.</param>
        /// <param name="details">Details about what went wrong, specific to each fault.</param>
        /// <returns>A HttpResponseException containing the fault.</returns>
        public static HttpResponseException BadRequest(
            string source, ushort code, string description, params object[] details)
        {
            return Create(HttpStatusCode.BadRequest, source, code, description, details);
        }

        /// <summary>
        /// Creates a new fault, wrapped in a HttpResponseException.
        /// </summary>
        /// <param name="statusCode">The HTTP status code to associate with the fault.</param>
        /// <param name="source">The source of the fault.</param>
        /// <param name="code">The code of the fault.</param>
        /// <param name="description">A human-readable description of what went wrong.</param>
        /// <param name="details">Details about what went wrong, specific to each fault.</param>
        /// <returns>A HttpResponseException containing the fault.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "Response message will be managed by the pipeline.")]
        public static HttpResponseException Create(
            HttpStatusCode statusCode, string source, ushort code, string description, params object[] details)
        {
            var fault = new ServiceFaultContract
            {
                Source = source,
                Code = code,
                Description = description
            };

            if (details != null)
            {
                foreach (var detail in details)
                {
                    fault.Details.Add(detail?.ToString());
                }
            }

            return Create(statusCode, fault);
        }

        /// <summary>
        /// Creates a new fault, wrapped in a HttpResponseException.
        /// </summary>
        /// <param name="statusCode">The HTTP status code to associate with the fault.</param>
        /// <param name="fault">A ServiceFault</param>
        /// <returns>A HttpResponseException containing the fault.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "Response message will be managed by the pipeline.")]
        public static HttpResponseException Create(HttpStatusCode statusCode, ServiceFaultContract fault)
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new ObjectContent<ServiceFaultContract>(fault, new JsonMediaTypeFormatter())
            };

            return new HttpResponseException(response);
        }

        /// <summary>
        /// Creates a fault exception with the HTTP status code 403.
        /// </summary>
        /// <param name="source">The source of the fault.</param>
        /// <param name="code">The code of the fault.</param>
        /// <param name="description">A human-readable description of what went wrong.</param>
        /// <param name="details">Details about what went wrong, specific to each fault.</param>
        /// <returns>A HttpResponseException containing the fault.</returns>
        public static HttpResponseException Forbidden(
            string source, ushort code, string description, params object[] details)
        {
            return Create(HttpStatusCode.Forbidden, source, code, description, details);
        }

        /// <summary>
        /// Creates a fault exception with the HTTP status code 500.
        /// </summary>
        /// <param name="source">The source of the fault.</param>
        /// <param name="code">The code of the fault.</param>
        /// <param name="description">A human-readable description of what went wrong.</param>
        /// <param name="details">Details about what went wrong, specific to each fault.</param>
        /// <returns>A HttpResponseException containing the fault.</returns>
        public static HttpResponseException InternalServerError(
            string source, ushort code, string description, params object[] details)
        {
            return Create(HttpStatusCode.InternalServerError, source, code, description, details);
        }

        /// <summary>
        /// Creates a fault exception with the HTTP status code 404.
        /// </summary>
        /// <param name="source">The source of the fault.</param>
        /// <param name="code">The code of the fault.</param>
        /// <param name="description">A human-readable description of what went wrong.</param>
        /// <param name="details">Details about what went wrong, specific to each fault.</param>
        /// <returns>A HttpResponseException containing the fault.</returns>
        public static HttpResponseException NotFound(
            string source, ushort code, string description, params object[] details)
        {
            return Create(HttpStatusCode.NotFound, source, code, description, details);
        }

        /// <summary>
        /// Creates a fault exception with the HTTP status code 503.
        /// </summary>
        /// <param name="source">The source of the fault.</param>
        /// <param name="code">The code of the fault.</param>
        /// <param name="description">A human-readable description of what went wrong.</param>
        /// <param name="details">Details about what went wrong, specific to each fault.</param>
        /// <returns>A HttpResponseException containing the fault.</returns>
        public static HttpResponseException ServiceUnavailable(
            string source, ushort code, string description, params object[] details)
        {
            return Create(HttpStatusCode.ServiceUnavailable, source, code, description, details);
        }

        /// <summary>
        /// Creates a fault exception with the HTTP status code 401.
        /// </summary>
        /// <param name="source">The source of the fault.</param>
        /// <param name="code">The code of the fault.</param>
        /// <param name="description">A human-readable description of what went wrong.</param>
        /// <param name="details">Details about what went wrong, specific to each fault.</param>
        /// <returns>A HttpResponseException containing the fault.</returns>
        public static HttpResponseException Unauthorized(
            string source, ushort code, string description, params object[] details)
        {
            return Create(HttpStatusCode.Unauthorized, source, code, description, details);
        }
    }
}