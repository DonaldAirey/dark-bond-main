// <copyright file="ProductViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager.ViewModels.ListViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Composition;
    using DarkBond.ClientModel;
    using DarkBond.LicenseManager.Strings;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// View model for a product for which license can be issued.
    /// </summary>
    public class ProductViewModel : CommonListViewViewModel
    {
        /// <summary>
        /// The description of the product.
        /// </summary>
        private string descriptionField;

        /// <summary>
        /// The number of licenses owned by this product.
        /// </summary>
        private int licensesOwned;

        /// <summary>
        /// The command to create a license.
        /// </summary>
        private DelegateCommand licenseNew;

        /// <summary>
        /// The display name of the item.
        /// </summary>
        private string nameField;

        /// <summary>
        /// A table that drives the notifications when data model changes.
        /// </summary>
        private Dictionary<string, Action<ProductRow>> notifyActions = new Dictionary<string, Action<ProductRow>>();

        /// <summary>
        /// The command to delete a product.
        /// </summary>
        private DelegateCommand productDelete;

        /// <summary>
        /// The unique identifier of this product.
        /// </summary>
        private Guid productIdField;

        /// <summary>
        /// The command to open a product.
        /// </summary>
        private DelegateCommand productOpen;

        /// <summary>
        /// The command to update the properties.
        /// </summary>
        private DelegateCommand productProperties;

        /// <summary>
        /// The product row.
        /// </summary>
        private ProductRow productRowField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductViewModel"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition container.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="licenseService">The license services.</param>
        public ProductViewModel(
            CompositionContext compositionContext,
            DataModel dataModel,
            ILicenseService licenseService)
            : base(compositionContext, dataModel, licenseService)
        {
            // Initialize the object
            this.ImageKey = ImageKeys.Product;

            // These commands are handled by this view model.
            this.licenseNew = new DelegateCommand(
                () => this.LicenseService.NavigateToProductLicense(this.ProductId),
                () => GlobalCommands.New.RegisteredCommands.Count == 1);
            this.productDelete = new DelegateCommand(
                () => this.LicenseService.DeleteProduct(this.ProductId),
                () => this.LicenseService.CanDeleteProduct(this.ProductId));
            this.productOpen = new DelegateCommand(
                () => GlobalCommands.Locate.Execute(this.Uri),
                () => GlobalCommands.Open.RegisteredCommands.Count == 1);
            this.productProperties = new DelegateCommand(
                () => this.LicenseService.NavigateToProduct(this.ProductId),
                () => this.LicenseService.CanNavigateToProduct(this.ProductId));
        }

        /// <summary>
        /// Gets or sets the date the item was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date the item was last modified.
        /// </summary>
        public DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of this product.
        /// </summary>
        public string Description
        {
            get
            {
                return this.descriptionField;
            }

            set
            {
                if (this.descriptionField != value)
                {
                    this.descriptionField = value;
                    this.OnPropertyChanged("Description");
                }
            }
        }

        /// <summary>
        /// Gets or sets the key used to reference the image in the view.
        /// </summary>
        public string ImageKey { get; protected set; }

        /// <summary>
        /// Gets or sets the number of licenses owned by this product.
        /// </summary>
        public int LicensesOwned
        {
            get
            {
                return this.licensesOwned;
            }

            set
            {
                if (this.licensesOwned != value)
                {
                    this.licensesOwned = value;
                    this.OnPropertyChanged("LicensesOwned");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of this object.
        /// </summary>
        public string Name
        {
            get
            {
                return this.nameField;
            }

            set
            {
                if (this.nameField != value)
                {
                    this.nameField = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of this product.
        /// </summary>
        public Guid ProductId
        {
            get
            {
                return this.productIdField;
            }

            set
            {
                if (this.productIdField != value)
                {
                    this.productIdField = value;
                    this.OnPropertyChanged("ProductId");
                }
            }
        }

        /// <summary>
        /// Gets the URI for this object.
        /// </summary>
        public override Uri Uri
        {
            get
            {
                return new Uri(this.Parent.Uri.OriginalString + "\\" + this.ProductId.ToString("N"));
            }
        }

        /// <summary>
        /// Maps the properties of a ProductRow in the data model to the properties of this view model.
        /// </summary>
        /// <param name="productRow">The product row.</param>
        public void Map(ProductRow productRow)
        {
            // Validate the parameter.
            if (productRow == null)
            {
                throw new ArgumentNullException(nameof(productRow));
            }

            // Initialize the object.
            this.productRowField = productRow;
            this.productRowField.PropertyChanged += this.OnProductRowChanged;
            this.DataModel.ProductLicenseProductIdKey.RelationChanged += this.OnLicenseRelationChanged;

            // This table drives the updating of the view model when the data model changes.
            this.notifyActions.Add("ProductId", this.UpdateProductId);
            this.notifyActions.Add("Description", (c) => this.Description = c.Description);
            this.notifyActions.Add("Name", (c) => this.Name = c.Name);

            // Initialize the view model with the data model.
            foreach (string property in this.notifyActions.Keys)
            {
                this.notifyActions[property](this.productRowField);
            }
        }

        /// <summary>
        /// Creates the appBar items.
        /// </summary>
        /// <returns>The appBar items.</returns>
        protected override ObservableCollection<IDisposable> CreateAppBarItems()
        {
            // This list view item uses a common set of appBar items.
            ObservableCollection<IDisposable> appBarItems = base.CreateAppBarItems();

            // The Delete command.
            ButtonViewModel deleteButton = this.CompositionContext.GetExport<ButtonViewModel>();
            deleteButton.Command = GlobalCommands.Delete;
            deleteButton.Label = Resources.Delete;
            deleteButton.ImageKey = ImageKeys.Delete;
            appBarItems.Add(deleteButton);

            // The Edit command.
            ButtonViewModel editButton = this.CompositionContext.GetExport<ButtonViewModel>();
            editButton.Command = new DelegateCommand(() => this.LicenseService.NavigateToProduct(this.ProductId));
            editButton.Label = Resources.Edit;
            editButton.ImageKey = ImageKeys.Edit;
            appBarItems.Add(editButton);

            // This is the set of appBar items for this ListView item.
            return appBarItems;
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
            DropDownButtonViewModel newButtonItem = this.CompositionContext.GetExport<DropDownButtonViewModel>();
            newButtonItem.Header = Resources.New;
            newButtonItem.ImageKey = ImageKeys.New;
            contextButtonViewItems.Add(newButtonItem);

            // New License Menu Item
            DropDownButtonViewModel newProductButton = this.CompositionContext.GetExport<DropDownButtonViewModel>();
            newProductButton.Command = GlobalCommands.New;
            newProductButton.Header = Resources.NewLicense;
            newProductButton.ImageKey = ImageKeys.License;
            newButtonItem.Items.Add(newProductButton);

            // Delete Menu Item
            ButtonViewModel deleteButton = this.CompositionContext.GetExport<ButtonViewModel>();
            deleteButton.Command = GlobalCommands.Delete;
            deleteButton.Header = Resources.Delete;
            deleteButton.ImageKey = ImageKeys.Delete;
            contextButtonViewItems.Add(deleteButton);

            // Properties Menu Item
            ButtonViewModel propertiesButtonItem = this.CompositionContext.GetExport<ButtonViewModel>();
            propertiesButtonItem.Command = GlobalCommands.Properties;
            propertiesButtonItem.Header = Resources.Properties;
            propertiesButtonItem.ImageKey = ImageKeys.Properties;
            contextButtonViewItems.Add(propertiesButtonItem);

            // This is the context menu.
            return contextButtonViewItems;
        }

        /// <inheritdoc/>
        protected override ObservableCollection<IDisposable> CreateContextMenuItems()
        {
            // Use the base class to create the common elements.
            ObservableCollection<IDisposable> contextMenuViewItems = base.CreateContextMenuItems();

            // Open Menu Item
            OpenMenuItemViewModel openMenuItem = this.CompositionContext.GetExport<OpenMenuItemViewModel>();
            openMenuItem.Command = GlobalCommands.Locate;
            openMenuItem.CommandParameter = this.Uri;
            openMenuItem.Header = Resources.Open;
            openMenuItem.ImageKey = ImageKeys.Open;
            contextMenuViewItems.Add(openMenuItem);

            // New License Menu Item
            MenuItemViewModel newProductMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            newProductMenuItem.Command = new DelegateCommand(() => this.LicenseService.NavigateToProductLicense(this.ProductId));
            newProductMenuItem.Header = Resources.NewLicense;
            newProductMenuItem.ImageKey = ImageKeys.License;
            contextMenuViewItems.Add(newProductMenuItem);

            // Delete Menu Item
            MenuItemViewModel deleteProductMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            deleteProductMenuItem.Command = new DelegateCommand(
                () => this.LicenseService.DeleteProduct(this.ProductId),
                () => this.LicenseService.CanDeleteProduct(this.ProductId));
            deleteProductMenuItem.Header = Resources.Delete;
            deleteProductMenuItem.ImageKey = ImageKeys.Delete;
            contextMenuViewItems.Add(deleteProductMenuItem);

            // Properties Menu Item
            MenuItemViewModel propertiesMenuItem = this.CompositionContext.GetExport<MenuItemViewModel>();
            propertiesMenuItem.Command = new DelegateCommand(() => this.LicenseService.NavigateToProduct(this.ProductId));
            propertiesMenuItem.Header = Resources.Properties;
            propertiesMenuItem.ImageKey = ImageKeys.Properties;
            contextMenuViewItems.Add(propertiesMenuItem);

            // This is the context menu.
            return contextMenuViewItems;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to indicate that the object is being disposed, false to indicate that the object is being finalized.</param>
        protected override void Dispose(bool disposing)
        {
            // This will unhook us from the data model when the item is no longer displayed.
            if (disposing)
            {
                // Disengage from notifications from the data model.
                this.productRowField.PropertyChanged -= this.OnProductRowChanged;
                this.DataModel.ProductLicenseProductIdKey.RelationChanged -= this.OnLicenseRelationChanged;

                // Remove all the actions that update this view model from the data model.
                this.notifyActions.Clear();

                // Make sure we unregister any composite commands.
                if (this.IsSelected)
                {
                    GlobalCommands.Delete.UnregisterCommand(this.productDelete);
                    GlobalCommands.New.UnregisterCommand(this.licenseNew);
                    GlobalCommands.Open.UnregisterCommand(this.productOpen);
                    GlobalCommands.Properties.UnregisterCommand(this.productProperties);
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
                GlobalCommands.Delete.RegisterCommand(this.productDelete);
                GlobalCommands.New.RegisterCommand(this.licenseNew);
                GlobalCommands.Open.RegisterCommand(this.productOpen);
                GlobalCommands.Properties.RegisterCommand(this.productProperties);
            }
            else
            {
                GlobalCommands.Delete.UnregisterCommand(this.productDelete);
                GlobalCommands.New.UnregisterCommand(this.licenseNew);
                GlobalCommands.Open.UnregisterCommand(this.productOpen);
                GlobalCommands.Properties.UnregisterCommand(this.productProperties);
            }

            // Allow the base class to handle the reset of the selection change.
            base.OnIsSelectedChanged();
        }

        /// <summary>
        /// Handles a change to the License child relations.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="notifyRelationChangedEventArgs">The event data.</param>
        private void OnLicenseRelationChanged(object sender, NotifyRelationChangedEventArgs<Guid> notifyRelationChangedEventArgs)
        {
            // Handle the different update actions.
            switch (notifyRelationChangedEventArgs.Action)
            {
                case NotifyRelationChangedAction.Add:

                    // Adding a child increments the licenses owned count.
                    if (this.ProductId == notifyRelationChangedEventArgs.NewKey)
                    {
                        this.LicensesOwned++;
                    }

                    break;

                case NotifyRelationChangedAction.Remove:

                    // Removing a child decrements the licenses owned count.
                    if (this.ProductId == notifyRelationChangedEventArgs.OldKey)
                    {
                        this.LicensesOwned--;
                    }

                    break;

                case NotifyRelationChangedAction.Change:

                    // Adding a child increments the licenses owned count.
                    if (this.ProductId == notifyRelationChangedEventArgs.NewKey)
                    {
                        this.LicensesOwned++;
                    }

                    // Removing a child decrements the licenses owned count.
                    if (this.ProductId == notifyRelationChangedEventArgs.OldKey)
                    {
                        this.LicensesOwned--;
                    }

                    break;
            }
        }

        /// <summary>
        /// Handles a change to the data model product row.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="e">The event data.</param>
        private void OnProductRowChanged(object sender, PropertyChangedEventArgs e)
        {
            Action<ProductRow> notifyAction;
            if (this.notifyActions.TryGetValue(e.PropertyName, out notifyAction))
            {
                notifyAction(this.productRowField);
            }
        }

        /// <summary>
        /// Handles a change to the ProductId property.
        /// </summary>
        /// <param name="productRow">The data model version of the product data.</param>
        private void UpdateProductId(ProductRow productRow)
        {
            // This will update the metadata that displays the number of licenses owned.
            this.ProductId = productRow.ProductId;
            this.SortKey = productRow.ProductId;
            this.LicensesOwned = this.DataModel.ProductLicenseProductIdKey.GetLicenseRows(this.ProductId).Count;
        }
    }
}