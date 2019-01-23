// <copyright file="ViewButton.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// A button with one or more states.
    /// </summary>
    [ContentProperty("States")]
    public class ViewButton : Fluent.Button
    {
        /// <summary>
        /// Identifies the State DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State",
            typeof(string),
            typeof(ViewButton),
            new PropertyMetadata(null, ViewButton.OnStatePropertyChanged));

        /// <summary>
        /// Identifies the States DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty StatesProperty = DependencyProperty.Register(
            "States",
            typeof(ObservableCollection<ButtonState>),
            typeof(ViewButton),
            new PropertyMetadata(null));

        /// <summary>
        /// The icon that holds the currently selected image.
        /// </summary>
        private System.Windows.Controls.Image image = new System.Windows.Controls.Image();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewButton"/> class.
        /// </summary>
        public ViewButton()
        {
            // Initialize the object.
            this.SetValue(ViewButton.StatesProperty, new ObservableCollection<ButtonState>());

            // Give the button an image that will change as the states change.
            this.image.Stretch = Stretch.None;
            this.LargeIcon = this.image;
        }

        /// <summary>
        /// Gets or sets the state of the button.
        /// </summary>
        public string State
        {
            get
            {
                return this.GetValue(ViewButton.StateProperty) as string;
            }

            set
            {
                this.SetValue(ViewButton.StateProperty, value);
            }
        }

        /// <summary>
        /// Gets the states of this button.
        /// </summary>
        public ObservableCollection<ButtonState> States
        {
            get
            {
                return this.GetValue(ViewButton.StatesProperty) as ObservableCollection<ButtonState>;
            }
        }

        /// <inheritdoc/>
        protected override void OnInitialized(EventArgs e)
        {
            // Reconcile the button properties to the state.
            this.SetState();

            // Allow the base class to handle the rest of the event.
            base.OnInitialized(e);
        }

        /// <summary>
        /// Invoked when the effective property value of the State property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static void OnStatePropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Reconcile the button properties to the state.
            ViewButton viewButton = dependencyObject as ViewButton;
            viewButton.SetState();
        }

        /// <summary>
        /// Invoked when a state has been added to the collection.
        /// </summary>
        private void SetState()
        {
            // It's possible that the state may have been set before all the states were loaded.  When that happens, Populate the button's command
            // properties (and visuals) with the values from the newly added ViewButtonState.
            ObservableCollection<ButtonState> buttonStates = this.States as ObservableCollection<ButtonState>;
            ButtonState buttonState = buttonStates.FirstOrDefault<ButtonState>((bs) => bs.State == this.State);
            if (buttonState != default(ButtonState))
            {
                buttonState.DataContext = this.DataContext;
                this.Command = buttonState.Command;
                this.CommandParameter = buttonState.CommandParameter;
                this.Header = buttonState.Header;
                this.image.Source = new BitmapImage(buttonState.Uri);
            }
        }
    }
}