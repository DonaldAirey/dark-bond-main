// <copyright file="ValidatingViewModelBase.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// An abstract class for creating view models that can be validated using rules.
    /// </summary>
    public abstract class ValidatingViewModelBase : ViewModel
    {
        /// <summary>
        /// A dictionary of property names and their validation rules.
        /// </summary>
        private readonly Dictionary<string, ValidationRule> validationRules;

        /// <summary>
        /// An indication of whether the dialog has attempted to validate the properties.
        /// </summary>
        private bool isSubmitted = false;

        /// <summary>
        /// A dictionary of the current validation errors associated with each property in the view model.
        /// </summary>
        private Dictionary<string, string> propertyErrors = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatingViewModelBase"/> class.
        /// </summary>
        protected ValidatingViewModelBase()
        {
            // HACK fix all this
            // This handler will filter the events looking specifically for properties that have validation rules.
            this.PropertyChanged += this.OnPropertyChanged;

            // The power of LINQ is just amazing.  This all could have been made into a single statement, but in the interest of readability, it's
            // been broken into two parts.  The main idea os to construct a dictionary.  The key is the name of the parameter and the value is a
            // structure that provides quick and easy access to the getter for that property and a list of all the validation functions associated
            // with the property.  While the list could have been constructed on-the-fly, it is more convenient to have them ahead of time as the
            // validation rules are static for the life of the view model.  This first statement pulls together all the parameters of this view model
            // that have validation rules.
            var validatingParameters = from param in this.GetType().GetRuntimeProperties()
                                       where param.GetCustomAttributes(typeof(ValidationAttribute), true).Length != 0
                                       select param;

            // This creates the dictionary that is used to validate the parameters.  The key is the parameter name and the value is the getter for
            // that parameter and the validation rules for that parameter.
            this.validationRules = validatingParameters.ToDictionary(
                param => param.Name,
                param => new ValidationRule(
                    new Func<ValidatingViewModelBase, object>(viewmodelBase => param.GetValue(viewmodelBase, null)),
                    param.GetCustomAttributes(typeof(ValidationAttribute), true) as ValidationAttribute[]));
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        public string Error
        {
            get
            {
                // Collect the errors in the form and join them using the system defined newline.
                return string.Join(Environment.NewLine, this.propertyErrors.Values.ToArray<string>());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view model is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                // This will enable the validation checking on the properties of this view model and raise an event indicating that all the rules
                // should be checked immediately.
                if (!this.isSubmitted)
                {
                    this.isSubmitted = true;
                    foreach (string property in this.validationRules.Keys)
                    {
                        this.OnPropertyChanged(property);
                    }

                    this.OnPropertyChanged("Item[]");
                }

                // The view model is valid when there are no errors.
                return this.propertyErrors.Count == 0;
            }

            set
            {
                // This will cause the valid status to be re-evaluated the next time it is called.
                this.isSubmitted = value;
            }
        }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>The error string associated with the given property.</returns>
        public string this[string propertyName]
        {
            get
            {
                // This view model will not provide validation feedback until it has been submitted.  This gives the user a chance to fill in all the
                // fields before you yell at them about what isn't right.
                string error;
                if (this.isSubmitted && this.propertyErrors.TryGetValue(propertyName, out error))
                {
                    return error;
                }

                // At this point the property either isn't validating, or is validating and has no errors, or we haven't submitted the form yet.
                return null;
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event raised when a property that requires validation is changed on a component.
        /// </summary>
        /// <param name="sender">The source of the event. </param>
        /// <param name="propertyChangedEventArgs">A <see cref="PropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnValidatingPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
        }

        /// <summary>
        /// Handles the PropertyChanged event raised when a property is changed on a component.
        /// </summary>
        /// <param name="sender">The source of the event. </param>
        /// <param name="propertyChangedEventArgs">A <see cref="PropertyChangedEventArgs"/> that contains the event data.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            // If the property has validation rules then we'll see if there are any violations and, if there are, place them in the indexer.
            string propertyName = propertyChangedEventArgs.PropertyName;
            ValidationRule validationRule;
            if (this.validationRules.TryGetValue(propertyChangedEventArgs.PropertyName, out validationRule))
            {
                // Clear out the previous error (if it exists).  The old error message will be compared against the new error message to determine if
                // the error properties (accessed through the indexer of this class) needs to be refreshed.
                string oldErrorMessage = null;
                if (this.propertyErrors.TryGetValue(propertyName, out oldErrorMessage))
                {
                    this.propertyErrors.Remove(propertyName);
                }

                // The general idea here is to cycle through all the validators associated with the given property and evaluate whether any of the
                // rules have been violated.  When we find a violated rule it's placed in the indexer that is bound to the error messages associated
                // with each field.
                string newErrorMessage = null;
                foreach (ValidationAttribute validationAttribute in validationRule.Validators)
                {
                    ValidationContext validationContext = new ValidationContext(this) { MemberName = propertyName };
                    ValidationResult validationResult = validationAttribute.GetValidationResult(validationRule.Getter(this), validationContext);
                    if (validationResult != ValidationResult.Success)
                    {
                        newErrorMessage = validationResult.ErrorMessage;
                        this.propertyErrors[propertyName] = newErrorMessage;
                        break;
                    }
                }

                // This is essentially the notification backing for the errors in this view model.  When the previous error is not the same as the
                // current error, then we're going to raise an event which says the indexer is out of date and needs to be refreshed.  Unfortunately,
                // the indexer will blast through every property in the view model, so we're gone to a little extra effort to make sure it only fires
                // when it needs to.
                if (oldErrorMessage != newErrorMessage)
                {
                    this.OnPropertyChanged("Item[]");
                }

                // This virtual method can be used by the inheriting class to indicate it's time to re-validate the form.  This is useful if the
                // submit button (or some other high level control) is used to indicate that the entire form is valid.
                this.OnValidatingPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Manages the validation rules associated with a property.
        /// </summary>
        private class ValidationRule
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ValidationRule"/> class.
            /// </summary>
            /// <param name="getter">The function that gets the value of a property.</param>
            /// <param name="validators">The validation rules associated with a property.</param>
            public ValidationRule(Func<ValidatingViewModelBase, object> getter, IEnumerable<ValidationAttribute> validators)
            {
                this.Getter = getter;
                this.Validators = validators;
            }

            /// <summary>
            /// Gets or sets the method that gets the value of a property.
            /// </summary>
            public Func<ValidatingViewModelBase, object> Getter
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the list of rules associated with a property.
            /// </summary>
            public IEnumerable<ValidationAttribute> Validators
            {
                get;
                set;
            }
        }
    }
}