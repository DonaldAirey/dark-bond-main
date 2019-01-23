// <copyright file="DropDownButtonView.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// A button with a drop down control.
    /// </summary>
    [TemplatePart(Name = DropDownButtonView.PartButton, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = DropDownButtonView.PartDropDown, Type = typeof(FrameworkElement))]
    public class DropDownButtonView : MenuItemView
    {
        /// <summary>
        /// Identifies the Button dependency property key.
        /// </summary>
        public static readonly DependencyProperty ButtonProperty = DependencyProperty.Register("Button", typeof(object), typeof(DropDownButtonView));

        /// <summary>
        /// Identifies the IsButtonPressed dependency property.
        /// </summary>
        public static readonly DependencyProperty IsButtonPressedProperty;

        /// <summary>
        /// Identifies the IsMouseOverButton dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMouseOverButtonProperty;

        /// <summary>
        /// Identifies the IsMouseOverDropDown dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMouseOverDropDownProperty;

        /// <summary>
        /// Identifies the event used to signal that the button has been clicked.
        /// </summary>
        public static readonly RoutedEvent PreviewClickEvent = EventManager.RegisterRoutedEvent(
            "PreviewClick",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(DropDownButtonView));

        /// <summary>
        /// The name of the button part of the control.
        /// </summary>
        private const string PartButton = "PART_Button";

        /// <summary>
        /// The name of the drop down (menu item) part of the control.
        /// </summary>
        private const string PartDropDown = "PART_DropDown";

        /// <summary>
        /// Identifies the IsMouseOverButton dependency property key.
        /// </summary>
        private static readonly DependencyPropertyKey IsMouseOverButtonPropertyKey = DependencyProperty.RegisterReadOnly(
            "IsMouseOverButton",
            typeof(bool),
            typeof(DropDownButtonView),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies the IsButtonPressed dependency property key.
        /// </summary>
        private static DependencyPropertyKey isButtonPressedPropertyKey = DependencyProperty.RegisterReadOnly(
            "IsButtonPressed",
            typeof(bool),
            typeof(DropDownButtonView),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies the IsMouseOverDropDown dependency property key.
        /// </summary>
        private static DependencyPropertyKey isMouseOverDropDownPropertyKey = DependencyProperty.RegisterReadOnly(
            "IsMouseOverDropDown",
            typeof(bool),
            typeof(DropDownButtonView),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// The part button part of the control.
        /// </summary>
        private UIElement buttonElement;

        /// <summary>
        /// The drop down part of the control.
        /// </summary>
        private UIElement dropDownElement;

        /// <summary>
        /// Initializes static members of the <see cref="DropDownButtonView"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "There is no other way to initialize metadata.")]
        static DropDownButtonView()
        {
            // This allows the templates in resource files to be properly associated with this new class.  Without this override, the type of the
            // base class would be used as the key in any lookup involving resources dictionaries.
            DropDownButtonView.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DropDownButtonView),
                new FrameworkPropertyMetadata(typeof(DropDownButtonView)));

            // These properties are extracted from the property keys in the static constructor because there is no guarantee of the order when the
            // are initialized using the fields.  That is, if a key is declared before the property, then it will initialized properly.  If the key
            // is declared after the property, it will not because of the forward reference.  This code pattern guarantees the proper initialization
            // of the properties no matter what the order of the fields in the declaration.
            DropDownButtonView.IsButtonPressedProperty = DropDownButtonView.isButtonPressedPropertyKey.DependencyProperty;
            DropDownButtonView.IsMouseOverButtonProperty = DropDownButtonView.IsMouseOverButtonPropertyKey.DependencyProperty;
            DropDownButtonView.IsMouseOverDropDownProperty = DropDownButtonView.isMouseOverDropDownPropertyKey.DependencyProperty;
        }

        /// <summary>
        /// Gets or sets the button.
        /// </summary>
        public object Button
        {
            get
            {
                return this.GetValue(DropDownButtonView.ButtonProperty);
            }

            set
            {
                this.SetValue(DropDownButtonView.ButtonProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the button is pressed.
        /// </summary>
        public bool IsButtonPressed
        {
            get
            {
                return (bool)this.GetValue(DropDownButtonView.IsButtonPressedProperty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the mouse is over the button part of the control.
        /// </summary>
        public bool IsMouseOverButton
        {
            get
            {
                return (bool)this.GetValue(DropDownButtonView.IsMouseOverButtonProperty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the mouse is over the drop down button part of the control.
        /// </summary>
        public bool IsMouseOverDropDown
        {
            get
            {
                return (bool)this.GetValue(DropDownButtonView.IsMouseOverDropDownProperty);
            }
        }

        /// <summary>
        /// Called when the template's tree is generated.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // The general idea is to split a single menu item control into a button and a menu item.  The button part will respond like a button, and the
            // menu part will respond like a menu.
            this.dropDownElement = this.GetTemplateChild(DropDownButtonView.PartDropDown) as UIElement;
            this.buttonElement = this.GetTemplateChild(DropDownButtonView.PartButton) as UIElement;

            // Allow the base class to handle the rest of the method.
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Exits the menu mode and executes any commands associated with the button.
        /// </summary>
        /// <param name="commandSource">The source of the command to be executed.</param>
        protected void ExitMenuMode(ICommandSource commandSource)
        {
            // This algorithm was backward engineered from the MenuItem and MenuBase in the .NET code.  The menu item, now acting like a button,
            // raises an event that indicates that a button was clicked.  The MenuView will catch this event and shut down the submenus.  The menu
            // items should never close themselves down as there are fields and properties in the menu that need to be reconciled.
            this.RaiseEvent(new RoutedEventArgs(DropDownButtonView.PreviewClickEvent, this));

            // It will confuse the menu system if the command is sent out while the menus are showing the drop down controls so we'll allow the menu
            // to close out the menu item system and then we'll invoke the command associated with this button.
            this.Dispatcher.BeginInvoke((Action<ICommandSource>)this.InvokeClickAfterRender, commandSource);
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Validate the parameters
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // Don't touch the events if the command has already been handled.
            if (!e.Handled)
            {
                // The Enter key is translated into a request to execute the command.
                if (e.Key == Key.Enter && this.dropDownElement == null)
                {
                    this.ExitMenuMode(this);
                    e.Handled = true;
                }
            }

            base.OnKeyDown(e);
        }

        /// <inheritdoc/>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            // Validate the arguments.
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            // Don't touch the events if the command has already been handled.
            if (!e.Handled)
            {
                // If the mouse is over the button part of the control when the left button is clicked this menu item is going to act like a button.
                if (this.ButtonContains(e.GetPosition(this)))
                {
                    // We need to capture the mouse so we'll see the mouse up event.
                    this.CaptureMouse();

                    // This property can be used to distinguish between the menu item being pressed and the button being pressed.
                    this.SetValue(DropDownButtonView.isButtonPressedPropertyKey, true);

                    // This prevents the item from acting like a menu item (where it would normally capture the mouse and set the internal menu
                    // modes).
                    e.Handled = true;
                }
            }

            // Allow the base class to handle the rest of the event.
            base.OnMouseLeftButtonDown(e);
        }

        /// <inheritdoc/>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            // Validate the arguments.
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            // Don't touch the events if the command has already been handled.
            if (!e.Handled)
            {
                // If we owned the mouse capture when the button is released, then we must have captured it in the first place.
                if (Mouse.Captured == this)
                {
                    // Let other controls get the mouse movements.
                    this.ReleaseMouseCapture();

                    // This indicates that the button isn't pressed any longer.
                    this.SetValue(DropDownButtonView.isButtonPressedPropertyKey, false);
                }

                // If the mouse is over the button part of the control when the mouse is released then we'll emulate the actions of a button (or a
                // menu item with no children).
                if (this.ButtonContains(e.GetPosition(this)))
                {
                    // When the button part is clicked, we'll exit the menu mode and then execute the command.  It must be done in this order to
                    // preserve the operational state of the menu system.
                    this.ExitMenuMode(this);

                    // This prevents the menu item class from acting like this was a menu item.
                    e.Handled = true;
                }
            }

            // Allow the base class to handle the rest of the event.
            base.OnMouseLeftButtonUp(e);
        }

        /// <inheritdoc/>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            // Validate parameters
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            // This property will indicate that the mouse is over the drop down part of the control.
            this.SetValue(DropDownButtonView.IsMouseOverButtonPropertyKey, this.ButtonContains(e.GetPosition(this)));

            // This property will indicate that the mouse is over the button part of the control.
            this.SetValue(DropDownButtonView.isMouseOverDropDownPropertyKey, this.DropDownContains(e.GetPosition(this)));

            // The base class has significant processing on this event which must be completed.
            base.OnMouseEnter(e);
        }

        /// <inheritdoc/>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            // The buttons can't be pressed and the mouse isn't over anything if the mouse has left the building.
            this.SetValue(DropDownButtonView.IsMouseOverButtonPropertyKey, false);
            this.SetValue(DropDownButtonView.isMouseOverDropDownPropertyKey, false);

            // Allow the base class to handle the event first.
            base.OnMouseLeave(e);
        }

        /// <inheritdoc/>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Validate the parameters.
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            // This property will indicate that the mouse is over the drop down part of the control.
            this.SetValue(DropDownButtonView.IsMouseOverButtonPropertyKey, this.ButtonContains(e.GetPosition(this)));

            // This property will indicate that the mouse is over the button part of the control.
            this.SetValue(DropDownButtonView.isMouseOverDropDownPropertyKey, this.DropDownContains(e.GetPosition(this)));

            // Allow the base class to handle the rest of the event.
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Determines whether the given point is over the drop down button part of the DropDownButtonView.
        /// </summary>
        /// <param name="point">A point to be tested.</param>
        /// <returns>True if the given point is over the drop down part of the control, false otherwise.</returns>
        private bool DropDownContains(Point point)
        {
            // If there's no drop down element defined, then the control acts like a menu item.
            if (this.dropDownElement == null)
            {
                return true;
            }

            // This will construct the rectangle that contains the drop down part of the control and test to see whether the given point (corrected to
            // use the origin of the element) lies within the control.
            Rect rect = new Rect(this.dropDownElement.TranslatePoint(default(Point), this), this.dropDownElement.RenderSize);
            return rect.Contains(point);
        }

        /// <summary>
        /// Determines whether the given point is over the button part of the DropDownButtonView.
        /// </summary>
        /// <param name="point">A point to be tested.</param>
        /// <returns>True if the given point is over the drop down part of the control, false otherwise.</returns>
        private bool ButtonContains(Point point)
        {
            // If there is no button element defined, then the control acts like a menu item.
            if (this.buttonElement == null)
            {
                return false;
            }

            // This will construct the rectangle that contains the button part of the control and test to see whether the given point (corrected to
            // use the origin of the element) lies within the control.
            Rect rect = new Rect(this.buttonElement.TranslatePoint(default(Point), this), this.buttonElement.RenderSize);
            return rect.Contains(point);
        }

        /// <summary>
        /// Executes the command in the given source.
        /// </summary>
        /// <param name="commandSource">The command source.</param>
        private void InvokeClickAfterRender(ICommandSource commandSource)
        {
            // This sends out a generic click that makes this look just like a button.
            this.RaiseEvent(new RoutedEventArgs(DropDownButtonView.ClickEvent, this));

            // This is the specific part of the command generator where we invoke the command.
            if (commandSource != null)
            {
                ICommand command = commandSource.Command;
                if (command != null)
                {
                    // Extract the parameter and the target from the command source.
                    object commandParameter = commandSource.CommandParameter;
                    IInputElement commandTarget = commandSource.CommandTarget;

                    // This control supports both routed and bubbled commands.
                    RoutedCommand routedCommand = command as RoutedCommand;
                    if (routedCommand != null)
                    {
                        // If the command is a routed command, then route to the target.  If no target was specified, then the source is assumed to be
                        // the target.  This is consistent with the .NET implementation in the Button and MenuItem class.
                        if (commandTarget == null)
                        {
                            commandTarget = commandSource as IInputElement;
                        }

                        // Route the command directly to the target.
                        if (routedCommand.CanExecute(commandParameter, commandTarget))
                        {
                            routedCommand.Execute(commandParameter, commandTarget);
                        }
                    }
                    else
                    {
                        // Otherwise we'll bubble the command up the visual tree (or through delegates, whatever).
                        if (command.CanExecute(commandParameter))
                        {
                            command.Execute(commandParameter);
                        }
                    }
                }
            }
        }
    }
}