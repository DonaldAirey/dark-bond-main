// <copyright file="MenuView.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Represents a Windows menu control with an overflow panel.
    /// </summary>
    public class MenuView : Menu
    {
        /// <summary>
        /// Initializes static members of the <see cref="MenuView"/> class.
        /// </summary>
        static MenuView()
        {
            // This allows MenuView instances to find their implicit styles in the themes.
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuView), new FrameworkPropertyMetadata(typeof(MenuView)));

            // This control is intended to be used with View Models so we want to use ItemContainerTemplates by default.
            Menu.UsesItemContainerTemplateProperty.OverrideMetadata(typeof(MenuView), new FrameworkPropertyMetadata(true));

            // These events are handled by this class.
            EventManager.RegisterClassHandler(
                typeof(MenuView),
                DropDownButtonView.PreviewClickEvent,
                new RoutedEventHandler(MenuView.OnMenuItemPreviewClick));
        }

        /// <summary>
        /// Handles the preview of the click event on an item acting like a button.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private static void OnMenuItemPreviewClick(object sender, RoutedEventArgs e)
        {
            // Extract the specific arguments.
            MenuView menuView = sender as MenuView;

            // Here's the short story: menu items don't like to be interrupted.  When a submenu is opened, the class is structured so that you can
            // only click on a submenu item or menu item (these are items with no child items) to dismiss it.  Any item with children is considered a
            // 'header' and is used as a gateway to get to the children.  The headers are never clicked themselves.  So when we try to use the menus
            // and menu items for other controls, it leaves the menu in this strange state that makes the drop downs appear whenever the mouse is
            // over them.  The 'MenuMode' needs to be reset when we manually close down the control.  Unfortunately, there's a bug with the MenuItem
            // class.  Setting the 'IsSubmenuOpen' property to false doesn't clear this internal state.  However, it can be forced closed with a
            // synthetic MouseUp event.
            menuView.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = Mouse.MouseUpEvent });
        }
    }
}