namespace FragranceRecommendation.Common.ValidationAttributes;

public class PasswordAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return new ValidationResult("The Password field is required.");

        var str = value as string;

        if (string.IsNullOrWhiteSpace(str))
            return new ValidationResult("The Password field is required.");
        if (str.Length < 8)
            return new ValidationResult("Password must be at least 8 characters.");

        var hasDigit = false;
        foreach (var ch in str)
        {
            if (char.IsDigit(ch))
            {
                hasDigit = true;
                break;
            }
        }

        var hasLetter = false;
        foreach (var ch in str)
        {
            if (char.IsLetter(ch))
            {
                hasLetter = true;
                break;
            }
        }

        if (!hasDigit)
            return new ValidationResult("Password must contain at least 1 number");

        if (!hasLetter)
            return new ValidationResult("Password must contain at least 1 letter");

        return ValidationResult.Success;
    }
}