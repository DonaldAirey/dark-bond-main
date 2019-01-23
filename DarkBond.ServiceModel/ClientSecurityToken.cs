// <copyright file="ClientSecurityToken.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ServiceModel
{
    /// <summary>
    /// The security token from the Azure Active Directory.
    /// </summary>
    public class ClientSecurityToken
    {
        /// <summary>
        /// Gets or sets the security token.
        /// </summary>
        public object Value { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}