// <copyright file="BooleanConverter.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views
{
    using System;
    using DarkBond.ViewModels.Strings;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Formats a value into a string.
    /// </summary>
    public class BooleanConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanConverter"/> class.
        /// </summary>
        public BooleanConverter()
        {
            // By default, there are no limits.
            this.UseYesNo = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use 'Yes/No' or 'True/False'.
        /// </summary>
        public bool UseYesNo { get; set; }

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Validate the value argument
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // Use the US standard for a default culture.
            if (string.IsNullOrEmpty(language))
            {
                language = "en-us";
            }

            // Convert to a 'Yes' or 'No' value.
            if (this.UseYesNo)
            {
                return (bool)value ? Resources.Yes : Resources.No;
            }

            // Convert to a 'True' or 'False' value.
            return (bool)value ? Resources.True : Resources.False;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}