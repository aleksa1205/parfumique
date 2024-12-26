namespace FragranceRecommendation.Models;

public class User
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
    
    [JsonProperty("password")]
    public required string Password { get; set; }
    
    //doesn't have to be property because get is not used anywhere
    public IList<Fragrance> Collection { get; set; } = new List<Fragrance>();

    #region Constructors
    public User() {}

    public User(string name, string surname, char gender, string username, string password)
    {
        Name = name;
        Surname = surname;
        Gender = gender;
        Username = username;
        Password = password;
    }

    public User(int id, string image, string name, string surname, char gender, string username, string password)
    {
        Image = image;
        Name = name;
        Surname = surname;
        Gender = gender;
        Username = username;
        Password = password;
    }
    #endregion
}
