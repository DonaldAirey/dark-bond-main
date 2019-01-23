// <copyright file="TemplateSelector.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Markup;

    /// <summary>
    /// Used to select a <see cref="DataTemplate"/> from a tag in the view model.
    /// </summary>
    [ContentProperty(Name = "Children")]
    public class TemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// The dictionary backing the <see cref="IDictionary"/> interface.
        /// </summary>
        private Dictionary<object, object> dictionary = new Dictionary<object, object>();

        /// <summary>
        /// A cache of data templates already recognized.
        /// </summary>
        private Dictionary<Type, DataTemplate> cache = new Dictionary<Type, DataTemplate>();

        /// <summary>
        /// This is used to ignore types that have already been evaluated and aren't part of the mapping.
        /// </summary>
        private HashSet<Type> ignoreTypes = new HashSet<Type>();

        /// <summary>
        /// Gets the children.
        /// </summary>
        public IDictionary<object, object> Children
        {
            get
            {
                return this.dictionary;
            }
        }

        /// <summary>
        /// Returns a specific DataTemplate for a given item or container.
        /// </summary>
        /// <param name="item">The item to return a template for.</param>
        /// <param name="container">The parent container for the item.</param>
        /// <returns>The template to use for the given item and/or container.</returns>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            // For some reason, we're given null objects to resolve.  I still haven't figured out why.
            if (item == null)
            {
                return null;
            }

            // The general idea here is to find a template that matches the type of the given view model.  One complications is that a lot of garbage
            // is also fed into this method and when we don't have a matching data template, we don't want to constantly be doing a search.  So a
            // list of previously evaluated types that didn't have a corresponding DataTemplate is used to ignore these types.
            Type type = item.GetType();
            if (!this.ignoreTypes.Contains(type))
            {
                // A cache is used to speed up the lookup.  If we searched for a type or base type that matched the given view model and successfully
                // found one, then it's put in the cache and, for the life of this selector, that view model will be associated with the found
                // DataTemplate.
                DataTemplate dataTemplate;
                if (this.cache.TryGetValue(type, out dataTemplate))
                {
                    return dataTemplate;
                }

                // If we're not ignoring the given view model and we haven't found it already, then search the type (and all the ancestor types) to
                // see if we have an association defined between a view model and a DataTemplate.
                while (type != null)
                {
                    // If we find an association, then add it to the cache.
                    object resource = null;
                    if (this.dictionary.TryGetValue(type.FullName, out resource))
                    {
                        dataTemplate = resource as DataTemplate;
                        this.cache.Add(item.GetType(), dataTemplate);
                        return dataTemplate;
                    }

                    // If we don't find an association, then repeat the process with the base type.  If we've tried all of the ancestor types, then
                    // remember to ignore this type the next time we see it.
                    type = type.GetTypeInfo().BaseType;
                    if (type == null)
                    {
                        this.ignoreTypes.Add(item.GetType());
                    }
                }
            }

            // If we reached here, we didn't find a matching DataTemplate.
            return null;
        }
    }
}