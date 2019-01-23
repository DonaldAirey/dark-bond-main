// <copyright file="FilterCollection.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// A collection of filters.
    /// </summary>
    /// <typeparam name="T">The type of object on which the filters will act.</typeparam>
    public class FilterCollection<T> : ObservableCollection<ObservableCollection<Predicate<T>>>
    {
    }
}
