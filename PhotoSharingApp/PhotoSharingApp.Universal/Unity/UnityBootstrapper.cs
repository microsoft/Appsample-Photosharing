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
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using PhotoSharingApp.Universal.Registries;

namespace PhotoSharingApp.Universal.Unity
{
    /// <summary>
    /// Helps initializing the Unity dependency container.
    /// </summary>
    public class UnityBootstrapper
    {
        /// <summary>
        /// Gets or sets the dependency container.
        /// </summary>
        public static UnityContainer Container { get; set; }

        /// <summary>
        /// Gets the registries.
        /// </summary>
        public static List<IRegistry> Registries { get; private set; }

        private static void AddRegistries()
        {
            Registries.Add(new NavigationBarRegistry(Container));
            Registries.Add(new ServicesRegistry(Container));
            Registries.Add(new ViewModelRegistry(Container));
            Registries.Add(new ViewRegistry());
        }

        /// <summary>
        /// Configures all registered dependencies.
        /// </summary>
        /// <remarks>
        /// The <see cref="ServiceLocator" /> needs to be initialized
        /// when calling this method.
        /// </remarks>
        public static void ConfigureRegistries()
        {
            AddRegistries();
            Registries.ForEach(r => r.Configure());
        }

        /// <summary>
        /// Initializes the <see cref="ServiceLocator" />.
        /// </summary>
        public static void Init()
        {
            Container = new UnityContainer();
            Registries = new List<IRegistry>();

            var serviceLocator = new UnityServiceLocator(Container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }
    }
}