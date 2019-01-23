// <copyright file="ButtonViewModel.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    /// <summary>
    /// A view model for menu items.
    /// </summary>
    public class ButtonViewModel : ViewModel
    {
        /// <summary>
        /// the command associated with the menu item.
        /// </summary>
        private object commandField;

        /// <summary>
        /// The parameter to pass to the <see cref="Command"/> property of a MenuItem.
        /// </summary>
        private object commandParameterField;

        /// <summary>
        /// The item that labels the control.
        /// </summary>
        private object headerField;

        /// <summary>
        /// The horizontal alignment of the item.
        /// </summary>
        private HorizontalAlignment horizontalAlignmentField;

        /// <summary>
        /// The key used to find an icon/image in the URI dictionary.
        /// </summary>
        private string imageKeyField;

        /// <summary>
        /// The item that labels the control.
        /// </summary>
        private string labelField;

        /// <summary>
        /// Gets or sets the command associated with the menu item.
        /// </summary>
        public object Command
        {
            get
            {
                return this.commandField;
            }

            set
            {
                if (this.commandField != value)
                {
                    this.commandField = value;
                    this.OnPropertyChanged("Command");
                }
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the <see cref="Command"/> property of a menu item.
        /// </summary>
        public object CommandParameter
        {
            get
            {
                return this.commandParameterField;
            }

            set
            {
                if (this.commandParameterField != value)
                {
                    this.commandParameterField = value;
                    this.OnPropertyChanged("CommandParameter");
                }
            }
        }

        /// <summary>
        /// Gets or sets the item that labels the control.
        /// </summary>
        public object Header
        {
            get
            {
                return this.headerField;
            }

            set
            {
                if (this.headerField != value)
                {
                    this.headerField = value;
                    this.OnPropertyChanged("Header");
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return this.horizontalAlignmentField;
            }

            set
            {
                if (this.horizontalAlignmentField != value)
                {
                    this.horizontalAlignmentField = value;
                    this.OnPropertyChanged("HorizontalAlignment");
                }
            }
        }

        /// <summary>
        /// Gets or sets the key used to find the URI for the image.
        /// </summary>
        public string ImageKey
        {
            get
            {
                return this.imageKeyField;
            }

            set
            {
                if (this.imageKeyField != value)
                {
                    this.imageKeyField = value;
                    this.OnPropertyChanged("ImageKey");
                }
            }
        }

        /// <summary>
        /// Gets or sets the item that labels the control.
        /// </summary>
        public string Label
        {
            get
            {
                return this.labelField;
            }

            set
            {
                if (this.labelField != value)
                {
                    this.labelField = value;
                    this.OnPropertyChanged("Label");
                }
            }
        }
    }
}