using System.ComponentModel.DataAnnotations;

namespace PingPong.Models.Validators
{
    public class NotEquals : ValidationAttribute
    {
        private readonly string[] _propertyNames;
        private readonly string _defaultMessageTemplate = "{0} cannot be the same as {1}";

        public NotEquals(params string[] PropertyNames)
        {
            _propertyNames = PropertyNames;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var errorMessage = !String.IsNullOrEmpty(ErrorMessage) ? ErrorMessage
                : String.Format(this._defaultMessageTemplate, validationContext.MemberName, string.Join(", ", _propertyNames));

            foreach(var propertyName in _propertyNames)
            {
                var comparisonProperty = validationContext.ObjectType.GetProperty(propertyName);
                if (comparisonProperty == null)
                {
                    throw new ArgumentException(String.Format("Property {0} not found", propertyName));
                }

                var comparisonValue = comparisonProperty.GetValue(validationContext.ObjectInstance, null);
                if (value == null && comparisonValue == null)
                {
                    return new ValidationResult(errorMessage);
                } else if (value != null && value.Equals(comparisonValue))
                {
                    return new ValidationResult(errorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }

}
