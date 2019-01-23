// <copyright file="ProductFolderViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.ListViews
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Composition;
    using DarkBond.LicenseManager.Strings;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// View model for the product folder when it appears as an item in a list.
    /// </summary>
    public class ProductFolderViewModel : CommonListViewViewModel
    {
        /// <summary>
        /// The number of children.
        /// </summary>
        private int countField;

        /// <summary>
        /// The command to open a product folder.
        /// </summary>
        private DelegateCommand productFolderOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFolderViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="licenseService">The license service.</param>
        public ProductFolderViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ILicenseService licenseService)
            : base(compositionContext, dataModel, licenseService)
        {
            // Initialize the properties of this view model.  Folders are a synthetic construct, so the dates are arbitrary.
            this.Count = this.DataModel.Product.Count;
            this.ImageKey = ImageKeys.Folder;
            this.Name = Resources.Product;

            // This will tell us when the number of products has changed.
            this.DataModel.Product.CollectionChanged += this.OnProductsChanged;

            // These commands are handled by this view model.
            this.productFolderOpen = new DelegateCommand(
                () => GlobalCommands.Locate.Execute(this.Uri),
                () => GlobalCommands.Open.RegisteredCommands.Count == 1);
        }

        /// <summary>
        /// Gets or sets the number of children.
        /// </summary>
        public int Count
        {
            get
            {
                return this.countField;
            }

            set
            {
                if (this.countField != value)
                {
                    this.countField = value;
                    this.OnPropertyChanged("Count");
                }
            }
        }

        /// <summary>
        /// Gets or sets the key used to reference the image in the view.
        /// </summary>
        public string ImageKey { get; protected set; }

        /// <summary>
        /// Gets or sets the name of this item.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the URI for this object.
        /// </summary>
        public override Uri Uri
        {
            get
            {
                return new Uri(this.Parent.Uri.OriginalString + @"\" + Resources.Product);
            }
        }

        /// <inheritdoc/>
        protected override ObservableCollection<IDisposable> CreateContextButtonItems()
        {
            // Use the base class to create the common elements.
            ObservableCollection<IDisposable> contextButtonViewItems = base.CreateContextButtonItems();

            // Open Button
            ButtonViewModel openButtonItem = this.CompositionContext.GetExport<ButtonViewModel>();
            openButtonItem.Command = GlobalCommands.Open;
            openButtonItem.Header = Resources.Open;
            openButtonItem.ImageKey = ImageKeys.Open;
            contextButtonViewItems.Add(openButtonItem);

            // New Button
            ButtonViewModel newButtonItem = this.CompositionContext.GetExport<ButtonViewModel>();
            newButtonItem.Command = new DelegateCommand(() => { }, () => false);
            newButtonItem.Header = Resources.New;
            newButtonItem.ImageKey = ImageKeys.New;
            contextButtonViewItems.Add(newButtonItem);

            // Delete Menu Item
            ButtonViewModel deleteButton = this.CompositionContext.GetExport<ButtonViewModel>();
            deleteButton.Command = new DelegateCommand(() => { }, () => false);
            deleteButton.Header = Resources.Delete;
            deleteButton.ImageKey = ImageKeys.Delete;
            contextButtonViewItems.Add(deleteButton);

            // Properties Menu Item
            ButtonViewModel propertiesButtonItem = this.CompositionContext.GetExport<ButtonViewModel>();
            propertiesButtonItem.Command = new DelegateCommand(() => { }, () => false);
            propertiesButtonItem.Header = Resources.Properties;
            propertiesButtonItem.ImageKey = ImageKeys.Properties;
            contextButtonViewItems.Add(propertiesButtonItem);

            // These are the context button items.
            return contextButtonViewItems;
        }

        /// <summary>
        /// Creates a context menu.
        /// </summary>
        /// <returns>The collection of menu items for the context menu.</returns>
        protected override ObservableCollection<IDisposable> CreateContextMenuItems()
        {
            // Create the default context menu for a list view item (everything can be opened in a list view).
            ObservableCollection<IDisposable> contextMenuViewItems = base.CreateContextMenuItems();

            // Open Menu Item
            OpenMenuItemViewModel openMenuItem = this.CompositionContext.GetExport<OpenMenuItemViewModel>();
            openMenuItem.Command = GlobalCommands.Open;
            openMenuItem.Header = Resources.Open;
            openMenuItem.ImageKey = ImageKeys.Open;
            contextMenuViewItems.Add(openMenuItem);

            // New Product Menu Item
            MenuItemViewModel newProductMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            newProductMenuItem.Command = new DelegateCommand(() => this.LicenseService.NavigateToProduct());
            newProductMenuItem.Header = Resources.NewProduct;
            newProductMenuItem.ImageKey = ImageKeys.Product;
            contextMenuViewItems.Add(newProductMenuItem);

            // These are the context menu items.
            return contextMenuViewItems;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            // This will unhook us from the data model when the item is no longer displayed.
            if (disposing)
            {
                // Disengage from notifications from the data model.
                this.DataModel.Product.CollectionChanged -= this.OnProductsChanged;

                // Make sure we unregister any composite commands.
                if (this.IsSelected)
                {
                    GlobalCommands.Open.UnregisterCommand(this.productFolderOpen);
                }
            }

            // Allow the base class to complete the method.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles a change to the IsSelected property.
        /// </summary>
        protected override void OnIsSelectedChanged()
        {
            // In order for the 'gang' operations to work, the individual commands must be added to the composite command (and removed when no longer
            // needed).
            if (this.IsSelected)
            {
                GlobalCommands.Open.RegisterCommand(this.productFolderOpen);
            }
            else
            {
                GlobalCommands.Open.UnregisterCommand(this.productFolderOpen);
            }

            // Allow the base class to handle the reset of the selection change.
            base.OnIsSelectedChanged();
        }

        /// <summary>
        /// Handles the CollectionChanged event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="notifyCollectionChangedEventArgs">Information about the event.</param>
        private void OnProductsChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            // Keep the count synchronized with the data model.
            this.Count = this.DataModel.Product.Count;
        }
    }
}