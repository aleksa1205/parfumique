namespace FragranceRecommendation.DTOs.PerfumerDTOs;

public class AddPerfumerDto
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required char Gender { get; set; }
    public required string Country { get; set; }
    
    public (bool isValid, string ErrorMessage) Validate()
    {
        var (isValid, errorMessage) = MyUtils.IsValidString(Name, "Name");
        if (!isValid)
            return (false, errorMessage);
        
        (isValid, errorMessage) = MyUtils.IsValidString(Surname, "Surname");
        if (!isValid)
            return (false, errorMessage);

        (isValid, errorMessage) = MyUtils.IsValidGender(Gender);
        if (!isValid)
            return (false, errorMessage);

        return (true, string.Empty);
    }
}
