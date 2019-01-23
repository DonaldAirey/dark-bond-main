// <copyright file="Program.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.OrderManagementSystem
{
    using System.IO;
    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point for the web service.
        /// </summary>
        /// <param name="args">The environment arguments.</param>
        public static void Main(string[] args)
        {
            // Build the host and run it.
            var webHostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();
            webHostBuilder.Run();
        }
    }
}