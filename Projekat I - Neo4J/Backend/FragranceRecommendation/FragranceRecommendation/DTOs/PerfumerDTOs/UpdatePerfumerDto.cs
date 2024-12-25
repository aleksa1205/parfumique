namespace FragranceRecommendation.DTOs.PerfumerDTOs;

public class UpdatePerfumerDto
{
    public required int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public char Gender { get; set; }
    public string Country { get; set; }
    
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
                        
        if (Id < 0)
            return (false, "Perfumer ID must be a positive integer!");

        return (true, string.Empty);
    }
}