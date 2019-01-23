// <copyright file="MenuItemView.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    /// <summary>
    /// A working version of the MenuItem designed to work with MVVM menus.
    /// </summary>
    public class MenuItemView : MenuItem, ICommandSource
    {
        /// <summary>
        /// Identifies the Command dependency property.
        /// </summary>
        public static new readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(MenuItemView),
            new FrameworkPropertyMetadata(null, MenuItemView.OnCommandChanged));

        /// <summary>
        /// Identifies the CommandProperty dependency property.
        /// </summary>
        public static new readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter",
            typeof(object),
            typeof(MenuItemView),
            new FrameworkPropertyMetadata(null, MenuItemView.OnCommandParameterChanged));

        /// <summary>
        /// Identifies the CommandTarget dependency property.
        /// </summary>
        public static new readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(
            "CommandTarget",
            typeof(IInputElement),
            typeof(MenuItemView));

        /// <summary>
        /// Indicates that this menu item can be executed.
        /// </summary>
        private bool canExecuteField = true;

        /// <summary>
        /// Initializes static members of the <see cref="MenuItemView"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "There is no other way to override metadata")]
        static MenuItemView()
        {
            // This allows the templates in resource files to be properly associated with this new class.  Without this override, the type of the
            // base class would be used as the key in any lookup involving resources dictionaries.
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuItemView), new FrameworkPropertyMetadata(typeof(MenuItemView)));

            // These controls are designed to work as view models and the ability to use container templates is a critical part of associating the
            // view models with the views.
            MenuItem.UsesItemContainerTemplateProperty.OverrideMetadata(typeof(MenuItemView), new FrameworkPropertyMetadata(true));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItemView"/> class.
        /// </summary>
        public MenuItemView()
        {
            // In order to fix a bug in the architecture of the menu system, we're going to inhibit the evaluation of whether the command can execute
            // until after this control has been loaded.  This fixes the issue where the command evaluation will fire as soon as the command is set
            // and not wait for the command parameter to be set.
            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// Gets or sets the command that will be executed when the command source is invoked.
        /// </summary>
        public new ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(MenuItemView.CommandProperty);
            }

            set
            {
                this.SetValue(MenuItemView.CommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the user defined data value that can be passed to the command when it is executed.
        /// </summary>
        public new object CommandParameter
        {
            get
            {
                return this.GetValue(MenuItemView.CommandParameterProperty);
            }

            set
            {
                this.SetValue(MenuItemView.CommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the object that the command is being executed on.
        /// </summary>
        public new IInputElement CommandTarget
        {
            get
            {
                return (IInputElement)this.GetValue(MenuItemView.CommandTargetProperty);
            }

            set
            {
                this.SetValue(MenuItemView.CommandTargetProperty, value);
            }
        }

        /// <inheritdoc/>
        protected override bool IsEnabledCore
        {
            get
            {
                return this.CanExecute;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the menu item can be executed.
        /// </summary>
        private bool CanExecute
        {
            get
            {
                return this.canExecuteField;
            }

            set
            {
                if (value != this.canExecuteField)
                {
                    this.canExecuteField = value;

                    // It's critical that the base class re-evaluates the IsEnabled property after the 'CanExecute' property has changed.
                    this.CoerceValue(UIElement.IsEnabledProperty);
                }
            }
        }

        /// <summary>
        /// Invoked when the effective property value of the 'Property' dependency property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static void OnCommandChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Pull the target object out of the generic parameters and call the instance to handle the parameter change.
            MenuItemView contextMenuItem = dependencyObject as MenuItemView;
            contextMenuItem.OnCommandChanged(
                (ICommand)dependencyPropertyChangedEventArgs.OldValue,
                (ICommand)dependencyPropertyChangedEventArgs.NewValue);
        }

        /// <summary>
        /// Invoked when the effective property value of the 'CommandParameter' dependency property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static void OnCommandParameterChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Update the state of the 'CanExecute' property when the command parameter changes.  This is the primary reason for this class: to fix
            // the problem that the command property never updates properly when the bindings are applied in an MVVM scenario.
            MenuItemView contextMenuItem = dependencyObject as MenuItemView;
            contextMenuItem.UpdateCanExecute();
        }

        /// <summary>
        /// Hook the command into the global event handler.
        /// </summary>
        /// <param name="command">The new command to be enabled.</param>
        private void HookCommand(ICommand command)
        {
            // Hook the command into the global event handler.
            CanExecuteChangedEventManager.AddHandler(command, new EventHandler<EventArgs>(this.OnCanExecuteChanged));

            // Update the status of the menu item.
            this.UpdateCanExecute();
        }

        /// <summary>
        /// Handes the loading of the control.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="e">The event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Both the Command and the CommandParameter are needed to give a proper context to a commanding event.  When initializing the object,
            // we're going to inhibit any updates to the 'CanExecute' status of this item.  Once all the properties have been set and the MenuItem is
            // ready for business, then we'll update the status.  This prevents the situation where you get a Command updating it's status with no
            // parameter data when you've clearly set the parameter in the XAML.
            this.UpdateCanExecute();
        }

        /// <summary>
        /// Invoked when the 'CanExecute' status of this item has changed.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="eventArgs">The event data.</param>
        private void OnCanExecuteChanged(object sender, EventArgs eventArgs)
        {
            this.UpdateCanExecute();
        }

        /// <summary>
        /// Handles a change to the Command property.
        /// </summary>
        /// <param name="oldCommand">The old command.</param>
        /// <param name="newCommand">The new command.</param>
        private void OnCommandChanged(ICommand oldCommand, ICommand newCommand)
        {
            // Unhook the event handlers for the old command.
            if (oldCommand != null)
            {
                this.UnhookCommand(oldCommand);
            }

            // Hook in the handlers for the new command.
            if (newCommand != null)
            {
                this.HookCommand(newCommand);
            }

            // The key hint that appears in the header the input gesture associated with the new command need to be updated.
            this.CoerceValue(HeaderedItemsControl.HeaderProperty);
            this.CoerceValue(MenuItem.InputGestureTextProperty);
        }

        /// <summary>
        /// Unhook the command from the global event handler.
        /// </summary>
        /// <param name="command">The old command to be disabled.</param>
        private void UnhookCommand(ICommand command)
        {
            // Unhook the command from the global event handler.
            CanExecuteChangedEventManager.RemoveHandler(command, new EventHandler<EventArgs>(this.OnCanExecuteChanged));

            // Update the status of the menu item.
            this.UpdateCanExecute();
        }

        /// <summary>
        /// Updates the 'CanExecute' status of this item.
        /// </summary>
        private void UpdateCanExecute()
        {
            // If there is no command associated with this MenuItemView, then the command will always appear to be enabled.  Otherwise we need to
            // check the source of the command to see if it is enabled.
            if (this.IsLoaded && this.Command != null)
            {
                // If this is a submenu, or if the submenu is open, we're going to assume that the control is enabled, even if it has a command
                // associated with it.  Otherwise, we'll ask the command handler if this command (with the current command parameter) can be
                // executed.
                MenuItem parentMenuItem = ItemsControl.ItemsControlFromItemContainer(this) as MenuItem;
                if (parentMenuItem == null || parentMenuItem.IsSubmenuOpen)
                {
                    this.CanExecute = this.CanExecuteCommandSource();
                }
                else
                {
                    // Submenus and menus with open submenus can always be executed.
                    this.CanExecute = true;
                }
            }
            else
            {
                // MenuItems without a command will always appear to be enabled.
                this.CanExecute = true;
            }
        }

        /// <summary>
        /// Determines from the source of the command if it can be executed.
        /// </summary>
        /// <returns>true if the command associated with this MenuItem can be executed, false otherwise.</returns>
        private bool CanExecuteCommandSource()
        {
            // If the command is bound to a property, then update it before deciding to execute the command.  When MenuItems are garbage collected,
            // they can still be connected to the CommandManager and may receive some post-mortem request to update their status.  This will prevent
            // a dead MenuItem from trying to query its target.
            BindingExpression commandBindingExpression = BindingOperations.GetBindingExpression(this, MenuItemView.CommandProperty);
            if (commandBindingExpression != null)
            {
                commandBindingExpression.UpdateTarget();
            }

            // If there is no command source, then the command can't be executed.
            ICommand command = this.Command;
            if (command == null)
            {
                return false;
            }

            // The general idea here is to figure out if the command is a simple or routed command.  We'll ask the target directly if the command
            // executes when we have a simple command.
            RoutedCommand routedCommand = command as RoutedCommand;
            if (routedCommand == null)
            {
                return command.CanExecute(this.CommandParameter);
            }

            // Routed commands will be, well, routed through the visual tree to see if there is a command handler that can handle the command.  If no
            // target is given, then this MenuItem will act as a target.
            return routedCommand.CanExecute(this.CommandParameter, this.CommandTarget == null ? this as IInputElement : this.CommandTarget);
        }
    }
}