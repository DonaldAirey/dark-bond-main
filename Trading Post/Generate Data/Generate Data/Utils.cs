// <copyright file="Utils.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.TradingPost.Data
{
    using System;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Generally useful utilities
    /// </summary>
    public static class Utils
    {

        /// <summary>
        /// Return the number of lines in the file that streamReader references.
        /// </summary>
        /// <param name="streamReader"></param>
        /// <returns></returns>
        public static int LinesOfTextInStream(StreamReader streamReader)
        {
            int numLines = 0;

            // even with text files that have a million lines, this is still pretty quick

            while (!streamReader.EndOfStream)
            {
                String rawConsumer = streamReader.ReadLine();
                numLines++;
            }
           
            return numLines;
        }

        /// <summary>
        /// Extension method which returns true/false depending on whether or not the value falls within min and max (inclusive)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Boolean IsWithinRange(this int value, int min, int max) 
        {
            if ((value <= max) && (value >= min))
                return true;
            
            return false;
        }

        
        /// <summary>
        /// Extension method which returns true/false depending on whether or not the value is odd
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean IsOddNumber(this int value)
        {
            return (value % 2 == 1);
        }

        /// <summary>
        /// Manually induces the Garbage Collector to run.
        /// </summary>
        public static void ForceGarbageCollection()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("Memory used before collection: {0}", GC.GetTotalMemory(false)));
            
            // Collect all generations of memory.
            GC.Collect();

            System.Diagnostics.Debug.WriteLine(String.Format("Memory used after full collection: {0}", GC.GetTotalMemory(true)));
        }

        /// <summary>
        /// Extension method that restarts (Stop/Reset/Start) the stopwatch
        /// </summary>
        /// <param name="value"></param>
        public static void Restart(this Stopwatch value)
        {
            value.Stop();
            value.Reset();
            value.Start();
        }
    }
}