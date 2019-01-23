// <copyright file="FilterItemViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;

    /// <summary>
    /// Describes a filter.
    /// </summary>
    public class FilterItemViewModel : ViewModel
    {
        /// <summary>
        /// A description of the filter.
        /// </summary>
        private string descriptionField;

        /// <summary>
        /// The name of the group to which this filter belongs.
        /// </summary>
        private string groupNameField;

        /// <summary>
        /// An indication of whether this filter is enabled.
        /// </summary>
        private bool isEnabledField;

        /// <summary>
        /// The name of the filter.
        /// </summary>
        private string nameField;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterItemViewModel"/> class.
        /// </summary>
        public FilterItemViewModel()
        {
            // These property change actions are handled by this view model.
            this.PropertyChangedActions["IsEnabled"] = () => GlobalCommands.Filter.Execute(this);
        }

        /// <summary>
        /// Gets or sets a description of the filter.
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
        /// Gets or sets the name of a group which an be used to combine filters for a column (or other logical grouping).
        /// </summary>
        public string GroupName
        {
            get
            {
                return this.groupNameField;
            }

            set
            {
                if (this.groupNameField != value)
                {
                    this.groupNameField = value;
                    this.OnPropertyChanged("GroupName");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the filter is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return this.isEnabledField;
            }

            set
            {
                if (this.isEnabledField != value)
                {
                    this.isEnabledField = value;
                    this.OnPropertyChanged("IsEnabled");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the filter.
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
    }
}