#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
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
    public class ExceptionMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DarkBond.ServiceModel.Properties.ExceptionMessages", typeof(ExceptionMessages).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The argument &apos;{0}&apos; is not allowed to be null..
        /// </summary>
        public static string ArgumentException {
            get {
                return ResourceManager.GetString("ArgumentException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CopyTo target is not a one-dimensional array..
        /// </summary>
        public static string BadTargetArray {
            get {
                return ResourceManager.GetString("BadTargetArray", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Items collection must be empty before using ItemsSource..
        /// </summary>
        public static string CannotUseItemsSource {
            get {
                return ResourceManager.GetString("CannotUseItemsSource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Number of elements in source Enumerable is greater than available space from index to the end of destination array..
        /// </summary>
        public static string CopyToNotEnoughSpace {
            get {
                return ResourceManager.GetString("CopyToNotEnoughSpace", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The evaluation period for type &apos;{0}&apos; has expired..
        /// </summary>
        public static string ExpiredEvaluationLicense {
            get {
                return ResourceManager.GetString("ExpiredEvaluationLicense", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The license for type &apos;{0}&apos; has expired..
        /// </summary>
        public static string ExpiredLicense {
            get {
                return ResourceManager.GetString("ExpiredLicense", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A valid license could not be granted for the type &apos;{0}&apos;..
        /// </summary>
        public static string InvalidLicense {
            get {
                return ResourceManager.GetString("InvalidLicense", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified path &apos;{0}&apos; does not exist..
        /// </summary>
        public static string InvalidPath {
            get {
                return ResourceManager.GetString("InvalidPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ViewableCollection cannot provide a sync root for synchronization because the underlying collection can be changed..
        /// </summary>
        public static string ItemCollectionShouldUseInnerSyncRoot {
            get {
                return ResourceManager.GetString("ItemCollectionShouldUseInnerSyncRoot", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operation is not valid while ItemsSource is in use. Access and modify elements with ItemsControl.ItemsSource instead..
        /// </summary>
        public static string ItemsSourceInUse {
            get {
                return ResourceManager.GetString("ItemsSourceInUse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is not allowed for this view..
        /// </summary>
        public static string MemberNotAllowedForView {
            get {
                return ResourceManager.GetString("MemberNotAllowedForView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} template requires a {1} part..
        /// </summary>
        public static string MissingPart {
            get {
                return ResourceManager.GetString("MissingPart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; must be of type &apos;{1}&apos;..
        /// </summary>
        public static string MustBeOfType {
            get {
                return ResourceManager.GetString("MustBeOfType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only one root element is allowed..
        /// </summary>
        public static string OnlyOneRootAllowed {
            get {
                return ResourceManager.GetString("OnlyOneRootAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} property cannot be null..
        /// </summary>
        public static string PropertyCannotBeNull {
            get {
                return ResourceManager.GetString("PropertyCannotBeNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to URI Mapping template cannot contain the same identifier more than once..
        /// </summary>
        public static string UriMappingUriTemplateCannotRepeatIdentifier {
            get {
                return ResourceManager.GetString("UriMappingUriTemplateCannotRepeatIdentifier", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specified index &apos;{0}&apos; is out of range.  Do not call this method if VisualChildrenCount returns zero, indicating that the Visual has no children..
        /// </summary>
        public static string VisualArgumentOutOfRange {
            get {
                return ResourceManager.GetString("VisualArgumentOutOfRange", resourceCulture);
            }
        }
    }
}
#pragma warning restore 1591