// <copyright file="B2CPolicyOptions.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ServiceModel
{
    /// <summary>
    /// The policies.
    /// </summary>
    public class B2CPolicyOptions
    {
        /// <summary>
        /// Gets or sets the edit-profile policy.
        /// </summary>
        public string ProfileEditingPolicy { get; set; }

        /// <summary>
        /// Gets or sets the reset password policy.
        /// </summary>
        public string PasswordResetPolicy { get; set; }

        /// <summary>
        /// Gets or sets the sign-in policy.
        /// </summary>
        public string SignInPolicy { get; set; }

        /// <summary>
        /// Gets or sets the sign-in or sign-up policy.
        /// </summary>
        public string SignInSignUpPolicy { get; set; }
    }
}