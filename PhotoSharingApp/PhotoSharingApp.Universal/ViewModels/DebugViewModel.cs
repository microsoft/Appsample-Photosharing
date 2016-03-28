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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.ServiceEnvironments;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Store;
using PhotoSharingApp.Universal.Store.Simulation;
using PhotoSharingApp.Universal.Unity;
using PhotoSharingApp.Universal.Views;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// ViewModel for debug view.
    /// </summary>
    public class DebugViewModel : ViewModelBase
    {
        private readonly IAuthenticationHandler _authenticationHandler;
        private readonly IDialogService _dialogService;
        private bool _isServiceConnected;
        private ServiceEnvironmentBase _selectedService;
        private IPhotoService _photoService;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="photoService">The photo service.</param>
        /// <param name="authenticationHandler">The authentication handler.</param>
        /// <param name="dialogService">The dialog service.</param>
        public DebugViewModel(IPhotoService photoService, IAuthenticationHandler authenticationHandler,
            IDialogService dialogService)
        {
            _photoService = photoService;
            _authenticationHandler = authenticationHandler;
            _dialogService = dialogService;

            ThrowExceptionCommand = new RelayCommand(OnThrowException);
        }

        /// <summary>
        /// If dummy service enabled, this enables simulated exceptions on
        /// service calls.
        /// </summary>
        public bool IsErrorSimulationEnabled
        {
            get
            {
                var dummyService = _photoService as PhotoDummyService;
                if (dummyService != null)
                {
                    return dummyService.IsErrorSimulationEnabled;
                }

                return false;
            }
            set
            {
                var dummyService = _photoService as PhotoDummyService;
                if (dummyService != null)
                {
                    dummyService.IsErrorSimulationEnabled = value;
                }

                NotifyPropertyChanged(nameof(IsErrorSimulationEnabled));
            }
        }

        /// <summary>
        /// If the app is properly connected to the selected service, this will be true.
        /// </summary>
        public bool IsServiceConnected
        {
            get
            {
                return _isServiceConnected;
            }
            set
            {
                if (value != _isServiceConnected)
                {
                    _isServiceConnected = value;
                    NotifyPropertyChanged(nameof(IsServiceConnected));
                }
            }
        }

        /// <summary>
        /// Loads the state.
        /// </summary>
        public override async Task LoadState()
        {
            await base.LoadState();

            RefreshConfig();
        }

        /// <summary>
        /// Collection that populates the service selector combobox
        /// </summary>
        public ObservableCollection<ServiceEnvironmentBase> MobileServices { get; } = new ObservableCollection
            <ServiceEnvironmentBase>
        {
            new ServiceEnvironment(),
        };

        /// <summary>
        /// The selected service shown in the combo box
        /// </summary>
        public ServiceEnvironmentBase SelectedService
        {
            get
            {
                return
                    MobileServices.FirstOrDefault(
                        service => service.ServiceBaseUrl == ServiceEnvironmentBase.Current.ServiceBaseUrl);
            }
            set
            {
                _selectedService = value;
                ServiceEnvironmentBase.Current = _selectedService;

                // Auth credentials will not work across different environments,
                // so we need to sign out the user.
                _photoService.SignOutAsync();

                AzureAppService.Reset();
                AppEnvironment.Instance.CurrentUser = null;

                // Whenever service environment is being switched, we need to make
                // sure we do not use invalid credentials from other environments.
                _authenticationHandler.ResetPasswordVault();

                UnityBootstrapper.Container.RegisterType<IPhotoService, ServiceClient>();
                _photoService = ServiceLocator.Current.GetInstance<IPhotoService>();

                RefreshConfig();
                NotifyPropertyChanged(nameof(SelectedService));
            }
        }

        /// <summary>
        /// Returns the command for throwing an unhandled exception.
        /// </summary>
        public RelayCommand ThrowExceptionCommand { get; private set; }

        /// <summary>
        /// Setting this to true will enable the photo dummy service.
        /// </summary>
        public bool UsePhotoDummyService
        {
            get
            {
                var currentService = ServiceLocator.Current.GetInstance<IPhotoService>();
                return currentService is PhotoDummyService;
            }
            set
            {
                _photoService?.SignOutAsync();

                if (value)
                {
                    // PhotoDummyService needs to be container controlled as we generate random Guids at runtime.
                    // Otherwise, e.g. navigation to a category would fail as a new instance of this class
                    // would have newly generated category Ids which are not matching with the ones passed.
                    UnityBootstrapper.Container.RegisterType<IPhotoService, PhotoDummyService>(
                        new ContainerControlledLifetimeManager());
                }
                else
                {
                    AzureAppService.Reset();
                    UnityBootstrapper.Container.RegisterType<IPhotoService, ServiceClient>();
                }

                // By default we switch on the CurrentAppSimulator if dummy service is enabled.
                UseStoreMock = value;

                RefreshConfig();
                _photoService = ServiceLocator.Current.GetInstance<IPhotoService>();

                NotifyPropertyChanged(nameof(UsePhotoDummyService));
            }
        }

        /// <summary>
        /// If set to true, CurrentAppSimulator class is being used.
        /// Otherwise, CurrentApp class.
        /// </summary>
        public bool UseStoreMock
        {
            get { return CurrentAppProxy.IsMockEnabled; }
            set
            {
                if (value != CurrentAppProxy.IsMockEnabled)
                {
                    CurrentAppProxy.IsMockEnabled = value;
                    NotifyPropertyChanged(nameof(UseStoreMock));

                    if (value)
                    {
                        CurrentAppSimulatorHelper.InitCurrentAppSimulator();
                    }
                }
            }
        }

        private void OnThrowException()
        {
            throw new ArgumentException("Test exception");
        }

        private async void RefreshConfig()
        {
            try
            {
                var currentService = ServiceLocator.Current.GetInstance<IPhotoService>();
                var config = await currentService.GetConfig();
                AppEnvironment.Instance.SetConfig(config);
                IsServiceConnected = true;
            }
            catch (Exception)
            {
                IsServiceConnected = false;
                AppEnvironment.Instance.SetConfig(new DefaultConfig());
                await _dialogService.ShowGenericServiceErrorNotification();
            }
        }
    }
}