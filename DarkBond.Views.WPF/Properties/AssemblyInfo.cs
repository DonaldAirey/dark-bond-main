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
[assembly: AssemblyTitle("DarkBond.PresentationFramework")]
[assembly: AssemblyDescription("A support library for the presentation of data.")]
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
[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames", Justification = "Reviewed")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DarkBond", Justification = "Reviewed")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DarkBond", Scope = "namespace", Target = "DarkBond", Justification = "Reviewed")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DarkBond", Scope = "namespace", Target = "DarkBond.Windows", Justification = "Reviewed")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DarkBond", Scope = "namespace", Target = "DarkBond.Views.Controls.Primitives", Justification = "Reviewed")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DarkBond", Scope = "namespace", Target = "DarkBond.Views.Documents", Justification = "Reviewed")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DarkBond", Scope = "namespace", Target = "DarkBond.Views.Navigation", Justification = "Reviewed")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DarkBond", Scope = "namespace", Target = "DarkBond.Views.Input", Justification = "Reviewed")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DarkBond", Scope = "namespace", Target = "DarkBond.Views.Data", Justification = "Reviewed")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DarkBond", Scope = "namespace", Target = "DarkBond.Views.Controls", Justification = "Reviewed")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "DarkBond.Views.Controls.Primitives", Justification = "Reviewed")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "DarkBond.Views.Documents", Justification = "Reviewed")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "DarkBond.Views.Input", Justification = "Reviewed")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "DarkBond.Views.Navigation", Justification = "Reviewed")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "XamlGeneratedNamespace", Justification = "Reviewed")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "DarkBond.View", Justification = "Will probably add more classes to this namespace soon")]