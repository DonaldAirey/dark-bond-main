// <copyright file="AssemblyInfo.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;

// General information about the assembly.
[assembly: AssemblyTitle("DarkBond.License")]
[assembly: AssemblyDescription("Library of functions to support the UI of the License Manager.")]
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

// This instructs the loader that WPF themes and the generic theme can be found in this library.
[assembly: ThemeInfo(ResourceDictionaryLocation.SourceAssembly, ResourceDictionaryLocation.SourceAssembly)]

// These specific messages are suppressed when the Code Analysis is run.
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "DarkBond.LicenseManager")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "DarkBond.LicenseManager.Forms", Justification = "Required to avoid collisions")]
[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames", Justification = "Assembly has a strong name.")]
