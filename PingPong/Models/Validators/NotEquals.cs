using System.ComponentModel.DataAnnotations;

namespace PingPong.Models.Validators
{
    public class NotEquals : ValidationAttribute
    {
        private readonly string[] _propertyNames;

        public NotEquals(params string[] PropertyNames)
        {
            _propertyNames = PropertyNames;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            foreach(var propertyName in _propertyNames)
            {
                var comparisonProperty = validationContext.ObjectType.GetProperty(propertyName);
                if (comparisonProperty == null)
                {
                    throw new ArgumentException(string.Format("Property {0} not found", propertyName));
                }

                var comparisonValue = comparisonProperty.GetValue(validationContext.ObjectInstance, null);
                if ((value == null && comparisonValue == null) || (value != null && value.Equals(comparisonValue)))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }

}
