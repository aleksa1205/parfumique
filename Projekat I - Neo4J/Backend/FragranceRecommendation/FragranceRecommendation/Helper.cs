namespace FragranceRecommendation;

public static class Helper
{
    public static string GetJson(INode node)
    {
        var properties = node.Properties;
        var json = JsonConvert.SerializeObject(properties);
        return json;
    }
}
