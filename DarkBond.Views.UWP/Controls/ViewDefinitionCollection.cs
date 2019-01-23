// <copyright file="ViewDefinitionCollection.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// A collection of <see cref="ViewDefinition"/> items.
    /// </summary>
    /// <remarks>This class exists primarily for the design surface which doesn't seem to handle generic types properly.</remarks>
    public class ViewDefinitionCollection : ObservableCollection<ViewDefinition>
    {
    }
}
