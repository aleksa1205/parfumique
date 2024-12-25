namespace FragranceRecommendation.DTOs.UserDTOs;

public class LoginDto
{
    public string Username { get; set; }
    public string Password { get; set; }

    public (bool isValid, string ErrorMessage) Validate()
    {
        var (isValid, errorMessage) = MyUtils.IsValidString(Username, "Username");
        if (!isValid)
            return (false, errorMessage);

        (isValid, errorMessage) = MyUtils.IsValidPassword(Password);
        if (!isValid)
            return (false, errorMessage);
        
        return (true, string.Empty);
    }
}