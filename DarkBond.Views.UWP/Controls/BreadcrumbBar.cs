﻿// <copyright file="BreadcrumbBar.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Diagnostics.CodeAnalysis;
    using DarkBond.ViewModels;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// A button that contains a single image surrounded by a circle.
    /// </summary>
    /// <remarks>This control exists primarily to fix a but with the AppBarButton that prevents the width from being changed.</remarks>
    public class BreadcrumbBar : ItemsControl
    {
        /// <summary>
        /// The Source DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty DictionaryProperty = DependencyProperty.Register(
            "Dictionary",
            typeof(UriDictionary),
            typeof(BreadcrumbBar),
            new PropertyMetadata(null));

        /// <summary>
        /// The Key DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ImageKeyProperty = DependencyProperty.Register(
            "Key",
            typeof(string),
            typeof(BreadcrumbBar),
            new PropertyMetadata(null));

        /// <summary>
        /// The Name DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty LeafHeaderProperty = DependencyProperty.Register(
            "LeafHeader",
            typeof(string),
            typeof(BreadcrumbBar),
            new PropertyMetadata(string.Empty));

        /// <summary>
        /// The ParentHeader DependencyProperty.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The field is immutable")]
        public static readonly DependencyProperty ParentHeaderProperty = DependencyProperty.Register(
            "ParentHeader",
            typeof(string),
            typeof(BreadcrumbBar),
            new PropertyMetadata(string.Empty));

        /// <summary>
        /// Initializes a new instance of the <see cref="BreadcrumbBar"/> class.
        /// </summary>
        public BreadcrumbBar()
        {
            // This allows the view to be styled.
            this.DefaultStyleKey = typeof(BreadcrumbBar);

            // This is the initial state of the breadcrumb control.
            VisualStateManager.GoToState(this, "Root", true);
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key
        {
            get
            {
                return this.GetValue(BreadcrumbBar.ImageKeyProperty) as string;
            }

            set
            {
                this.SetValue(BreadcrumbBar.ImageKeyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the dictionary of images.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "There is no other way to assign Dictionaries in XAML")]
        public UriDictionary Dictionary
        {
            get
            {
                return this.GetValue(BreadcrumbBar.DictionaryProperty) as UriDictionary;
            }

            set
            {
                this.SetValue(BreadcrumbBar.DictionaryProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the leaf header.
        /// </summary>
        public object LeafHeader
        {
            get
            {
                return this.GetValue(BreadcrumbBar.LeafHeaderProperty);
            }

            set
            {
                this.SetValue(BreadcrumbBar.LeafHeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parent header.
        /// </summary>
        public object ParentHeader
        {
            get
            {
                return this.GetValue(BreadcrumbBar.ParentHeaderProperty);
            }

            set
            {
                this.SetValue(BreadcrumbBar.ParentHeaderProperty, value);
            }
        }

        /// <summary>
        /// Invoked when the value of the Items property changes.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected override void OnItemsChanged(object e)
        {
            // Reconcile the breadcrumbs in the view with the view model.
            if (this.Items.Count == 0)
            {
                // When the list is empty, clear the binding to the breadcrumbs in the view.
                this.ClearValue(BreadcrumbBar.LeafHeaderProperty);
                this.ClearValue(BreadcrumbBar.ParentHeaderProperty);
            }
            else
            {
                // Populate the leaf.  Binding to the view model is important for live updates.
                BreadcrumbItemViewModel leafBreadcrumb = this.Items[this.Items.Count - 1] as BreadcrumbItemViewModel;
                Binding leafHeaderBinding = new Binding();
                leafHeaderBinding.Path = new PropertyPath("Header");
                leafHeaderBinding.Source = leafBreadcrumb;
                this.SetBinding(BreadcrumbBar.LeafHeaderProperty, leafHeaderBinding);

                // Populate the parent node.
                if (this.Items.Count == 1)
                {
                    // A single breadcrumb means we're at the root level of the hierarchy.
                    VisualStateManager.GoToState(this, "Root", true);
                    this.ClearValue(BreadcrumbBar.ParentHeaderProperty);

                    // The icon gives visual feedback that we're at the application root.
                    this.Key = leafBreadcrumb.ImageKey;
                }
                else
                {
                    // Any more than one breadcrumb means that we're presenting a parent/child relationship.
                    VisualStateManager.GoToState(this, "Tree", true);
                    BreadcrumbItemViewModel parentBreadcrumb = this.Items[this.Items.Count - 2] as BreadcrumbItemViewModel;
                    Binding parentHeaderBinding = new Binding();
                    parentHeaderBinding.Path = new PropertyPath("Header");
                    parentHeaderBinding.Source = parentBreadcrumb;
                    this.SetBinding(BreadcrumbBar.ParentHeaderProperty, parentHeaderBinding);

                    // The leaf's icon in the Breadcrumb bar gives a visual indication of the current level in the hierarchy.
                    this.Key = leafBreadcrumb.ImageKey;
                }
            }

            // Allow the base class to handle the rest of the call.
            base.OnItemsChanged(e);
        }
    }
}