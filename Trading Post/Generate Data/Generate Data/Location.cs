// <copyright file="Location.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.TradingPost.Data
{
    using System;

    public struct Location
	{

		// Public instance Fields
		public String City;
		public String ProvinceCode;
		public String PostalCode;

		/// <summary>
		/// Initializes a new instance of the <see cref="Location"/> struct.
		/// </summary>
		/// <param name="city">The city where they live.</param>
		/// <param name="state">The state/province where they live.</param>
		/// <param name="postalCode">The location code where they live.</param>
		public Location(String city, String provinceCode, String postalCode)
		{
			// Initialize the object
			this.City = city;
            this.ProvinceCode = provinceCode;
			this.PostalCode = postalCode;
		}
	}
}