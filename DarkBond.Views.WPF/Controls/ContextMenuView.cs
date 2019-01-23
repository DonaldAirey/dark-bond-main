// <copyright file="ContextMenuView.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// An MVVM ContextMenu.
    /// </summary>
    public class ContextMenuView : ContextMenu
    {
        /// <summary>
        /// Initializes static members of the <see cref="ContextMenuView"/> class.
        /// </summary>
        static ContextMenuView()
        {
            // By default, these types of ContextMenu are designed to work with Windows controls (e.g.  MenuItems, Separators), not view models.  As
            // such, they will want to use the ItemContainerTemplate to map the Items to a view model.
            ContextMenu.UsesItemContainerTemplateProperty.OverrideMetadata(typeof(ContextMenuView), new FrameworkPropertyMetadata(true));
        }

        /// <inheritdoc/>
        protected override void OnOpened(RoutedEventArgs e)
        {
            // The main idea here is to update the menu items to reflect a change in state.  For example, if an item can no longer be deleted because
            // of a change to the data model, we need to query the menu item associated with the 'Delete' command to see if it should still be
            // enabled on the menu.  This will parse each item in the menu looking for commands that can be updated.
            foreach (object menuItemObject in this.Items)
            {
                // See if item is a menu item view model.
                MenuItemViewModel menuItemViewModel = menuItemObject as MenuItemViewModel;
                if (menuItemViewModel != null)
                {
                    // If the menu item has a composite command, then run through each of the composite commands and update the status of that
                    // command.
                    CompositeCommand compositeCommand = menuItemViewModel.Command as CompositeCommand;
                    if (compositeCommand != null)
                    {
                        foreach (ICommand command in compositeCommand.RegisteredCommands)
                        {
                            DelegateCommandBase delegateCommandBase = command as DelegateCommandBase;
                            if (delegateCommandBase != null)
                            {
                                delegateCommandBase.RaiseCanExecuteChanged();
                            }
                        }
                    }

                    // If the menu item has a composite command, then run through each of the composite commands and update the status of that
                    // command.
                    DelegateCommand delegateCommand = menuItemViewModel.Command as DelegateCommand;
                    if (delegateCommand != null)
                    {
                        delegateCommand.RaiseCanExecuteChanged();
                    }
                }
            }

            // We don't want to interrupt the process.
            base.OnOpened(e);
        }
    }
}