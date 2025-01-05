namespace FragranceRecommendation;

public static class DbSettings
{
    public static IDriver GetDbDriver()
    {
        string? uri = Environment.GetEnvironmentVariable("NEO4J_FR_URI");
        string? username = Environment.GetEnvironmentVariable("NEO4J_FR_USERNAME");
        string? password = Environment.GetEnvironmentVariable("NEO4J_FR_PASSWORD");

        if (String.IsNullOrEmpty(uri) || String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
        {
            throw new AuthorizationException("Environment Variables for database are not set on this system!");
        }
        
        return GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
    }
}