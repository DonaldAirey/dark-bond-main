// <copyright file="TestDataConfig.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.TradingPost.Data
{
    using System;

    /// <summary>
    /// For now a generic class that contains configuration values that are less prone to be tweaked versus
    /// something in the app.config 
    /// </summary>
    public static class TestDataConfig
    {
        // Misc/Standalone constants
        public const Char CsvDelimiter = ',';               // used to delimit strings read in from most of the raw data files.
        public const Char CommentMarker = '#';              // used to denote a commented-out line in the test data
        public const Int32 GenerateRandomNumberOfCreditCards = -1; //flag maps to config setting to generate a random amount of CCs

        /// <summary>
        /// categories of building heights
        /// </summary>
        public static class BuildingFloors
        {
            public const Int32 SkyscraperFloors = 150; // very tall building 
            public const Int32 LowriseFloors = 8;
            public const Int32 ApartmentsPerFloor = 20;
        }

        /// <summary>
        /// Class that defines the column order of data fields we are reading in from the Raw Consumer Debt 
        /// data file.  That file is based on the config setting: ConsumerDebtInput
        /// </summary>
        public static class RawConsumerDebtColumn
        {
            public const Int32 OpenedDate = 4;
            public const Int32 LastPaidDate = 5;
            public const Int32 CollectionDate = 6;
            public const Int32 DateOfDelinquency = 7;
            public const Int32 InterestRate = 11;
        }

        /// <summary>
        /// 
        /// </summary>
        public static class RawUserColumn
        {
            public const Int32 Name = 0;
            public const Int32 Password = 1;
            public const Int32 Organization = 2;
            public const Int32 Email = 3;
        }
    }
}