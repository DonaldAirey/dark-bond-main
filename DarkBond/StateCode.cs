// <copyright file="StateCode.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    /// <summary>
    /// States of an order.
    /// </summary>
    public enum StateCode
    {
        /// <summary>
        /// The order has been acknowledged.
        /// </summary>
        Acknowledged,

        /// <summary>
        /// The cancellation was confirmed.
        /// </summary>
        CancelAcknowledged,

        /// <summary>
        /// The order is cancelled.
        /// </summary>
        Canceled,

        /// <summary>
        /// The order is awaiting a cancelation.
        /// </summary>
        CancelPending,

        /// <summary>
        /// The cancellation was rejected.
        /// </summary>
        CancelRejected,

        /// <summary>
        /// The order is done for the day.
        /// </summary>
        DoneForDay,

        /// <summary>
        /// The order is in an error state.
        /// </summary>
        Error,

        /// <summary>
        /// The order is in the initial state.
        /// </summary>
        Initial,

        /// <summary>
        /// The order has been rejected.
        /// </summary>
        Rejected,

        /// <summary>
        /// The replacement has been acknowledged.
        /// </summary>
        ReplaceAcknowledged,

        /// <summary>
        /// The order has been replaced.
        /// </summary>
        Replaced,

        /// <summary>
        /// The order is awaiting a replacement.
        /// </summary>
        ReplacePending,

        /// <summary>
        /// The replacement has been rejected.
        /// </summary>
        ReplaceRejected,

        /// <summary>
        /// The order has been sent.
        /// </summary>
        Sent,

        /// <summary>
        /// The order is stopped.
        /// </summary>
        Stopped
    }
}
