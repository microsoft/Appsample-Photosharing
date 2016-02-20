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

using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Telemetry;
using PhotoSharingApp.Universal.Views;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The ViewModel for the give gold view.
    /// </summary>
    public class GiveGoldViewModel : ViewModelBase
    {
        private string _annotationText;
        private readonly IDialogService _dialogService;
        private bool _isBusy;
        private readonly IPhotoService _photoService;
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="photoService">The photo service.</param>
        /// <param name="telemetryClient">The telemetry client.</param>
        /// <param name="dialogService">The dialog service.</param>
        public GiveGoldViewModel(IPhotoService photoService, TelemetryClient telemetryClient,
            IDialogService dialogService)
        {
            _photoService = photoService;
            _telemetryClient = telemetryClient;
            _dialogService = dialogService;

            // Initialize commands
            CancelCommand = new RelayCommand(OnCancel);
        }

        /// <summary>
        /// Gets or sets the annotation.
        /// </summary>
        public Annotation Annotation { get; private set; }

        /// <summary>
        /// Gets or sets the annotation text.
        /// </summary>
        public string AnnotationText
        {
            get { return _annotationText; }
            set
            {
                if (value != _annotationText)
                {
                    _annotationText = value;
                    NotifyPropertyChanged(nameof(AnnotationText));
                }
            }
        }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public RelayCommand CancelCommand { get; }

        /// <summary>
        /// Gets the amount of gold to give.
        /// </summary>
        public int GoldToGive { get; } = 1;

        /// <summary>
        /// Gets or sets the busy state.
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (value != _isBusy)
                {
                    _isBusy = value;
                    NotifyPropertyChanged(nameof(IsBusy));
                }
            }
        }

        /// <summary>
        /// Gets or sets the photo.
        /// </summary>
        public Photo Photo { get; set; }

        /// <summary>
        /// Loads the state.
        /// </summary>
        public override async Task LoadState()
        {
            await base.LoadState();
        }

        private void OnCancel()
        {
            _telemetryClient.TrackEvent(TelemetryEvents.GiveGoldDialogCanceled);
            Annotation = null;
        }

        public async Task<bool> PostAnnotationToService()
        {
            try
            {
                IsBusy = true;

                var serviceResult = await _photoService.PostAnnotation(Photo, _annotationText, GoldToGive);

                // If annotation was succesfully posted, update HasUserGivenGold, and store annotation
                if (serviceResult != null)
                {
                    _telemetryClient.TrackEvent(TelemetryEvents.GiveGoldDialogSuccess);
                    Photo.HasUserGivenGold = true;
                    Annotation = serviceResult;

                    return true;
                }

                _telemetryClient.TrackEvent(TelemetryEvents.GiveGoldDialogFailed);

                return false;
            }
            catch (InsufficientBalanceException balanceException)
            {
                _telemetryClient.TrackException(balanceException);
                await _dialogService.ShowNotification("InsufficientBalance_Message", "InsufficientBalance_Title");

                return false;
            }
            catch (ServiceException e)
            {
                _telemetryClient.TrackException(e);
                await _dialogService.ShowGenericServiceErrorNotification();

                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}