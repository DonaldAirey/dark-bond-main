﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DarkBond.ServiceModel.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DarkBond.ServiceModel.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Access is denied.  The user doesn&apos;t have the required claims..
        /// </summary>
        internal static string AccessDeniedInsufficientClaimsError {
            get {
                return ResourceManager.GetString("AccessDeniedInsufficientClaimsError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The clustered index {0} must specify a set of columns.
        /// </summary>
        internal static string ClusteredIndexColumnError {
            get {
                return ResourceManager.GetString("ClusteredIndexColumnError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} record was deleted after it was locked..
        /// </summary>
        internal static string DeletedAfterLockedError {
            get {
                return ResourceManager.GetString("DeletedAfterLockedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0},.
        /// </summary>
        internal static string EmitByteArrayByteFormat {
            get {
                return ResourceManager.GetString("EmitByteArrayByteFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  .
        /// </summary>
        internal static string EmitByteArrayByteGap {
            get {
                return ResourceManager.GetString("EmitByteArrayByteGap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to static Byte[] {0} = {{.
        /// </summary>
        internal static string EmitByteArrayDeclaration {
            get {
                return ResourceManager.GetString("EmitByteArrayDeclaration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to \t\t{0}.
        /// </summary>
        internal static string EmitByteArrayHangingMargin {
            get {
                return ResourceManager.GetString("EmitByteArrayHangingMargin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to \t\t\t\t\t.
        /// </summary>
        internal static string EmitByteArrayMargin {
            get {
                return ResourceManager.GetString("EmitByteArrayMargin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}} };.
        /// </summary>
        internal static string EmitByteArrayTermination {
            get {
                return ResourceManager.GetString("EmitByteArrayTermination", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The index {0} on the &apos;{1}&apos; table doesn&apos;t exist.
        /// </summary>
        internal static string IndexNotFoundError {
            get {
                return ResourceManager.GetString("IndexNotFoundError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Security context is null.  User can not be authenticated..
        /// </summary>
        internal static string InvalidSecurityContextError {
            get {
                return ResourceManager.GetString("InvalidSecurityContextError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} is busy, please try again..
        /// </summary>
        internal static string OptimisticConcurrencyError {
            get {
                return ResourceManager.GetString("OptimisticConcurrencyError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The security principal has not been authenticated.
        /// </summary>
        internal static string PrincipalNotAuthenticatedError {
            get {
                return ResourceManager.GetString("PrincipalNotAuthenticatedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The record ({0}) in the &apos;{1}&apos; table doesn&apos;t exist..
        /// </summary>
        internal static string RecordNotFoundError {
            get {
                return ResourceManager.GetString("RecordNotFoundError", resourceCulture);
            }
        }
    }
}
