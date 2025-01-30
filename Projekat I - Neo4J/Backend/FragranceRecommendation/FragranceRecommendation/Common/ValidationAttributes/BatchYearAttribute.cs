namespace FragranceRecommendation.Common.ValidationAttributes;

public class BatchYearAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        var currValue = (int)value;
        var currYear = DateTime.Now.Year;
        if (currValue >= 1900 && currValue <= currYear)
            return ValidationResult.Success;

        return new ValidationResult($"Batch year must be between 1900 to {currYear}.");
    }
}