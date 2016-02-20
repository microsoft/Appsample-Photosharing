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
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using PhotoSharingApp.Universal.Models;

namespace PhotoSharingApp.Universal.Services
{
    public interface IAuthenticationHandler
    {
        /// <summary>
        /// Gets the preferred authentication providers for this app.
        /// </summary>
        /// <value>
        /// The authentication providers.
        /// </value>
        List<MobileServiceAuthenticationProvider> AuthenticationProviders { get; }

        /// <summary>
        /// Starts the authentication process.
        /// </summary>
        /// <param name="provider">The provider to authenticate with.</param>
        Task AuthenticateAsync(MobileServiceAuthenticationProvider provider);

        /// <summary>
        /// Logs the user out.
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Clears the password vault.
        /// </summary>
        void ResetPasswordVault();

        /// <summary>
        /// Restores the sign in status.
        /// </summary>
        /// <returns>True, if successful. Otherwise, false.</returns>
        Task<User> RestoreSignInStatus();
    }
}