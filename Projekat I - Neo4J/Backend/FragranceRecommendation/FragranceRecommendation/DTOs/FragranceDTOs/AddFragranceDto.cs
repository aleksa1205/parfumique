namespace FragranceRecommendation.DTOs.FragranceDTOs;

public class AddFragranceDto
{
    public required string Name { get; set; }
    public required char Gender { get; set; }
    public int BatchYear { get; set; }
    
    public (bool isValid, string ErrorMessage) Validate()
    {
        var (isValid, errorMessage) = MyUtils.IsValidString(Name, "Name");
        if (!isValid)
            return (false, errorMessage);

        (isValid, errorMessage) = MyUtils.IsValidGenderFragrance(Gender);
        if (!isValid)
            return (false, errorMessage);
                
        if(BatchYear < 1)
            return (false, "Batch year must be a positive integer!");

        return (true, string.Empty);
    }
}