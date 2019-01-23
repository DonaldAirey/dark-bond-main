// <copyright file="XamlMetadataProvider.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views
{
    using System;
    using System.Collections.Generic;
    using Windows.UI.Xaml.Markup;

    /// <summary>
    /// Provides the metadata for the classes instantiated in the XML of this project.
    /// </summary>
    public sealed class XamlMetadataProvider : IXamlMetadataProvider
    {
        /// <summary>
        /// This dictionary is used to convert the type names into XamlTypes for the parser.
        /// </summary>
        private Dictionary<string, IXamlType> xamlTypeDictionary = new Dictionary<string, IXamlType>
        {
            { "DarkBond.Views.Controls.ColumnViewRowPanel", new ColumnViewRowPanelXamlType() },
        };

        /// <summary>
        /// Implements XAML schema context access to underlying type mapping, based on specifying a full type name.
        /// </summary>
        /// <param name="fullName">The name of the class for which to return a XAML type mapping.</param>
        /// <returns>The schema context's implementation of the IXamlType concept.</returns>
        public IXamlType GetXamlType(string fullName)
        {
            // This method is part of a chain constructed by the code generators in the visual studio.  If a type for which we can provide metadata
            // is passed in, then provide a structure that contains the metadata that XAML will need to access the structure dynamically.
            IXamlType xamlType = null;
            this.xamlTypeDictionary.TryGetValue(fullName, out xamlType);
            return xamlType;
        }

        /// <summary>
        /// Implements XAML schema context access to underlying type mapping, based on providing a helper value that describes a type.
        /// </summary>
        /// <param name="type">The type as represented by the relevant type system or inter-operation support type.</param>
        /// <returns>The schema context's implementation of the IXamlType concept.</returns>
        public IXamlType GetXamlType(Type type)
        {
            return null;
        }

        /// <summary>
        /// Gets the set of XMLNS (XAML namespace) definitions that apply to the context.
        /// </summary>
        /// <returns>The set of XMLNS (XAML namespace) definitions.</returns>
        public XmlnsDefinition[] GetXmlnsDefinitions()
        {
            throw new NotImplementedException();
        }
    }
}