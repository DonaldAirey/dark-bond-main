// <copyright file="ViewButton.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Markup;

    /// <summary>
    /// A button with one or more states.
    /// </summary>
    [ContentProperty(Name = "States")]
    public class ViewButton : FrameButton
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
            typeof(ICollection<ButtonState>),
            typeof(ViewButton),
            new PropertyMetadata(null));

        /// <summary>
        /// The icon that holds the currently selected image.
        /// </summary>
        private BitmapIcon bitmapIcon = new BitmapIcon();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewButton"/> class.
        /// </summary>
        public ViewButton()
        {
            // Initialize the object.
            ObservableCollection<ButtonState> list = new ObservableCollection<ButtonState>();
            this.SetValue(ViewButton.StatesProperty, list);
            ObservableCollection<ButtonState> buttonStates = this.States as ObservableCollection<ButtonState>;
            buttonStates.CollectionChanged += this.OnStatesChanged;

            // Give the button an icon that will change as the states change.
            this.Content = this.bitmapIcon;
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
        public ICollection<ButtonState> States
        {
            get
            {
                return this.GetValue(ViewButton.StatesProperty) as ICollection<ButtonState>;
            }
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
            ViewButton viewButton = dependencyObject as ViewButton;
            string state = dependencyPropertyChangedEventArgs.NewValue as string;

            // Populate the button's command properties (and visuals) with the values from the newly selected ViewButton.
            ObservableCollection<ButtonState> buttonStates = viewButton.States as ObservableCollection<ButtonState>;
            ButtonState buttonState = buttonStates.FirstOrDefault<ButtonState>((bs) => bs.State == state);
            if (buttonState != default(ButtonState))
            {
                // Once we've given the button state - that isn't really connected to a visual tree hierarchy - a data context, we can then extract
                // the parameters (including the command binding) and make them part of the view button.
                buttonState.DataContext = viewButton.DataContext;
                viewButton.Command = buttonState.Command;
                viewButton.CommandParameter = buttonState.CommandParameter;
                viewButton.bitmapIcon.UriSource = buttonState.UriSource;
            }
        }

        /// <summary>
        /// Invoked when a state has been added to the collection.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="e">The event data.</param>
        private void OnStatesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // It's possible that the state may have been set before all the states were loaded.  When that happens, Populate the button's command
            // properties (and visuals) with the values from the newly added ViewButtonState.
            ObservableCollection<ButtonState> buttonStates = this.States as ObservableCollection<ButtonState>;
            ButtonState buttonState = buttonStates.FirstOrDefault<ButtonState>((bs) => bs.State == this.State);
            if (buttonState != default(ButtonState))
            {
                this.Command = buttonState.Command;
                this.CommandParameter = buttonState.CommandParameter;
                this.bitmapIcon.UriSource = buttonState.UriSource;
            }
        }
    }
}