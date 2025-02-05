namespace FragranceRecommendation.Models;

public class Perfumer
{
    [JsonProperty("id")]
    public int? Id { get; set; }
    
    [JsonProperty("image")]
    public string? Image { get; set; } = String.Empty;
    
    [JsonProperty("name")]
    public required string Name { get; set; }
    
    [JsonProperty("surname")]
    public required string Surname { get; set; }
    
    [JsonProperty("gender")]
    public char Gender { get; set; }
    
    [JsonProperty("country")]
    public string? Country { get; set; }

    public IList<Fragrance> Fragrances { get; set; } = new List<Fragrance>();

    #region Constructors
    public Perfumer() {}
    public Perfumer(string name, string surname, char gender, string country)
    {
        Name = name;
        Surname = surname;
        Gender = gender;
        Country = country;
    }
    #endregion
}
