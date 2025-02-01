namespace FragranceRecommendation.DTOs.UserDTOs;

public class ReturnUserDto
{
    [JsonProperty("image")]
    public string? Image { get; set; } = String.Empty;

    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("surname")]
    public required string Surname { get; set; }

    [JsonProperty("gender")]
    public char Gender { get; set; }

    [JsonProperty("username")]
    public required string Username { get; set; }

    // [JsonProperty("admin")]
    // public required bool Admin { get; set; }

    //doesn't have to be property because get is not used anywhere
    public IList<Fragrance> Collection { get; set; } = new List<Fragrance>();
}