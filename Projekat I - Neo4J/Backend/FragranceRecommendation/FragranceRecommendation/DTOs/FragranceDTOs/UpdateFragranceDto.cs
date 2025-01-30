using FragranceRecommendation.Common.ValidationAttributes;

namespace FragranceRecommendation.DTOs.FragranceDTOs;

public class UpdateFragranceDto
{
        [Range(0, int.MaxValue)]
        public int Id { get; set; }

        [StringLength(30, MinimumLength = 3)]
        public string? Name { get; set; }

        [Gender]
        public char? Gender { get; set; }

        [BatchYear]
        public int? BatchYear { get; set; }

        public string? Image { get; set; }
}