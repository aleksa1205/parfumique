namespace FragranceRecommendation.DTOs.UserDTOs;

public class DeleteUserDto
{
    public required string Username { get; set; }

    public (bool isValid, string errorMessage) Validate()
    {
        var (isValid, errorMessage) = MyUtils.IsValidString(Username, "Username");
        return isValid ?(true, string.Empty) : ((false, errorMessage));
    }
}