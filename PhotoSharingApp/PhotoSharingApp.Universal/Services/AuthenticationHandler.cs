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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using PhotoSharingApp.Portable.DataContracts;
using PhotoSharingApp.Universal.ContractModelConverterExtensions;
using PhotoSharingApp.Universal.Models;
using Windows.Security.Credentials;

namespace PhotoSharingApp.Universal.Services
{
    /// <summary>
    /// Helper calss for doing authentication & managing auth status
    /// using Azure Mobile Services.
    /// </summary>
    public class AuthenticationHandler : IAuthenticationHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationHandler" /> class.
        /// </summary>
        public AuthenticationHandler()
        {
            AuthenticationProviders = new List<MobileServiceAuthenticationProvider>
            {
                MobileServiceAuthenticationProvider.Facebook,
                MobileServiceAuthenticationProvider.MicrosoftAccount,
                MobileServiceAuthenticationProvider.Google,
                MobileServiceAuthenticationProvider.Twitter
            };
        }

        /// <summary>
        /// Gets the preferred authentication providers for this app.
        /// </summary>
        /// <value>
        /// The authentication providers.
        /// </value>
        public List<MobileServiceAuthenticationProvider> AuthenticationProviders { get; }

        /// <summary>
        /// Starts the authentication process.
        /// </summary>
        /// <param name="provider">The provider to authenticate with.</param>
        public async Task AuthenticateAsync(MobileServiceAuthenticationProvider provider)
        {
            try
            {
                var vault = new PasswordVault();

                // Login with the identity provider.
                var user = await AzureAppService.Current
                    .LoginAsync(provider);

                // Create and store the user credentials.
                var credential = new PasswordCredential(provider.ToString(),
                    user.UserId, user.MobileServiceAuthenticationToken);

                vault.Add(credential);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                if (invalidOperationException.Message
                    .Contains("Authentication was cancelled by the user."))
                {
                    throw new AuthenticationCanceledException("Authentication canceled by user",
                        invalidOperationException);
                }

                throw new AuthenticationException("Authentication failed", invalidOperationException);
            }
            catch (Exception e)
            {
                throw new AuthenticationException("Authentication failed", e);
            }
        }

        /// <summary>
        /// Logs the user out.
        /// </summary>
        public async Task LogoutAsync()
        {
            // Use the PasswordVault to securely store and access credentials.
            var vault = new PasswordVault();

            var credential = vault.RetrieveAll().FirstOrDefault();
            if (credential != null)
            {
                // Create a user from the stored credentials.
                var user = new MobileServiceUser(credential.UserName);
                credential.RetrievePassword();
                user.MobileServiceAuthenticationToken = credential.Password;

                // Set the user from the stored credentials.
                AzureAppService.Current.CurrentUser = user;

                try
                {
                    // Try to return an item now to determine if the cached credential has expired.
                    await AzureAppService.Current.LogoutAsync();
                }
                catch (MobileServiceInvalidOperationException)
                {
                    // We are not interested in any exceptions here
                }
                finally
                {
                    // Make sure vault is cleaned up
                    ResetPasswordVault();
                }
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Clears the password vault.
        /// </summary>
        public void ResetPasswordVault()
        {
            var vault = new PasswordVault();
            var credentials = vault.RetrieveAll();

            foreach (var passwordCredential in credentials)
            {
                vault.Remove(passwordCredential);
            }
        }

        /// <summary>
        /// Restores the sign in status.
        /// </summary>
        /// <returns>Returns the current user.</returns>
        public async Task<User> RestoreSignInStatus()
        {
            // Use the PasswordVault to securely store and access credentials.
            var vault = new PasswordVault();

            // Try to get an existing credential from the vault.
            var credential = vault.RetrieveAll().FirstOrDefault();

            if (credential != null)
            {
                // Create a user from the stored credentials.
                var user = new MobileServiceUser(credential.UserName);
                credential.RetrievePassword();
                user.MobileServiceAuthenticationToken = credential.Password;

                // Set the user from the stored credentials.
                AzureAppService.Current.CurrentUser = user;

                try
                {
                    var userContract = await AzureAppService.Current.InvokeApiAsync<UserContract>("User", HttpMethod.Get,
                        null);

                    return userContract.ToDataModel();
                }
                catch (MobileServiceInvalidOperationException invalidOperationException)
                {
                    if (invalidOperationException.Response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        // Remove the credentials.
                        ResetPasswordVault();

                        AzureAppService.Current.CurrentUser = null;
                        credential = null;
                    }
                }
            }

            return null;
        }
    }
}