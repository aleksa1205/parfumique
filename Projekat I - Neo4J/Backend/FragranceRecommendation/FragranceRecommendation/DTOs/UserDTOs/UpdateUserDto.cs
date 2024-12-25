namespace FragranceRecommendation.DTOs.UserDTOs;

public class UpdateUserDto
{    
    public string Username { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public char Gender { get; set; }

    public (bool isValid, string ErrorMessage) Validate()
    {
        var (isValid, errorMessage) = MyUtils.IsValidString(Username, "Username");
        if (!isValid)
            return (false, errorMessage);
        
        (isValid, errorMessage) = MyUtils.IsValidString(Name, "Name");
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