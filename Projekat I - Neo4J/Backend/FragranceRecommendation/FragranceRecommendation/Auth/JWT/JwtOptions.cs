namespace FragranceRecommendation.Auth.JWT;

public class JwtOptions
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SigningKey { get; set; }
    public TimeSpan JwtLifetime { get; set; }

    public JwtOptions(IConfiguration config)
    {
        Issuer = config.GetSection("JWT").GetSection("Issuer").Value!;
        Audience = config.GetSection("Jwt").GetSection("Audience").Value!;
        SigningKey = config.GetSection("Jwt").GetSection("Key").Value!;
        JwtLifetime = TimeSpan.Parse(config.GetSection("Jwt").GetSection("JwtLifetime").Value!);
    }
}