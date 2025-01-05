namespace FragranceRecommendation.Common.ValidationAttributes;

public class GenderAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return new ValidationResult("The Gender field is required.");

        if (value is char gender && (gender == 'M' || gender == 'F' || gender == 'U'))
            return ValidationResult.Success;

        return new ValidationResult("Gender must be 'M', 'F' or 'U'.");
    }
}