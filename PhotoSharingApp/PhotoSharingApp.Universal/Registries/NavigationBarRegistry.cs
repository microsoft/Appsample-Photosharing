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
using PhotoSharingApp.Universal.NavigationBar;
using PhotoSharingApp.Universal.Unity;

namespace PhotoSharingApp.Universal.Registries
{
    /// <summary>
    /// Dependency registry for navigation bar items.
    /// </summary>
    public class NavigationBarRegistry : RegistryBase, IRegistry
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="container">The Unity container.</param>
        public NavigationBarRegistry(IUnityContainer container) : base(container)
        {
        }

        /// <summary>
        /// Configures dependencies.
        /// </summary>
        public void Configure()
        {
            // Top items
            Container.RegisterTypeWithName<INavigationBarMenuItem, CategoriesNavigationBarMenuItem>();
            Container.RegisterTypeWithName<INavigationBarMenuItem, CameraNavigationBarMenuItem>();
            Container.RegisterTypeWithName<INavigationBarMenuItem, WelcomeNavigationBarMenuItem>();
            Container.RegisterTypeWithName<INavigationBarMenuItem, LeaderboardNavigationBarMenuItem>();
            Container.RegisterTypeWithName<INavigationBarMenuItem, ProfileNavigationBarMenuItem>();

            // Bottom items
            Container.RegisterTypeWithName<INavigationBarMenuItem, SettingsNavigationBarMenuItem>();
        }
    }
}