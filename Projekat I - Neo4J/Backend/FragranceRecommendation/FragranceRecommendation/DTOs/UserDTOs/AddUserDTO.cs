using System.ComponentModel.DataAnnotations;

namespace FragranceRecommendation.DTOs.UserDTOs;

public class AddUserDto
{
    [Required]
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required char Gender { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}
