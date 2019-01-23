// <copyright file="TemplateSelector.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Used to select a <see cref="DataTemplate"/> from a tag in the view model.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1035:ICollectionImplementationsHaveStronglyTypedMembers", Justification = "The compiler doesn't like the Object array version.  Still thinks it's generic.")]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", Target = "DarkBond.Views.Controls.TemplateSelector", Justification = "Name chosen to reflect UI function.")]
    public class TemplateSelector : DataTemplateSelector, IDictionary
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
        /// Gets the number of elements contained in the ICollection.
        /// </summary>
        public int Count
        {
            get
            {
                return this.dictionary.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IDictionary"/> object has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return ((IDictionary)this.dictionary).IsFixedSize;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IDictionary"/> object is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return ((IDictionary)this.dictionary).IsReadOnly;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the ICollection is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return ((IDictionary)this.dictionary).IsSynchronized;
            }
        }

        /// <summary>
        /// Gets an ICollection object containing the keys of the <see cref="IDictionary"/> object.
        /// </summary>
        public ICollection Keys
        {
            get
            {
                return this.dictionary.Keys;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ICollection.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return ((IDictionary)this.dictionary).SyncRoot;
            }
        }

        /// <summary>
        /// Gets an ICollection object containing the values in the <see cref="IDictionary"/> object.
        /// </summary>
        public ICollection Values
        {
            get
            {
                return this.dictionary.Values;
            }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The element with the specified key, or null if the key does not exist.</returns>
        public object this[object key]
        {
            get
            {
                return this.dictionary[key];
            }

            set
            {
                this.dictionary[key] = value;
            }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="IDictionary"/> object.
        /// </summary>
        /// <param name="key">The Object to use as the key of the element to add. </param>
        /// <param name="value">The Object to use as the value of the element to add. </param>
        public void Add(object key, object value)
        {
            this.dictionary.Add(key, value);
        }

        /// <summary>
        /// Removes all elements from the <see cref="IDictionary"/> object.
        /// </summary>
        public void Clear()
        {
            this.dictionary.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="IDictionary"/> object contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="IDictionary"/> object.</param>
        /// <returns>true if the <see cref="IDictionary"/> contains an element with the key; otherwise, false.</returns>
        public bool Contains(object key)
        {
            return ((IDictionary)this.dictionary).Contains(key);
        }

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional Array that is the destination of the elements copied from ICollection.  The Array must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            ((IDictionary)this.dictionary).CopyTo(array, index);
        }

        /// <summary>
        /// Returns an IDictionaryEnumerator object for the <see cref="IDictionary"/> object.
        /// </summary>
        /// <returns>An IDictionaryEnumerator object for the <see cref="IDictionary"/> object.</returns>
        public IDictionaryEnumerator GetEnumerator()
        {
            return this.dictionary.GetEnumerator();
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="IDictionary"/> object.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        public void Remove(object key)
        {
            this.dictionary.Remove(key);
        }

        /// <summary>
        /// Returns a specific DataTemplate for a given item or container.
        /// </summary>
        /// <param name="item">The item to return a template for.</param>
        /// <param name="container">The parent container for the item.</param>
        /// <returns>The template to use for the given item and/or container.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
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
                    type = type.BaseType;
                    if (type == null)
                    {
                        this.ignoreTypes.Add(item.GetType());
                    }
                }
            }

            // If we reached here, we didn't find a matching DataTemplate.
            return null;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.dictionary).GetEnumerator();
        }
    }
}