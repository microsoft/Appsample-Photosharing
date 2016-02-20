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

using Microsoft.Practices.Unity;

namespace PhotoSharingApp.Universal.Unity
{
    /// <summary>
    /// Provides extensions for <see cref="IUnityContainer" />.
    /// </summary>
    public static class UnityExtensions
    {
        /// <summary>
        /// Registers a type mapping with the container.
        /// </summary>
        /// <typeparam name="TFrom">System.Type that will be requested.</typeparam>
        /// <typeparam name="TTo">System.Type that will actually be returned.</typeparam>
        /// <param name="container">Container to configure.</param>
        public static void RegisterTypeWithName<TFrom, TTo>(this IUnityContainer container) where TTo : TFrom
        {
            container.RegisterType<TFrom, TTo>(typeof(TTo).FullName);
        }
    }
}