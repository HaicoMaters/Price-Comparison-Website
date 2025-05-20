using System.ComponentModel.DataAnnotations;

namespace PriceComparisonWebsite.Validation
{
    /// <summary>
    /// Validation attribute that ensures a decimal value is less than another property's value
    /// </summary>
    public class LessThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        /// <summary>
        /// Initializes a new instance of the LessThanAttribute
        /// </summary>
        /// <param name="comparisonProperty">The name of the property to compare against</param>
        public LessThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        /// <summary>
        /// Determines whether the value is valid by comparing it with another property's value
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="validationContext">The context information about the validation operation</param>
        /// <returns>ValidationResult.Success if the value is valid; otherwise, an error ValidationResult</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            var currentValue = (decimal)value;
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            var comparisonValue = (decimal)property.GetValue(validationContext.ObjectInstance);

            return currentValue < comparisonValue 
                ? ValidationResult.Success 
                : new ValidationResult(ErrorMessage);
        }
    }
}
