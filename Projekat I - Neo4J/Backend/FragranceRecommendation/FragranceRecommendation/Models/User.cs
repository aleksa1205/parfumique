using Neo4j.Driver.Mapping;

namespace FragranceRecommendation.Models;

public class User
{
    [MappingSource("id")]
    public int? Id { get; set; }
    
    [MappingSource("image")]
    public string? Image { get; set; } = String.Empty;
    
    [MappingSource("name")]
    public required string Name { get; set; }
    
    [MappingSource("surname")]
    public required string Surname { get; set; }
    
    [MappingSource("gender")]
    public char Gender { get; set; }
    
    [MappingSource("username")]
    public required string Username { get; set; }
    
    [MappingSource("password")]
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
        Id = id;
        Image = image;
        Name = name;
        Surname = surname;
        Gender = gender;
        Username = username;
        Password = password;
    }
    #endregion
}
