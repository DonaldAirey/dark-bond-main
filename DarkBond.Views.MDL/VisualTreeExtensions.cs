// <copyright file="VisualTreeExtensions.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Utility functions for the visual tree.
    /// </summary>
    public static class VisualTreeExtensions
    {
        /// <summary>
        /// Finds the first ancestor of the given object having a type of T.
        /// </summary>
        /// <typeparam name="T">The type of the ancestor to be found.</typeparam>
        /// <param name="child">The descendant of the ancestor to be found.</param>
        /// <returns>The first ancestor of the given child having a type of T.</returns>
        public static T FindParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            // If there are no more parents left we've reached the top of the visual tree and not found the requested type.
            DependencyObject genericParent = VisualTreeHelper.GetParent(child);
            if (genericParent == null)
            {
                return null;
            }

            // This will recursively traverse the visual tree upwards until the given ancestor with a type of T is found.
            T typedParent = genericParent as T;
            return typedParent == null ? FindParent<T>(genericParent) : typedParent;
        }

        /// <summary>
        /// Finds the first descendant of the given object having a type of T.
        /// </summary>
        /// <typeparam name="T">The type of the descendant to be found.</typeparam>
        /// <param name="parent">The ancestor of the descendant to be found.</param>
        /// <returns>The first descendant of the given parent having a type of T.</returns>
        public static T FindChild<T>(DependencyObject parent)
            where T : DependencyObject
        {
            // Recurse into the visual tree until a child of the given type is found.
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int index = 0; index < childCount; index++)
            {
                DependencyObject childObject = VisualTreeHelper.GetChild(parent, index);
                T child = childObject as T;
                if (child == null)
                {
                    child = VisualTreeExtensions.FindChild<T>(childObject);
                    if (child != null)
                    {
                        return child;
                    }
                }
                else
                {
                    // Found the control so return
                    return child;
                }
            }

            // At this point there is no child in this section of the visual tree with the given type.
            return null;
        }
    }
}