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
using PhotoSharingApp.Universal.Camera;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Store;
using PhotoSharingApp.Universal.Store.Simulation;
using PhotoSharingApp.Universal.Views;

namespace PhotoSharingApp.Universal.Registries
{
    /// <summary>
    /// Registry for services.
    /// </summary>
    public class ServicesRegistry : RegistryBase, IRegistry
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="container">The Unity container.</param>
        public ServicesRegistry(IUnityContainer container) : base(container)
        {
        }

        /// <summary>
        /// Configures dependencies.
        /// </summary>
        public void Configure()
        {
            Container.RegisterInstance(typeof(IAppEnvironment), new AppEnvironment());

            Container.RegisterType<IDialogService, MessageDialogService>();
            Container.RegisterType<INavigationFacade, NavigationFacade>();
            Container.RegisterType<IAuthEnforcementHandler, AuthEnforcementHandler>(
                new ContainerControlledLifetimeManager());
            Container.RegisterType<IAuthenticationHandler, AuthenticationHandler>();
            Container.RegisterType<ICameraEngine, CameraEngine>();
            Container.RegisterType<ILicensingFacade, LicensingFacade>();

            // By default, in Debug mode the photo dummy service is used to showcase
            // test data.
            // In order to connect to a live service, either switch to Release mode
            // or remove the DUMMY_SERVICE directive entry in
            // PhotoSharingApp.Universal > Properties > Build > Conditional compilation symbols.
#if ( DEBUG && DUMMY_SERVICE )
            Container.RegisterType<IPhotoService, PhotoDummyService>(new ContainerControlledLifetimeManager());
            CurrentAppProxy.IsMockEnabled = true;
            CurrentAppSimulatorHelper.InitCurrentAppSimulator();
#else
            Container.RegisterType<IPhotoService, ServiceClient>();
#endif
        }
    }
}