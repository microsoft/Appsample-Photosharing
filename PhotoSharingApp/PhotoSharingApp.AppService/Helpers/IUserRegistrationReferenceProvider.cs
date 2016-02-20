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

using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhotoSharingApp.AppService.Helpers
{
    /// <summary>
    /// Provides operations to help identify the User accessing the service via HttpRequest.
    /// </summary>
    public interface IUserRegistrationReferenceProvider
    {
        /// <summary>
        /// Extracts the User details accessing the service as a unique id in the form
        /// of "{authprovider}:{uniqueId}" using ProviderCrednetials for the logged
        /// in user.
        /// </summary>
        /// <param name="principal">The principal accessing the service.</param>
        /// <param name="request">The HttpRequest used to access the service.</param>
        /// <returns>The unique user id.</returns>
        Task<string> GetCurrentUserRegistrationReferenceAsync(ClaimsPrincipal principal, HttpRequestMessage request);
    }
}