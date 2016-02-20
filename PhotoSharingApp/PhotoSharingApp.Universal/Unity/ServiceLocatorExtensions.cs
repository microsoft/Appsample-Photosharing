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

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace PhotoSharingApp.Universal.Unity
{
    /// <summary>
    /// Provides extensions for <see cref="IServiceLocator" />.
    /// </summary>
    public static class ServiceLocatorExtensions
    {
        /// <summary>
        /// Gets an instance of the given TService.
        /// </summary>
        /// <typeparam name="TService">The requested type.</typeparam>
        /// <param name="serviceLocator">The service locator instance.</param>
        /// <param name="createNew">
        /// If true, a new instance will be returned and the existing one in the service locator
        /// overwritten.
        /// </param>
        /// <returns>The instance of TService.</returns>
        public static TService GetInstance<TService>(this IServiceLocator serviceLocator, bool createNew)
        {
            if (createNew)
            {
                UnityBootstrapper.Container.RegisterType(typeof(TService), typeof(TService),
                    new ContainerControlledLifetimeManager());
            }

            return serviceLocator.GetInstance<TService>();
        }
    }
}