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
    
    [JsonProperty("year")]
    public int BirthYear { get; set; }
    
    public IList<Fragrance> CreatedFragrances  = new List<Fragrance>();

    #region Constructors
    public Perfumer() {}
    public Perfumer(string name, string surname, char gender, string country)
    {
        Name = name;
        Surname = surname;
        Gender = gender;
        Country = country;
        BirthYear = 0;
    }

    public Perfumer(string name, string surname, char gender, string country, int birthYear)
    {
        Name = name;
        Surname = surname;
        Gender = gender;
        Country = country;
        BirthYear = birthYear;
    }
    #endregion
}
