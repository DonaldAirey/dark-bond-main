// <copyright file="ProvinceCountryKey.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    using System;

    /// <summary>
    /// The view model for a province (state).
    /// </summary>
    public struct ProvinceCountryKey
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProvinceCountryKey"/> struct.
        /// </summary>
        /// <param name="countryId">The country code.</param>
        /// <param name="provinceId">The province code.</param>
        public ProvinceCountryKey(Guid countryId, Guid? provinceId)
            : this()
        {
            // Initialize the object.
            this.CountryId = countryId;
            this.ProvinceId = provinceId;
        }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        public Guid CountryId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the province.
        /// </summary>
        public Guid? ProvinceId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns true if the values of its operands are equal, false otherwise.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if the values of its operands are equal, false otherwise.</returns>
        public static bool operator ==(ProvinceCountryKey left, ProvinceCountryKey right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns false if its operands are equal, true otherwise.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>False if its operands are equal, true otherwise</returns>
        public static bool operator !=(ProvinceCountryKey left, ProvinceCountryKey right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether two object instances are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            // Null arguments are never equal.
            if (obj == null)
            {
                return false;
            }

            // This key can't be compared to other objects.
            if (obj.GetType() != typeof(ProvinceCountryKey))
            {
                return false;
            }

            // The keys are not equal if any of the fields in the index are null.
            ProvinceCountryKey provinceKey = (ProvinceCountryKey)obj;
            if (this.CountryId == null || this.ProvinceId == null)
            {
                return false;
            }

            return this.CountryId.Equals(provinceKey.CountryId) && this.ProvinceId.Equals(provinceKey.ProvinceId);
        }

        /// <summary>
        /// Serves as the hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return this.CountryId.GetHashCode() + this.ProvinceId.GetHashCode();
        }
    }
}