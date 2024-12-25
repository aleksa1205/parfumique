namespace FragranceRecommendation.DTOs.UserDTOs;

public class AddUserDto
{
    [Required]
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required char Gender { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }

    public (bool IsValid, string ErrorMessage) Validate()
    {
        var (isValid, errorMessage) = MyUtils.IsValidString(Name, "Name");
        if (!isValid)
            return (false, errorMessage);
        
        (isValid, errorMessage) = MyUtils.IsValidString(Surname, "Surname");
        if (!isValid)
            return (false, errorMessage);
        
        (isValid, errorMessage) = MyUtils.IsValidString(Username, "Username");
        if (!isValid)
            return (false, errorMessage);
        
        (isValid, errorMessage) = MyUtils.IsValidPassword(Password);
        if (!isValid)
            return (false, errorMessage);
        
        (isValid, errorMessage) = MyUtils.IsValidGender(Gender);
        if (!isValid)
            return (false, errorMessage);

        return (true, string.Empty);
    }
}
