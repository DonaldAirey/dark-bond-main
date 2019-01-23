// <copyright file="AssemblyInfo.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

// General information about the assembly.
[assembly: AssemblyTitle("DarkBond.LicenseManager.WorkerRole")]
[assembly: AssemblyDescription("Worker Role for the License Manager.")]
[assembly: AssemblyCompany("Dark Bond, Inc.")]
[assembly: AssemblyProduct("DarkBond")]
[assembly: AssemblyCopyright("Copyright © 2015, Dark Bond, Inc.  All rights reserved.")]

// Indicates that this assembly is compliant with the Common Language Specification (CLS).
[assembly: CLSCompliant(true)]

// Disables the accessibility of this assembly to COM.
[assembly: ComVisible(false)]

// Describes the default language used for the resources.
[assembly: NeutralResourcesLanguageAttribute("en-US")]

// Version information for this assembly.
[assembly: AssemblyVersion("1.0.1.0")]
[assembly: AssemblyFileVersion("1.0.1.0")]

// Suppress these analyzer issues.
[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames", Justification = "Has strong name.")]
