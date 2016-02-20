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
using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Universal.ViewModels;
using PhotoSharingApp.Universal.Views;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace PhotoSharingApp.Universal.Controls
{
    /// <summary>
    /// The page header control.
    /// </summary>
    public sealed partial class PageHeader : UserControl
    {
        /// <summary>
        /// The header content.
        /// </summary>
        public static readonly DependencyProperty HeaderContentProperty =
            DependencyProperty.Register("HeaderContent", typeof(UIElement), typeof(PageHeader),
                new PropertyMetadata(DependencyProperty.UnsetValue));

        /// <summary>
        /// The gold balance.
        /// </summary>
        public static readonly DependencyProperty GoldBalanceProperty = DependencyProperty.Register(
            "GoldBalance", typeof(int), typeof(PageHeader),
            new PropertyMetadata(default(int), GoldBalancePropertyChanged));

        private bool _goldCounterBindingInitialized;

        public PageHeader()
        {
            InitializeComponent();

            ViewModel = ServiceLocator.Current.GetInstance<PageHeaderViewModel>();

            // Let the UI listen to ViewModel changes, as we want
            // to perform animations once the gold count changes.
            SetBinding(GoldBalanceProperty, new Binding
            {
                Source = ViewModel.CurrentUser,
                Path = new PropertyPath("GoldBalance")
            });

            Loaded += (s, a) =>
            {
                if (AppShell.Current != null)
                {
                    AppShell.Current.TogglePaneButtonRectChanged += Current_TogglePaneButtonSizeChanged;
                    titleBar.Margin = new Thickness(AppShell.Current.TogglePaneButtonRect.Right, 0, 0, 0);
                }
            };
        }

        /// <summary>
        /// Gets or sets the gold balance.
        /// </summary>
        public int GoldBalance
        {
            get { return (int)GetValue(GoldBalanceProperty); }
            set { SetValue(GoldBalanceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the header content.
        /// </summary>
        public UIElement HeaderContent
        {
            get { return (UIElement)GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        /// <summary>
        /// Gets the ViewModel.
        /// </summary>
        public PageHeaderViewModel ViewModel { get; }

        private void Current_TogglePaneButtonSizeChanged(AppShell sender, Rect e)
        {
            titleBar.Margin = new Thickness(e.Right, 0, 0, 0);
        }

        private static void GoldBalancePropertyChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var pageHeaderView = dependencyObject as PageHeader;
            pageHeaderView?.OnGoldBalanceChanged();
        }

        private async void OnGoldBalanceChanged()
        {
            // Suppress playing the gold counter animation when
            // data binding is set up.
            if (!_goldCounterBindingInitialized)
            {
                _goldCounterBindingInitialized = true;
            }
            else
            {
                var action = new Action(() =>
                {
                    goldButton.PlayMovingCoinAnimation();
                    flipStoryboard.Begin();
                });

                // The gold counter change can be triggered by non-UI
                // components (push notifications), so we need to make sure
                // we do the UI update on the right thread.
                if (Dispatcher.HasThreadAccess)
                {
                    action();
                }
                else
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () => action());
                }
            }
        }
    }
}