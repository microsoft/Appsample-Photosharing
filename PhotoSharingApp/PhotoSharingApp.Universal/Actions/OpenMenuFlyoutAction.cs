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

using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace PhotoSharingApp.Universal.Actions
{
    /// <summary>
    /// Opens the attached menu flyout.
    /// </summary>
    public class OpenMenuFlyoutAction : DependencyObject, IAction
    {
        /// <summary>
        /// The dependency property for the <see cref="IsEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
            "IsEnabled", typeof(bool), typeof(OpenMenuFlyoutAction), new PropertyMetadata(true));

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="T:System.Object" /> that is passed to the action by the behavior. Generally this is
        /// <seealso cref="P:Microsoft.Xaml.Interactivity.IBehavior.AssociatedObject" /> or a target object.
        /// </param>
        /// <param name="parameter">The value of this parameter is determined by the caller.</param>
        /// <remarks>
        /// An example of parameter usage is EventTriggerBehavior, which passes the EventArgs as a parameter to its actions.
        /// </remarks>
        /// <returns>
        /// Returns the result of the action.
        /// </returns>
        public object Execute(object sender, object parameter)
        {
            if (IsEnabled)
            {
                var senderElement = sender as FrameworkElement;
                var flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);

                flyoutBase.ShowAt(senderElement);
            }

            return null;
        }

        /// <summary>
        /// Gets or sets whether the flyout is being displayed.
        /// </summary>
        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }
    }
}