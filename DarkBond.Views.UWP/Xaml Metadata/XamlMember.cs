// <copyright file="XamlMember.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views
{
    using System;
    using Windows.UI.Xaml.Markup;

    /// <summary>
    /// Concrete implementation of the <see cref="IXamlMember"/> class.
    /// </summary>
    public class XamlMember : IXamlMember
    {
        /// <summary>
        /// The name of the member.
        /// </summary>
        private string nameField;

        /// <summary>
        /// The metadata for the member's type.
        /// </summary>
        private IXamlType xamlType;

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlMember"/> class.
        /// </summary>
        /// <param name="name">The name of the member.</param>
        /// <param name="xamlType">The member type.</param>
        public XamlMember(string name, IXamlType xamlType)
        {
            // Initialize the object.
            this.nameField = name;
            this.xamlType = xamlType;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the XAML member is an attachable member.
        /// </summary>
        public bool IsAttachable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the XAML member is implemented as a DependencyProperty.
        /// </summary>
        public bool IsDependencyProperty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the XAML member is read-only in its backing implementation.
        /// </summary>
        public bool IsReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the XamlName name string that declares the XAML member.
        /// </summary>
        public string Name
        {
            get
            {
                return this.nameField;
            }
        }

        /// <summary>
        /// Gets the IXamlType of the type where the member can exist.
        /// </summary>
        public IXamlType TargetType
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the IXamlType of the type that is used by the member.
        /// </summary>
        public virtual IXamlType Type
        {
            get
            {
                return this.xamlType;
            }
        }

        /// <summary>
        /// Provides a get-value utility for this IXamlMember.
        /// </summary>
        /// <param name="instance">The object instance to get the member value from.</param>
        /// <returns>The member value.</returns>
        public object GetValue(object instance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Provides a set-value utility for this IXamlMember.
        /// </summary>
        /// <param name="instance">The object instance to set the member value on.</param>
        /// <param name="value">The member value to set.</param>
        public virtual void SetValue(object instance, object value)
        {
            throw new NotImplementedException();
        }
    }
}