// <copyright file="ConditionCode.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    /// <summary>
    /// Conditions for the execution of an order.
    /// </summary>
    public enum ConditionCode
    {
        /// <summary>
        /// Execute all or none of the order.
        /// </summary>
        AllOrNone,

        /// <summary>
        /// Execute all or none and do not reduce the order.
        /// </summary>
        AllOrNoneDoNotReduce,

        /// <summary>
        /// Do not reduce the order.
        /// </summary>
        DoNotReduce
    }
}
