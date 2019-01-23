// <copyright file="DirectoryViewBase.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Diagnostics.CodeAnalysis;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Displays items in a variety of views: thumbnail, detail or columnar.
    /// </summary>
    public class DirectoryViewBase : ContentControl
    {
        /// <summary>
        /// Identifies the View dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ViewProperty = DependencyProperty.Register(
            "View",
            typeof(string),
            typeof(DirectoryViewBase),
            new PropertyMetadata(int.MinValue, DirectoryViewBase.OnViewPropertyChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryViewBase"/> class.
        /// </summary>
        public DirectoryViewBase()
        {
            // The contents of a directory will fill all the space available by default.
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.VerticalContentAlignment = VerticalAlignment.Stretch;
        }

        /// <summary>
        /// Gets or sets the index of the current view.
        /// </summary>
        public string View
        {
            get
            {
                return this.GetValue(DirectoryViewBase.ViewProperty) as string;
            }

            set
            {
                this.SetValue(DirectoryViewBase.ViewProperty, value);
            }
        }

        /// <summary>
        /// Handles a change to the state of the IsProductView property.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">Event data that tracks changes to the effective value of this property.</param>
        private static void OnViewPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Set the view to the given index.
            DirectoryViewBase directoryViewBase = dependencyObject as DirectoryViewBase;
            directoryViewBase.SetView(dependencyPropertyChangedEventArgs.NewValue as string);
        }

        /// <summary>
        /// Change the view.
        /// </summary>
        /// <param name="view">The new view value.</param>
        private void SetView(string view)
        {
            // Set the view.
            this.View = view;
        }
    }
}