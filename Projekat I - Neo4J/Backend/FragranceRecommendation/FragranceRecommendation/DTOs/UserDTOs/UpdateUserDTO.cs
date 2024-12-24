namespace FragranceRecommendation.DTOs.UserDTOs;

public class UpdateUserDto
{    
    public string Username { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public char Gender { get; set; } = 'M';
}