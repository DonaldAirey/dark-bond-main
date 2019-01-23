// <copyright file="AssemblyInfo.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

// General information about the assembly.
[assembly: AssemblyTitle("DarkBond.LicenseManager.Common")]
[assembly: AssemblyDescription("A library of common types for the License Manager.")]
[assembly: AssemblyCompany("Dark Bond, Inc.")]
[assembly: AssemblyProduct("DarkBond")]
[assembly: AssemblyCopyright("Copyright © 2016-2017, Dark Bond, Inc.  All rights reserved.")]

// Indicates that this assembly is compliant with the Common Language Specification (CLS).
[assembly: CLSCompliant(false)]

// Disables the accessibility of this assembly to COM.
[assembly: ComVisible(false)]

// Describes the default language used for the resources.
[assembly: NeutralResourcesLanguageAttribute("en")]

// Version information for this assembly.
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// Suppress these analyzer issues.
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "DarkBond.LicenseManager", Justification = "The namespace is common to all architectures.")]
[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames", Justification = "Has strong name.")]
