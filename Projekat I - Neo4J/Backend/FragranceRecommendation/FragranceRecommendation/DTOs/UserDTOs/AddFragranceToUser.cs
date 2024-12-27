using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FragranceRecommendation.DTOs.UserDTOs;

public class AddFragranceToUser
{
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string? Username { get; set; }

    [Range(0, int.MaxValue)]
    public int Id { get; set; }
}