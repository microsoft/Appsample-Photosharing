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
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace PhotoSharingApp.Universal.Controls
{
    /// <summary>
    /// A specialized ListView to represent the items in the navigation menu.
    /// </summary>
    /// <remarks>
    /// This class handles the following:
    /// 1. Sizes the panel that hosts the items so they fit in the hosting pane.  Otherwise, the keyboard
    /// may appear cut off on one side b/c the Pane clips instead of affecting layout.
    /// 2. Provides a single selection experience where keyboard focus can move without changing selection.
    /// Both the 'Space' and 'Enter' keys will trigger selection.  The up/down arrow keys can move
    /// keyboard focus without triggering selection.  This is different than the default behavior when
    /// SelectionMode == Single.  The default behavior for a ListView in single selection requires using
    /// the Ctrl + arrow key to move keyboard focus without triggering selection.  Users won't expect
    /// this type of keyboarding model on the nav menu.
    /// </remarks>
    public class NavMenuListView : ListView
    {
        private SplitView _splitViewHost;

        public NavMenuListView()
        {
            SelectionMode = ListViewSelectionMode.Single;
            IsItemClickEnabled = true;
            ItemClick += ItemClickedHandler;

            Loaded += (s, a) =>
            {
                // Locate the hosting SplitView control
                var parent = VisualTreeHelper.GetParent(this);

                while (parent != null && !(parent is SplitView))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }

                if (parent != null)
                {
                    _splitViewHost = parent as SplitView;

                    _splitViewHost.RegisterPropertyChangedCallback(SplitView.IsPaneOpenProperty,
                        (sender, args) => { OnPaneToggled(); });

                    // Call once to ensure we're in the correct state
                    OnPaneToggled();
                }
            };
        }

        private void InvokeItem(object focusedItem)
        {
            SetSelectedItem(focusedItem as ListViewItem);
            ItemInvoked?.Invoke(this, focusedItem as ListViewItem);

            if (_splitViewHost.IsPaneOpen && (
                _splitViewHost.DisplayMode == SplitViewDisplayMode.CompactOverlay ||
                _splitViewHost.DisplayMode == SplitViewDisplayMode.Overlay))
            {
                _splitViewHost.IsPaneOpen = false;
                if (focusedItem is ListViewItem)
                {
                    ((ListViewItem)focusedItem).Focus(FocusState.Programmatic);
                }
            }
        }

        private void ItemClickedHandler(object sender, ItemClickEventArgs e)
        {
            // Triggered when the item is selected using 
            // something other than a keyboard
            var item = ContainerFromItem(e.ClickedItem);
            InvokeItem(item);
        }

        /// <summary>
        /// Occurs when an item has been selected
        /// </summary>
        public event EventHandler<ListViewItem> ItemInvoked;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Remove the entrance animation on the item containers.
            for (var i = 0; i < ItemContainerTransitions.Count; i++)
            {
                if (ItemContainerTransitions[i] is EntranceThemeTransition)
                {
                    ItemContainerTransitions.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Custom keyboarding logic to enable movement via the arrow keys without triggering selection
        /// until a 'Space' or 'Enter' key is pressed.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            var focusedItem = FocusManager.GetFocusedElement();

            switch (e.Key)
            {
                case VirtualKey.Up:
                    TryMoveFocus(FocusNavigationDirection.Up);
                    e.Handled = true;
                    break;

                case VirtualKey.Down:
                    TryMoveFocus(FocusNavigationDirection.Down);
                    e.Handled = true;
                    break;

                case VirtualKey.Tab:
                    var shiftKeyState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift);
                    var shiftKeyDown = (shiftKeyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;

                    // If we're on the header item then this will be null and 
                    // we'll still get the default behavior.
                    if (focusedItem is ListViewItem)
                    {
                        var currentItem = (ListViewItem)focusedItem;
                        var onlastitem = currentItem != null && IndexFromContainer(currentItem) == Items.Count - 1;
                        var onfirstitem = currentItem != null && IndexFromContainer(currentItem) == 0;

                        if (!shiftKeyDown)
                        {
                            if (onlastitem)
                            {
                                TryMoveFocus(FocusNavigationDirection.Next);
                            }
                            else
                            {
                                TryMoveFocus(FocusNavigationDirection.Down);
                            }
                        }
                        else // Shift + Tab
                        {
                            if (onfirstitem)
                            {
                                TryMoveFocus(FocusNavigationDirection.Previous);
                            }
                            else
                            {
                                TryMoveFocus(FocusNavigationDirection.Up);
                            }
                        }
                    }
                    else if (focusedItem is Control)
                    {
                        if (!shiftKeyDown)
                        {
                            TryMoveFocus(FocusNavigationDirection.Down);
                        }
                        else // Shift + Tab
                        {
                            TryMoveFocus(FocusNavigationDirection.Up);
                        }
                    }

                    e.Handled = true;
                    break;

                case VirtualKey.Space:
                case VirtualKey.Enter:

                    // Fire our event using the item with current keyboard focus
                    InvokeItem(focusedItem);
                    e.Handled = true;
                    break;

                default:
                    base.OnKeyDown(e);
                    break;
            }
        }

        /// <summary>
        /// Re-size the ListView's Panel when the SplitView is compact so the items
        /// will fit within the visible space and correctly display a keyboard focus rect.
        /// </summary>
        private void OnPaneToggled()
        {
            if (_splitViewHost.IsPaneOpen)
            {
                ItemsPanelRoot.ClearValue(WidthProperty);
                ItemsPanelRoot.ClearValue(HorizontalAlignmentProperty);
            }
            else if (_splitViewHost.DisplayMode == SplitViewDisplayMode.CompactInline ||
                     _splitViewHost.DisplayMode == SplitViewDisplayMode.CompactOverlay)
            {
                ItemsPanelRoot.SetValue(WidthProperty, _splitViewHost.CompactPaneLength);
                ItemsPanelRoot.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left);
            }
        }

        /// <summary>
        /// Mark the <paramref name="item" /> as selected and ensures everything else is not.
        /// If the <paramref name="item" /> is null then everything is unselected.
        /// </summary>
        /// <param name="item">The item to select.</param>
        public void SetSelectedItem(ListViewItem item)
        {
            var index = -1;

            if (item != null)
            {
                index = IndexFromContainer(item);
            }

            for (var i = 0; i < Items.Count; i++)
            {
                var listViewItem = (ListViewItem)ContainerFromIndex(i);

                if (listViewItem != null)
                {
                    if (i != index)
                    {
                        listViewItem.IsSelected = false;
                    }
                    else if (i == index)
                    {
                        listViewItem.IsSelected = true;
                    }
                }
            }
        }

        /// <summary>
        /// This method is a work-around until the bug in FocusManager.TryMoveFocus is fixed.
        /// </summary>
        /// <param name="direction">The focus navigation direction.</param>
        private void TryMoveFocus(FocusNavigationDirection direction)
        {
            if (direction == FocusNavigationDirection.Next || direction == FocusNavigationDirection.Previous)
            {
                FocusManager.TryMoveFocus(direction);
            }
            else
            {
                var control = FocusManager.FindNextFocusableElement(direction) as Control;
                control?.Focus(FocusState.Programmatic);
            }
        }
    }
}