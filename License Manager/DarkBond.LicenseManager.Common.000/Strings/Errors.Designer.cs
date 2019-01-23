// <auto-generated />
namespace DarkBond.LicenseManager.Strings
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;
    using System.Resources;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors")]
    [DebuggerNonUserCode()]
    [CompilerGenerated()]
    public class Errors
    {
        /// <summary>
        /// The resource manager.
        /// </summary>
        private static ResourceManager resourceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="Errors"/> class.
        /// </summary>
        public Errors()
        {
        }

        /// <summary>
        /// Gets the resource string indexed by the ArgumentFault key.
        /// </summary>
        public static string ArgumentFault
        {
            get
            {
                return Errors.ResourceManager.GetString("ArgumentFault", Errors.Culture);
            }
        }

        /// <summary>
        /// Gets the resource string indexed by the ConstraintFault key.
        /// </summary>
        public static string ConstraintFault
        {
            get
            {
                return Errors.ResourceManager.GetString("ConstraintFault", Errors.Culture);
            }
        }

        /// <summary>
        /// Gets or sets the culture to use for lookups using this strongly typed resource class.
        /// </summary>
        public static CultureInfo Culture { get; set; }

        /// <summary>
        /// Gets the resource string indexed by the CustomerNotFound key.
        /// </summary>
        public static string CustomerNotFound
        {
            get
            {
                return Errors.ResourceManager.GetString("CustomerNotFound", Errors.Culture);
            }
        }

        /// <summary>
        /// Gets the resource string indexed by the DateOfBirthViolation key.
        /// </summary>
        public static string DateOfBirthViolation
        {
            get
            {
                return Errors.ResourceManager.GetString("DateOfBirthViolation", Errors.Culture);
            }
        }

        /// <summary>
        /// Gets the resource string indexed by the IncorrectFormat key.
        /// </summary>
        public static string IncorrectFormat
        {
            get
            {
                return Errors.ResourceManager.GetString("IncorrectFormat", Errors.Culture);
            }
        }

        /// <summary>
        /// Gets the resource string indexed by the OptimisticConcurrencyFault key.
        /// </summary>
        public static string OptimisticConcurrencyFault
        {
            get
            {
                return Errors.ResourceManager.GetString("OptimisticConcurrencyFault", Errors.Culture);
            }
        }

        /// <summary>
        /// Gets the resource string indexed by the ProductNotFound key.
        /// </summary>
        public static string ProductNotFound
        {
            get
            {
                return Errors.ResourceManager.GetString("ProductNotFound", Errors.Culture);
            }
        }

        /// <summary>
        /// Gets the resource string indexed by the RecordNotFoundFault key.
        /// </summary>
        public static string RecordNotFoundFault
        {
            get
            {
                return Errors.ResourceManager.GetString("RecordNotFoundFault", Errors.Culture);
            }
        }

        /// <summary>
        /// Gets the resource string indexed by the RequiredFieldViolation key.
        /// </summary>
        public static string RequiredFieldViolation
        {
            get
            {
                return Errors.ResourceManager.GetString("RequiredFieldViolation", Errors.Culture);
            }
        }

        /// <summary>
        /// Gets the cached ResourceManager instance used by this class.
        /// </summary>
        public static ResourceManager ResourceManager
        {
            get
            {
                if (Errors.resourceManager == null)
                {
                    Errors.resourceManager = new ResourceManager("DarkBond.LicenseManager.Strings.Errors", typeof(Errors).GetTypeInfo().Assembly);
                }

                return Errors.resourceManager;
            }
        }
    }
}