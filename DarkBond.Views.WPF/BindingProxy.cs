// <copyright file="BindingProxy.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views
{
    using System.Windows;

    /// <summary>
    /// Used to manually provide a data context for items not part of the visual tree.
    /// </summary>
    public class BindingProxy : Freezable
    {
        /// <summary>
        /// The DataContext DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty DataContextProperty = DependencyProperty.Register(
            "DataContext",
            typeof(object),
            typeof(BindingProxy),
            null);

        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>
        /// The data context.
        /// </value>
        public object DataContext
        {
            get
            {
                return (object)this.GetValue(BindingProxy.DataContextProperty);
            }

            set
            {
                this.SetValue(BindingProxy.DataContextProperty, value);
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BindingProxy"/> class.
        /// </summary>
        /// <returns>A freezable instance of this class.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
    }
}