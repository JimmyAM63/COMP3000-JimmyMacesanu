using System;
using System.ComponentModel.DataAnnotations;

public class StartsWithUpperCaseAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        string strValue = value as string;

        if (strValue != null && !string.IsNullOrEmpty(strValue))
        {
            if (char.IsUpper(strValue[0]))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("The first character must be uppercase.");
            }
        }
        return ValidationResult.Success;
    }
}
