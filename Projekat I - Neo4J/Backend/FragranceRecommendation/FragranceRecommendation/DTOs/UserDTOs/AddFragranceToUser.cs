using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FragranceRecommendation.DTOs.UserDTOs;

public class AddFragranceToUser
{
    public string Username { get; set; }
    public int Id { get; set; }
    
    public (bool isValid, string errorMessage) Validate()
    {
        var (isValid, errorMessage) = MyUtils.IsValidString(Username, "Username");
        if (!isValid)
            return (false, errorMessage);

        if (Id < 0)
            return (false, "Fragrance ID must be a positive integer!");

        return (true, string.Empty);
    }
}