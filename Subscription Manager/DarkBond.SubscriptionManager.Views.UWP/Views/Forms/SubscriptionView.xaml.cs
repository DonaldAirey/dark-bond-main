// <copyright file="SubscriptionView.xaml.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Views.Forms
{
    using System;
    using System.Composition;
    using System.Diagnostics.CodeAnalysis;
    using DarkBond.SubscriptionManager.ViewModels;
    using DarkBond.SubscriptionManager.ViewModels.Forms;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Interaction logic for <see cref="SubscriptionView"/>.
    /// </summary>
    public partial class SubscriptionView : Page
    {
        /// <summary>
        /// Identifies the ViewState dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ViewStateProperty = DependencyProperty.Register(
            "ViewState",
            typeof(SubscriptionViewState),
            typeof(SubscriptionView),
            new PropertyMetadata(SubscriptionViewState.License, SubscriptionView.OnViewStatePropertyChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionView"/> class.
        /// </summary>
        /// <param name="subscriptionViewModel">The subscription view model.</param>
        [ImportingConstructor]
        public SubscriptionView(SubscriptionViewModel subscriptionViewModel)
        {
            // Validate the subscriptionViewModel argument.
            if (subscriptionViewModel == null)
            {
                throw new ArgumentNullException(nameof(subscriptionViewModel));
            }

            // Initialize the object
            this.DataContext = subscriptionViewModel;

            // Initialize the IDE managed resources.
            this.InitializeComponent();

            // The ViewState in the view is automatically bound to the view model since there's no easy way to do it in the XAML for a user control
            // or page.  From now on, the view model can control the visual state of the page.
            BindingOperations.SetBinding(
                this,
                SubscriptionView.ViewStateProperty,
                new Binding() { Mode = BindingMode.TwoWay, Path = new PropertyPath("ViewState") });
        }

        /// <summary>
        /// Gets or sets the SubscriptionViewState which controls the configuration of controls in the form.
        /// </summary>
        public SubscriptionViewState ViewState
        {
            get
            {
                return (SubscriptionViewState)this.GetValue(SubscriptionView.ViewStateProperty);
            }

            set
            {
                this.SetValue(SubscriptionView.ViewStateProperty, value);
            }
        }

        /// <summary>
        /// Handles a change to the state of the IsOfferingView property.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">Event data that that tracks changes to the effective value of this property.</param>
        private static void OnViewStatePropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            SubscriptionView subscriptionView = dependencyObject as SubscriptionView;
            VisualStateManager.GoToState(subscriptionView, subscriptionView.ViewState.ToString(), true);
        }
    }
}