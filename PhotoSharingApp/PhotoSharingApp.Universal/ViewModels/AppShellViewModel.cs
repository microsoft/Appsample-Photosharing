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
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Universal.NavigationBar;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The ViewModel for the AppShell.
    /// </summary>
    public class AppShellViewModel : ViewModelBase
    {
        public AppShellViewModel()
        {
            NavigationBarMenuItems = ServiceLocator.Current
                .GetAllInstances<INavigationBarMenuItem>()
                .Where(i => i.Position == NavigationBarItemPosition.Top)
                .ToList();

            BottomNavigationBarMenuItems = ServiceLocator.Current
                .GetAllInstances<INavigationBarMenuItem>()
                .Where(i => i.Position == NavigationBarItemPosition.Bottom)
                .ToList();

#if DEBUG
            BottomNavigationBarMenuItems.Add(new DebugNavigationBarMenuItem());
#endif
        }

        /// <summary>
        /// The navigation bar items at the bottom.
        /// </summary>
        public List<INavigationBarMenuItem> BottomNavigationBarMenuItems { get; }

        /// <summary>
        /// The navigation bar items at the top.
        /// </summary>
        public List<INavigationBarMenuItem> NavigationBarMenuItems { get; private set; }
    }
}