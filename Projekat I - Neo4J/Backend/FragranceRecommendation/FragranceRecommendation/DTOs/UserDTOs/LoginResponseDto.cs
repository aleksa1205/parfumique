using FragranceRecommendation.Auth;

namespace FragranceRecommendation.DTOs.UserDTOs;

public class LoginResponseDto
{
    public required string Username { get; set; }
    public required string Token { get; set; }
    public required Roles Role { get; set; }
}