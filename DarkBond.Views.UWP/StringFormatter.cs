// <copyright file="StringFormatter.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views
{
    using System;
    using System.Globalization;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Formats a value into a string.
    /// </summary>
    public class StringFormatter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringFormatter"/> class.
        /// </summary>
        public StringFormatter()
        {
            // By default, there are no limits.
            this.MaxLength = int.MaxValue;
            this.UseEllipse = true;
        }

        /// <summary>
        /// Gets or sets the maximum length of a generated string.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the formatting should issue an ellipse when the maximum length is exceeded.
        /// </summary>
        public bool UseEllipse { get; set; }

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

            // If the format string is null or empty, simply call ToString() on the value.  Otherwise this conversion will attempt to convert the
            // value to a string using the format parameter.  If the string is too long, then it will be truncated and, optionally, have ellipses
            // added to indicate a truncated value.
            string convertedValue = value.ToString();
            string formatString = parameter as string;
            if (!string.IsNullOrEmpty(formatString))
            {
                convertedValue = string.Format(new CultureInfo(language), formatString, value);
                if (this.MaxLength != int.MaxValue)
                {
                    convertedValue = convertedValue.Substring(0, Math.Min(convertedValue.Length, this.MaxLength));
                    if (convertedValue.Length == this.MaxLength && this.UseEllipse)
                    {
                        convertedValue += "...";
                    }
                }
            }

            // This is the converted value.
            return convertedValue;
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