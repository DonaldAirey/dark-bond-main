// <copyright file="SubscriptionTypeCode.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.SubscriptionManager.Common
{
    /// <summary>
    /// Describes how a license is managed by the client.
    /// </summary>
    public enum SubscriptionTypeCode
    {
        /// <summary>
        /// This is not a valid license.  Used for testing.
        /// </summary>
        NotValid = 0x00,

        /// <summary>
        /// Evaluation License that reminds the user to register.
        /// </summary>
        EvaluationNag = 0x01,

        /// <summary>
        /// Evaluation 1 Month License.
        /// </summary>
        Evaluation1Month = 0x02,

        /// <summary>
        /// Evaluation 2 Month License.
        /// </summary>
        Evaluation2Month = 0x03,

        /// <summary>
        /// Evaluation 3 Month License.
        /// </summary>
        Evaluation3Month = 0x04,

        /// <summary>
        /// Evaluation 4 Month License.
        /// </summary>
        Evaluation4Month = 0x05,

        /// <summary>
        /// Evaluation 5 Month License.
        /// </summary>
        Evaluation5Month = 0x06,

        /// <summary>
        /// Evaluation 6 Month License.
        /// </summary>
        Evaluation6Month = 0x07,

        /// <summary>
        /// Full 1 Year License.
        /// </summary>
        Full1Year = 0x08,

        /// <summary>
        /// Full 2 Year License.
        /// </summary>
        Full2Year = 0x09,

        /// <summary>
        /// Full 3 Year License.
        /// </summary>
        Full3Year = 0x0A,

        /// <summary>
        /// Full 4 Year License.
        /// </summary>
        Full4Year = 0x0B,

        /// <summary>
        /// Full 5 Year License.
        /// </summary>
        Full5Year = 0x0C,

        /// <summary>
        /// Perpetual License.
        /// </summary>
        Perpetual = 0x0D
    }
}
