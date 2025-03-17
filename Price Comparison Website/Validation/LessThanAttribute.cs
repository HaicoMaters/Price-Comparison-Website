using System.ComponentModel.DataAnnotations;

namespace Price_Comparison_Website.Validation
{
    public class LessThanAttribute : ValidationAttribute // Custom Validation Attribute for Decimal Properties to check if the value is less than another property
    {
        private readonly string _comparisonProperty;

        public LessThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

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
