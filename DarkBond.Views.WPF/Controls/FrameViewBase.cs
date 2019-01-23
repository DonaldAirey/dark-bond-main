// <copyright file="FrameViewBase.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;
    using DarkBond.Views.Properties;

    /// <summary>
    /// A window used to navigate through generic objects in a hierarchy.
    /// </summary>
    [ContentProperty("DirectoryPane")]
    public class FrameViewBase : ContentControl
    {
        /// <summary>
        /// Identifies the DetailsPaneMaxWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsPaneMaxWidthProperty;

        /// <summary>
        /// Identifies the DetailsPaneMinWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsPaneMinWidthProperty;

        /// <summary>
        /// Identifies the DetailsPane dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsPaneProperty = DependencyProperty.Register(
            "DetailsPane",
            typeof(FrameworkElement),
            typeof(FrameViewBase));

        /// <summary>
        /// Identifies the DirectoryPane dependency property.
        /// </summary>
        public static readonly DependencyProperty DirectoryPaneProperty = DependencyProperty.Register(
            "DirectoryPane",
            typeof(FrameworkElement),
            typeof(FrameViewBase));

        /// <summary>
        /// Identifies the DetailsPaneWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsPaneWidthProperty = DependencyProperty.Register(
            "DetailsPaneWidth",
            typeof(GridLength),
            typeof(FrameViewBase),
            new FrameworkPropertyMetadata(new GridLength(FrameViewBase.DefaultDetailsPaneWidth), FrameViewBase.OnFrameMetricPropertyChanged));

        /// <summary>
        /// Identifies the IsDetailsPaneVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDetailsPaneVisibleProperty = DependencyProperty.Register(
            "IsDetailsPaneVisible",
            typeof(bool),
            typeof(FrameViewBase),
            new FrameworkPropertyMetadata(true, FrameViewBase.OnFrameMetricPropertyChanged));

        /// <summary>
        /// Identifies the IsNavigationPaneVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsNavigationPaneVisibleProperty = DependencyProperty.Register(
            "IsNavigationPaneVisible",
            typeof(bool),
            typeof(FrameViewBase),
            new FrameworkPropertyMetadata(true, FrameViewBase.OnFrameMetricPropertyChanged));

        /// <summary>
        /// Identifies the MenuBar dependency property.
        /// </summary>
        public static readonly DependencyProperty MenuBarProperty = DependencyProperty.Register(
            "MenuBar",
            typeof(FrameworkElement),
            typeof(FrameViewBase));

        /// <summary>
        /// Identifies the NavigationBar dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationBarProperty = DependencyProperty.Register(
            "NavigationBar",
            typeof(FrameworkElement),
            typeof(FrameViewBase));

        /// <summary>
        /// Identifies the HiearchyMaxWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationPaneMaxWidthProperty;

        /// <summary>
        /// Identifies the HiearchyMinWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationPaneMinWidthProperty;

        /// <summary>
        /// Identifies the NavigationPane dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationPaneProperty = DependencyProperty.Register(
            "NavigationPane",
            typeof(FrameworkElement),
            typeof(FrameViewBase));

        /// <summary>
        /// Identifies the NavigationPaneWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationPaneWidthProperty = DependencyProperty.Register(
            "NavigationPaneWidth",
            typeof(GridLength),
            typeof(FrameViewBase),
            new FrameworkPropertyMetadata(new GridLength(FrameViewBase.DefaultNavigationPaneWidth), FrameViewBase.OnFrameMetricPropertyChanged));

        /// <summary>
        /// Identifies the RibbonBar dependency property.
        /// </summary>
        public static readonly DependencyProperty RibbonBarProperty = DependencyProperty.Register(
            "RibbonBar",
            typeof(FrameworkElement),
            typeof(FrameViewBase));

        /// <summary>
        /// The default width of the details pane.
        /// </summary>
        private const double DefaultDetailsPaneWidth = 275.0;

        /// <summary>
        /// The default width of the navigation pane.
        /// </summary>
        private const double DefaultNavigationPaneWidth = 200.0;

        /// <summary>
        /// The minimum width allowed in for the content area of the frame.
        /// </summary>
        private const double MinimumContentWidth = 133.0;

        /// <summary>
        /// The minimum width allowed for the detail pane before it disappears.
        /// </summary>
        private const double MinimumDetailsPaneWidth = 275.0;

        /// <summary>
        /// The minimum width allowed for the navigation pane.
        /// </summary>
        private const double MinimumNavigationPaneWidth = 139.0;

        /// <summary>
        /// Identifies the DetailsPaneMaxWidth dependency property.key.
        /// </summary>
        private static DependencyPropertyKey detailMaxWidthPropertyKey = DependencyProperty.RegisterReadOnly(
            "DetailsPaneMaxWidth",
            typeof(double),
            typeof(FrameViewBase),
            new FrameworkPropertyMetadata(double.MaxValue));

        /// <summary>
        /// Identifies the DetailsPaneMinWidth dependency property.key.
        /// </summary>
        private static DependencyPropertyKey detailMinWidthPropertyKey = DependencyProperty.RegisterReadOnly(
            "DetailsPaneMinWidth",
            typeof(double),
            typeof(FrameViewBase),
            new FrameworkPropertyMetadata(FrameViewBase.MinimumDetailsPaneWidth));

        /// <summary>
        /// Identifies the DetailsPaneMaxWidth dependency property key.
        /// </summary>
        private static DependencyPropertyKey navigationPaneMaxWidthPropertyKey = DependencyProperty.RegisterReadOnly(
            "NavigationPaneMaxWidth",
            typeof(double),
            typeof(FrameViewBase),
            new FrameworkPropertyMetadata(double.MaxValue));

        /// <summary>
        /// Identifies the NavigationPaneMinWidth dependency property key.
        /// </summary>
        private static DependencyPropertyKey navigationPaneMinWidthPropertyKey = DependencyProperty.RegisterReadOnly(
            "NavigationPaneMinWidth",
            typeof(double),
            typeof(FrameViewBase),
            new FrameworkPropertyMetadata(FrameViewBase.MinimumNavigationPaneWidth));

        /// <summary>
        /// Initializes static members of the <see cref="FrameViewBase"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Must be initialized here to avoid forward reference.")]
        static FrameViewBase()
        {
            // This allows the templates in resource files to be properly associated with this new class.  Without this override, the type of the
            // base class would be used as the key in any lookup involving resources dictionaries.
            FrameViewBase.DefaultStyleKeyProperty.OverrideMetadata(typeof(FrameViewBase), new FrameworkPropertyMetadata(typeof(FrameViewBase)));

            // The ContentControl is a TabStop by default.  This is a frame window and, while it is focusable, it will want to give the focus
            // immediately to its content.  There's no need for the tab to stop here.
            FrameViewBase.FocusableProperty.OverrideMetadata(typeof(FrameViewBase), new FrameworkPropertyMetadata(false));

            // The key(s) are initialized here to avoid the forward reference problems that can creep in when you move fields around.
            FrameViewBase.DetailsPaneMaxWidthProperty = FrameViewBase.detailMaxWidthPropertyKey.DependencyProperty;
            FrameViewBase.DetailsPaneMinWidthProperty = FrameViewBase.detailMinWidthPropertyKey.DependencyProperty;
            FrameViewBase.NavigationPaneMaxWidthProperty = FrameViewBase.navigationPaneMaxWidthPropertyKey.DependencyProperty;
            FrameViewBase.NavigationPaneMinWidthProperty = FrameViewBase.navigationPaneMinWidthPropertyKey.DependencyProperty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameViewBase"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed")]
        public FrameViewBase()
        {
            // When the size of the frame changes we need to adjust the settings on the grid to make sure that everything important stays visible.
            this.SizeChanged += (s, e) => this.FormatFrame(true);

            // The frame needs to be reformatted when the settings are restored to the factory values.
            Settings.Default.SettingsLoaded += (s, e) => this.FormatFrame(true);

            // Format the frame given the initial metrics.
            this.FormatFrame(true);
        }

        /// <summary>
        /// Gets or sets the menu bar content.
        /// </summary>
        public FrameworkElement DetailsPane
        {
            get
            {
                return (Control)this.GetValue(FrameViewBase.DetailsPaneProperty);
            }

            set
            {
                this.SetValue(FrameViewBase.DetailsPaneProperty, value);
            }
        }

        /// <summary>
        /// Gets the maximum width of the detail pane.
        /// </summary>
        public double DetailsPaneMaxWidth
        {
            get
            {
                return (double)this.GetValue(FrameViewBase.DetailsPaneMaxWidthProperty);
            }
        }

        /// <summary>
        /// Gets the minimum width of the detail pane.
        /// </summary>
        public double DetailsPaneMinWidth
        {
            get
            {
                return (double)this.GetValue(FrameViewBase.DetailsPaneMinWidthProperty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Detail pane is visible.
        /// </summary>
        public GridLength DetailsPaneWidth
        {
            get
            {
                return (GridLength)this.GetValue(FrameViewBase.DetailsPaneWidthProperty);
            }

            set
            {
                this.SetValue(FrameViewBase.DetailsPaneWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the menu bar content.
        /// </summary>
        public FrameworkElement DirectoryPane
        {
            get
            {
                return (Control)this.GetValue(FrameViewBase.DirectoryPaneProperty);
            }

            set
            {
                this.SetValue(FrameViewBase.DirectoryPaneProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the details pane is visible.
        /// </summary>
        public bool IsDetailsPaneVisible
        {
            get
            {
                return (bool)this.GetValue(FrameViewBase.IsDetailsPaneVisibleProperty);
            }

            set
            {
                this.SetValue(FrameViewBase.IsDetailsPaneVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the NavigationPane pane is visible.
        /// </summary>
        public bool IsNavigationPaneVisible
        {
            get
            {
                return (bool)this.GetValue(FrameViewBase.IsNavigationPaneVisibleProperty);
            }

            set
            {
                this.SetValue(FrameViewBase.IsNavigationPaneVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the menu bar content.
        /// </summary>
        public FrameworkElement MenuBar
        {
            get
            {
                return (Control)this.GetValue(FrameViewBase.MenuBarProperty);
            }

            set
            {
                this.SetValue(FrameViewBase.MenuBarProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the menu bar content.
        /// </summary>
        public FrameworkElement NavigationBar
        {
            get
            {
                return (Control)this.GetValue(FrameViewBase.NavigationBarProperty);
            }

            set
            {
                this.SetValue(FrameViewBase.NavigationBarProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the menu bar content.
        /// </summary>
        public FrameworkElement NavigationPane
        {
            get
            {
                return (Control)this.GetValue(FrameViewBase.NavigationPaneProperty);
            }

            set
            {
                this.SetValue(FrameViewBase.NavigationPaneProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the navigatin pane width.
        /// </summary>
        public GridLength NavigationPaneWidth
        {
            get
            {
                return (GridLength)this.GetValue(FrameViewBase.NavigationPaneWidthProperty);
            }

            set
            {
                this.SetValue(FrameViewBase.NavigationPaneWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets the maximum width of the navigation pane.
        /// </summary>
        public double NavigationPaneMaxWidth
        {
            get
            {
                return (double)this.GetValue(FrameViewBase.NavigationPaneMaxWidthProperty);
            }
        }

        /// <summary>
        /// Gets the minimum width of the navigation pane.
        /// </summary>
        public double NavigationPaneMinWidth
        {
            get
            {
                return (double)this.GetValue(FrameViewBase.NavigationPaneMinWidthProperty);
            }
        }

        /// <summary>
        /// Gets or sets the menu bar content.
        /// </summary>
        public FrameworkElement RibbonBar
        {
            get
            {
                return (Control)this.GetValue(FrameViewBase.RibbonBarProperty);
            }

            set
            {
                this.SetValue(FrameViewBase.RibbonBarProperty, value);
            }
        }

        /// <summary>
        /// Invoked when the frame metrics need to be recalculated.
        /// </summary>
        /// <param name="dependencyobject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">Event data that is issued by any event that tracks changes to the effective value of this
        /// property.</param>
        private static void OnFrameMetricPropertyChanged(
            DependencyObject dependencyobject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // The logic for adjusting the frame for the Details Row is the same as adjusting it for a different window size so a common routine for
            // both will adjust all the elements of the content area.
            FrameViewBase frameViewBase = dependencyobject as FrameViewBase;
            frameViewBase.FormatFrame(false);
        }

        /// <summary>
        /// Formats the frame.
        /// </summary>
        /// <param name="isResized">true if the frame has been resized, false otherwise.</param>
        private void FormatFrame(bool isResized)
        {
            // Calculate the width of the optional panes.
            double navigationPaneWidth = this.IsNavigationPaneVisible ? this.NavigationPaneWidth.Value : 0.0;
            double detailPaneWidth = this.IsDetailsPaneVisible ? this.DetailsPaneWidth.Value : 0.0;

            // Calculate the maximum size of the details pane.  If there's not enough space for the detail pane, then make it disappear.
            double maxDetailsPaneWidth = Math.Max(0.0, this.ActualWidth - navigationPaneWidth - FrameViewBase.MinimumContentWidth);
            if (!this.IsDetailsPaneVisible || (isResized && maxDetailsPaneWidth <= FrameViewBase.MinimumDetailsPaneWidth))
            {
                this.SetValue(FrameViewBase.detailMaxWidthPropertyKey, 0.0);
                this.SetValue(FrameViewBase.detailMinWidthPropertyKey, 0.0);
            }
            else
            {
                this.SetValue(FrameViewBase.detailMaxWidthPropertyKey, maxDetailsPaneWidth);
                this.SetValue(FrameViewBase.detailMinWidthPropertyKey, FrameViewBase.MinimumDetailsPaneWidth);
            }

            // This limits the width of the hierarchy pane.
            if (!this.IsNavigationPaneVisible)
            {
                this.SetValue(FrameViewBase.navigationPaneMaxWidthPropertyKey, 0.0);
                this.SetValue(FrameViewBase.navigationPaneMinWidthPropertyKey, 0.0);
            }
            else
            {
                double maxNavigationPaneWidth = Math.Max(0.0, this.ActualWidth - detailPaneWidth - FrameViewBase.MinimumContentWidth);
                this.SetValue(FrameViewBase.navigationPaneMaxWidthPropertyKey, maxNavigationPaneWidth);
                this.SetValue(FrameViewBase.navigationPaneMinWidthPropertyKey, FrameViewBase.MinimumNavigationPaneWidth);
            }
        }

        /// <summary>
        /// Hides or displays the Detail Pane.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="executedRoutedEventArgs">The event data.</param>
        private void OnViewDetailsPane(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            // Toggle the visible state of the detail pane.
            this.IsDetailsPaneVisible = !this.IsDetailsPaneVisible;
        }

        /// <summary>
        /// Hides or displays the NavigationPane Pane.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="executedRoutedEventArgs">The event data.</param>
        private void OnViewNavigationPanePane(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            // Toggle the visible state of the navigation pane.
            this.IsNavigationPaneVisible = !this.IsNavigationPaneVisible;
        }
    }
}