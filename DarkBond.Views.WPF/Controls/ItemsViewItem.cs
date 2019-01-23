// <copyright file="ItemsViewItem.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Provides a container for any object presented in an <see cref="ItemsView"/>.
    /// </summary>
    public class ItemsViewItem : ListBoxItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsViewItem"/> class.
        /// </summary>
        public ItemsViewItem()
        {
            // This allows the view to be styled.
            this.DefaultStyleKey = typeof(ItemsViewItem);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // This will initialize the visual state.
            this.UpdateState();
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Updates the visual states using the parent's visual state.
        /// </summary>
        /// <param name="parent">The parent control.</param>
        public void UpdateState(UIElement parent)
        {
            // The standard visual states are unusable: it contains different visual state groups for selected and focused states that are mutually
            // exclusive.  Our containers combine these states to determine what highlighting is used.
            string state = "Default";
            if (this.IsSelected)
            {
                bool isParentFocusWithin = parent != null && parent.IsKeyboardFocusWithin;
                state = this.IsKeyboardFocusWithin ? "FocusedSelected" : isParentFocusWithin ? "UnfocusedSelected" : "InactiveSelected";
            }
            else
            {
                if (this.IsMouseOver)
                {
                    state = "MouseOverUnselected";
                }

                if (this.IsKeyboardFocusWithin)
                {
                    state = "FocusedUnselected";
                }
            }

            VisualStateManager.GoToState(this, state, false);
        }

        /// <summary>
        /// Invoked when an unhandled Keyboard.GotKeyboardFocus attached event reaches an element in its route.
        /// </summary>
        /// <param name="e">The <see cref="KeyboardFocusChangedEventArgs"/> that contains the event data.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            // Update the visual state when the row gets the keyboard focus.
            this.UpdateState();
            base.OnGotKeyboardFocus(e);
        }

        /// <summary>
        /// Invoked when an unhandled Keyboard.LostKeyboardFocus attached event reaches an element in its route.
        /// </summary>
        /// <param name="e">The <see cref="KeyboardFocusChangedEventArgs"/> that contains the event data.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            // Update the visual state when the row loses the keyboard focus.
            this.UpdateState();
            base.OnLostKeyboardFocus(e);
        }

        /// <summary>
        /// Called when the mouse enters a <see cref="ItemsViewItem"/>.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            this.UpdateState();
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Called when the mouse leaves a <see cref="ItemsViewItem"/>.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            this.UpdateState();
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Called when the <see cref="ItemsViewItem"/> is selected in an <see cref="ItemsView"/>.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnSelected(RoutedEventArgs e)
        {
            // Update the visual state when the item is selected.
            this.UpdateState();
            base.OnSelected(e);
        }

        /// <summary>
        /// Called when the <see cref="ItemsViewItem"/> is unselected in an <see cref="ItemsView"/>.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnUnselected(RoutedEventArgs e)
        {
            // Update the visual state when the item is unselected.
            this.UpdateState();
            base.OnUnselected(e);
        }

        /// <summary>
        /// Updates the visual state of the container.
        /// </summary>
        private void UpdateState()
        {
            // This determines whether the parent ItemsControl has the focus.  If that's the case, then multiple items can be selected and, since
            // they're part of the same parent, they should appear to be active.  When the focus leaves the parent, then they should appear to be
            // selected, but inactive.
            ItemsView itemsView = VisualTreeExtensions.FindParent<ItemsView>(this);
            this.UpdateState(itemsView);
        }
    }
}