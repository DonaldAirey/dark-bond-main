// <copyright file="XamlType.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views
{
    using System;
    using System.Collections.Generic;
    using Windows.UI.Xaml.Markup;

    /// <summary>
    /// Concrete implementation of the <see cref="IXamlType"/> interface.
    /// </summary>
    public class XamlType : IXamlType
    {
        /// <summary>
        /// The base type described by this metadata.
        /// </summary>
        private XamlType baseTypeField;

        /// <summary>
        /// The full name of the type.
        /// </summary>
        private string fullNameField;

        /// <summary>
        /// A dictionary of metadata for them members of this type.
        /// </summary>
        private Dictionary<string, IXamlMember> membersField = new Dictionary<string, IXamlMember>();

        /// <summary>
        /// The type described by this metadata.
        /// </summary>
        private Type typeField;

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlType"/> class.
        /// </summary>
        /// <param name="type">The type of object described by the metadata.</param>
        public XamlType(Type type)
        {
            // Validate the 'type' parameter
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // Initialize the object.
            this.typeField = type;
            this.fullNameField = type.FullName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlType"/> class.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        /// <param name="type">The type of object described by the metadata.</param>
        public XamlType(Type baseType, Type type)
        {
            // Validate the 'baseType' parameter
            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }

            // Validate the 'type' parameter
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // Initialize the object.
            this.baseTypeField = new XamlType(baseType);
            this.typeField = type;
            this.fullNameField = type.FullName;
        }

        /// <summary>
        /// Gets the IXamlType for the immediate base type of the XAML type.  Determination of this value is based on the underlying type for core
        /// types.
        /// </summary>
        /// <value>
        /// The IXamlType for the immediate base type of the XAML type.  Determination of this value is based on the underlying type for core
        /// types.
        /// </value>
        public virtual IXamlType BaseType
        {
            get
            {
                return this.baseTypeField;
            }
        }

        /// <summary>
        /// Gets the IXamlMember information for the XAML content property of this IXamlType.
        /// </summary>
        public virtual IXamlMember ContentProperty
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the full class name of the underlying type.
        /// </summary>
        public virtual string FullName
        {
            get
            {
                return this.fullNameField;
            }
        }

        /// <summary>
        /// Gets the dictionary of member metadata.
        /// </summary>
        public Dictionary<string, IXamlMember> Members
        {
            get
            {
                return this.membersField;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IXamlType represents an array.
        /// </summary>
        public virtual bool IsArray
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the type is bind-able.
        /// </summary>
        public virtual bool IsBindable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this IXamlType represents a collection.
        /// </summary>
        public virtual bool IsCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this IXamlType represents a constructible type, as per the XAML definition.
        /// </summary>
        public bool IsConstructible
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this IXamlType represents a dictionary/map.
        /// </summary>
        public virtual bool IsDictionary
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IXamlType represents a markup extension.
        /// </summary>
        public virtual bool IsMarkupExtension
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value that provides the type information for the Items property of this IXamlType.
        /// </summary>
        public virtual IXamlType ItemType
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a value that provides the type information for the Key property of this IXamlType, if this IXamlType represents a dictionary/map.
        /// </summary>
        public virtual IXamlType KeyType
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets information for the backing type.
        /// </summary>
        public Type UnderlyingType
        {
            get
            {
                return this.typeField;
            }
        }

        /// <summary>
        /// Sets its values for initialization and returns a usable instance.
        /// </summary>
        /// <returns>The usable instance.</returns>
        public virtual object ActivateInstance()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds an item to a custom map type.
        /// </summary>
        /// <param name="instance">The type instance to set the map item to.</param>
        /// <param name="key">The key of the map item to add.</param>
        /// <param name="value">The value of the map item to add.</param>
        public virtual void AddToMap(object instance, object key, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds an item to a custom vector type.
        /// </summary>
        /// <param name="instance">The type instance to set the item to.</param>
        /// <param name="value">The value of the item to add.</param>
        public virtual void AddToVector(object instance, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a type system representation based on a string.  The main scenario for this usage is creating an enumeration value and mapping
        /// the appropriate enumeration.
        /// </summary>
        /// <param name="value">The string to create from.</param>
        /// <returns>The resulting type system representation.</returns>
        public virtual object CreateFromString(string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the IXamlMember information for a specific named member from this IXamlType.
        /// </summary>
        /// <param name="name">The name of the member to get (as a string).</param>
        /// <returns>The <see cref="IXamlMember"/> information for the member, if a member as specified by name was found; otherwise, null.</returns>
        public IXamlMember GetMember(string name)
        {
            // If a member exists
            IXamlMember xamlMember = null;
            this.membersField.TryGetValue(name, out xamlMember);
            return xamlMember;
        }

        /// <summary>
        /// Invokes any necessary pre-activation logic as required by the XAML schema context and its platform dependencies.
        /// </summary>
        public virtual void RunInitializer()
        {
        }
    }
}