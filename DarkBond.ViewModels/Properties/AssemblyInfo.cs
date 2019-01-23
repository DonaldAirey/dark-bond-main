// <copyright file="AssemblyInfo.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

// General information about the assembly.
[assembly: AssemblyDescription("A library of common view models.")]
[assembly: AssemblyCopyright("Copyright © 2016-2018, Dark Bond, Inc.  All rights reserved.")]
[assembly: AssemblyTrademark("The Essential Element")]

// Indicates that this assembly is compliant with the Common Language Specification (CLS).
[assembly: CLSCompliant(false)]

// Disables the accessibility of this assembly to COM.
[assembly: ComVisible(false)]

// Describes the default language used for the resources.
[assembly: NeutralResourcesLanguageAttribute("en")]

// Suppress these FXCop issues.
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "DarkBond.ViewModels.Input", Justification = "Reviewed")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DarkBond", Justification = "Spelled Correctly")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DarkBond", Scope = "namespace", Target = "DarkBond.ViewModel", Justification = "Spelled Correctly")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DarkBond", Scope = "namespace", Target = "DarkBond.ViewModels.Input", Justification = "Spelled Correctly")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "DarkBond", Scope = "namespace", Target = "DarkBond.ViewModels.Events", Justification = "Spelled Correctly")]
[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames", Justification = "Reviewed")]
