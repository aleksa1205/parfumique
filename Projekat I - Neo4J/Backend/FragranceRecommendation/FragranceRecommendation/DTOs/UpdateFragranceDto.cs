namespace FragranceRecommendation.DTOs;

public class UpdateFragranceDto
{
        public int Id { get; set; }
        public string? Name { get; set; }
        public char Gender { get; set; } = 'U';
        public int BatchYear { get; set; } 
}