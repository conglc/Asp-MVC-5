using MvcTemplate.Resources;
using MvcTemplate.Resources.Form;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MvcTemplate.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class EqualToAttribute : ValidationAttribute
    {
        public String OtherPropertyName { get; }
        public String OtherPropertyDisplayName { get; set; }

        public EqualToAttribute(String otherPropertyName)
            : base(() => Validations.EqualTo)
        {
            OtherPropertyName = otherPropertyName;
        }

        public override String FormatErrorMessage(String name)
        {
            return String.Format(ErrorMessageString, name, OtherPropertyDisplayName);
        }
        protected override ValidationResult IsValid(Object value, ValidationContext context)
        {
            PropertyInfo other = context.ObjectType.GetProperty(OtherPropertyName);
            Object otherValue = other.GetValue(context.ObjectInstance);

            if (Equals(value, otherValue))
                return null;

            OtherPropertyDisplayName = ResourceProvider.GetPropertyTitle(context.ObjectType, OtherPropertyName) ?? OtherPropertyName;

            return new ValidationResult(FormatErrorMessage(context.DisplayName));
        }
    }
}
