﻿// <copyright file="DelegateReference.cs" company="DarkBond, Inc.">
//     Copyright © 2015 - DarkBond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.View.Events
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Represents a reference to a <see cref="Delegate"/> that may contain a <see cref="WeakReference"/> to the target.  This class is used
    /// internally by the Composite Application Library.
    /// </summary>
    public class DelegateReference : IDelegateReference
    {
        /// <summary>
        /// The delegate.
        /// </summary>
        private readonly Delegate targetDelegate;

        /// <summary>
        /// The weak reference to the target.
        /// </summary>
        private readonly WeakReference targetWeakReference;

        /// <summary>
        /// The method to be invoked.
        /// </summary>
        private readonly MethodInfo targetMethod;

        /// <summary>
        /// The delegate's type.
        /// </summary>
        private readonly Type targetDelegateType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateReference"/> class.
        /// </summary>
        /// <param name="delegate">The original <see cref="Delegate"/> to create a reference for.</param>
        /// <param name="keepReferenceAlive">If <see langword="false" /> the class will create a weak reference to the delegate, allowing it to be garbage collected. Otherwise it will keep a strong reference to the target.</param>
        public DelegateReference(Delegate @delegate, bool keepReferenceAlive)
        {
            if (@delegate == null)
            {
                throw new ArgumentNullException("delegate");
            }

            if (keepReferenceAlive)
            {
                this.targetDelegate = @delegate;
            }
            else
            {
                this.targetWeakReference = new WeakReference(@delegate.Target);
                this.targetMethod = @delegate.GetMethodInfo();
                this.targetDelegateType = @delegate.GetType();
            }
        }

        /// <summary>
        /// Gets the <see cref="Delegate" /> (the target) referenced by the current <see cref="DelegateReference"/> object.
        /// </summary>
        /// <value><see langword="null"/> if the object referenced by the current <see cref="DelegateReference"/> object has been garbage collected; otherwise, a reference to the <see cref="Delegate"/> referenced by the current <see cref="DelegateReference"/> object.</value>
        public Delegate Target
        {
            get
            {
                if (this.targetDelegate != null)
                {
                    return this.targetDelegate;
                }
                else
                {
                    return this.TryGetDelegate();
                }
            }
        }

        /// <summary>
        /// Try to get the delegate.
        /// </summary>
        /// <returns>The delegate to the target method.</returns>
        private Delegate TryGetDelegate()
        {
            if (this.targetMethod.IsStatic)
            {
                return this.targetMethod.CreateDelegate(this.targetDelegateType);
            }

            object target = this.targetWeakReference.Target;
            if (target != null)
            {
                return this.targetMethod.CreateDelegate(this.targetDelegateType, target);
            }

            return null;
        }
    }
}