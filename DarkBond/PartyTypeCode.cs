// <copyright file="PartyTypeCode.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond
{
    /// <summary>
    /// Classification of parties to a transaction.
    /// </summary>
    public enum PartyTypeCode
    {
        /// <summary>
        /// A broker that acts only as an agent.
        /// </summary>
        Agency,

        /// <summary>
        /// A broker that acts as a principal or agent.
        /// </summary>
        Broker,

        /// <summary>
        /// A negotiator of consumer debt.
        /// </summary>
        ConsumerTrust,

        /// <summary>
        /// An owner of consumer debt.
        /// </summary>
        ConsumerDebt,

        /// <summary>
        /// A Hedge Fund.
        /// </summary>
        Hedge,

        /// <summary>
        /// A traditional Money Manager.
        /// </summary>
        MoneyManager,

        /// <summary>
        /// Not a valid Counter-Party.
        /// </summary>
        NotValid,

        /// <summary>
        /// Use the parent in a hierarchy of party types.
        /// </summary>
        UseParent
    }
}
