// <copyright file="ReportTypeCode.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    /// <summary>
    /// The types of dynamic reports found in the application.
    /// </summary>
    public enum ReportTypeCode
    {
        /// <summary>
        /// Detail of credit cards.
        /// </summary>
        CreditCardDetail,

        /// <summary>
        /// Report of destination orders.
        /// </summary>
        DestinationOrder,

        /// <summary>
        /// Detail of destination orders.
        /// </summary>
        DestinationOrderDetail,

        /// <summary>
        /// Report of executions.
        /// </summary>
        Execution,

        /// <summary>
        /// Detail of executions.
        /// </summary>
        ExecutionDetail,

        /// <summary>
        /// Report of matched orders.
        /// </summary>
        Match,

        /// <summary>
        /// Detail of payments.
        /// </summary>
        PaymentSummary,

        /// <summary>
        /// Report of quotes.
        /// </summary>
        Quote,

        /// <summary>
        /// Report of settlements.
        /// </summary>
        Settlement,

        /// <summary>
        /// Report of source orders.
        /// </summary>
        SourceOrder,

        /// <summary>
        /// Detail of source orders.
        /// </summary>
        SourceOrderDetail,

        /// <summary>
        /// Report of working orders.
        /// </summary>
        WorkingOrder,

        /// <summary>
        /// Static report.
        /// </summary>
        StaticReport
    }
}
