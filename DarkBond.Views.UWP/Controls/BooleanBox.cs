// <copyright file="BooleanBox.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A visual cue that is either on or off.
    /// </summary>
    public class BooleanBox : Control
    {
        /// <summary>
        /// The IsSet DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty IsSetProperty = DependencyProperty.Register(
            "IsSet",
            typeof(bool),
            typeof(BooleanBox),
            new PropertyMetadata(true, BooleanBox.OnIsSetPropertyChanged));

        /// <summary>
        /// The SetSourcUri DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty SetUriSourceProperty = DependencyProperty.Register(
            "SetUriSource",
            typeof(Uri),
            typeof(BooleanBox),
            new PropertyMetadata(default(Uri)));

        /// <summary>
        /// The UnsetUriSource DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty UnsetUriSourceProperty = DependencyProperty.Register(
            "UnsetUriSource",
            typeof(Uri),
            typeof(BooleanBox),
            new PropertyMetadata(default(Uri)));

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanBox"/> class.
        /// </summary>
        public BooleanBox()
        {
            // This allows the view to be styled.
            this.DefaultStyleKey = typeof(BooleanBox);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the box is set or unset.
        /// </summary>
        public bool IsSet
        {
            get
            {
                return (bool)this.GetValue(BooleanBox.IsSetProperty);
            }

            set
            {
                this.SetValue(BooleanBox.IsSetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the URI for the set icon.
        /// </summary>
        public Uri SetUriSource
        {
            get
            {
                return this.GetValue(BooleanBox.SetUriSourceProperty) as Uri;
            }

            set
            {
                this.SetValue(BooleanBox.SetUriSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the URI for the unset icon.
        /// </summary>
        public Uri UnsetUriSource
        {
            get
            {
                return this.GetValue(BooleanBox.UnsetUriSourceProperty) as Uri;
            }

            set
            {
                this.SetValue(BooleanBox.UnsetUriSourceProperty, value);
            }
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            // The visual states can only be set when there is a template to recognize the states.
            VisualStateManager.GoToState(this, this.IsSet ? "Set" : "Unset", true);

            // Allow the base class to handle the rest of the event.
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Invoked when the effective property value of the IsSet property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static void OnIsSetPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Select a source for the image based on the new category.
            BooleanBox booleanBox = dependencyObject as BooleanBox;
            bool isSet = (bool)dependencyPropertyChangedEventArgs.NewValue;
            VisualStateManager.GoToState(booleanBox, isSet ? "Set" : "Unset", true);
        }
    }
}