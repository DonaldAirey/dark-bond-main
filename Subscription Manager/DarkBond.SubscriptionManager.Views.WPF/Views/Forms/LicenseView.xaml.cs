// <copyright file="LicenseView.xaml.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.Views.Forms
{
    using System;
    using System.Composition;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using DarkBond.LicenseManager.ViewModels;
    using DarkBond.LicenseManager.ViewModels.Forms;

    /// <summary>
    /// Interaction logic for <see cref="LicenseView"/>.
    /// </summary>
    public partial class LicenseView : Page
    {
        /// <summary>
        /// Identifies the ViewState dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewStateProperty = DependencyProperty.Register(
            "ViewState",
            typeof(LicenseViewState),
            typeof(LicenseView),
            new PropertyMetadata(LicenseViewState.License, LicenseView.OnViewStatePropertyChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseView"/> class.
        /// </summary>
        /// <param name="licenseViewModel">The license view model.</param>
        [ImportingConstructor]
        public LicenseView(LicenseViewModel licenseViewModel)
        {
            // Validate the licenseViewModel argument.
            if (licenseViewModel == null)
            {
                throw new ArgumentNullException(nameof(licenseViewModel));
            }

            // Initialize the object
            this.DataContext = licenseViewModel;

            // Initialize the IDE managed resources.
            this.InitializeComponent();

            // The ViewState in the view is automatically bound to the view model since there's no easy way to do it in the XAML for a user control
            // or page.  From now on, the view model can control the visual state of the page.
            BindingOperations.SetBinding(
                this,
                LicenseView.ViewStateProperty,
                new Binding() { Mode = BindingMode.TwoWay, Path = new PropertyPath("ViewState") });
        }

        /// <summary>
        /// Gets or sets the LicenseViewState which controls the configuration of controls in the form.
        /// </summary>
        public LicenseViewState ViewState
        {
            get
            {
                return (LicenseViewState)this.GetValue(LicenseView.ViewStateProperty);
            }

            set
            {
                this.SetValue(LicenseView.ViewStateProperty, value);
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // This will initialize the visual state.
            VisualStateManager.GoToState(this, this.ViewState.ToString(), false);
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Handles a change to the state of the IsProductView property.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">Event data that that tracks changes to the effective value of this property.</param>
        private static void OnViewStatePropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            LicenseView licenseView = dependencyObject as LicenseView;
            VisualStateManager.GoToState(licenseView, licenseView.ViewState.ToString(), true);
        }
    }
}