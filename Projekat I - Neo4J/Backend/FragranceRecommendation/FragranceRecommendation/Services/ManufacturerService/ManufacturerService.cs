namespace FragranceRecommendation.Services.ManufacturerService;

public class ManufacturerService(IDriver driver) : IManufacturerService
{
    public async Task<List<Manufacturer>> GetAllManufacturers()
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var query = @"MATCH (n:MANUFACTURER) RETURN n";

            var cursor = await tx.RunAsync(query);

            var manufacturers = new List<Manufacturer>();
            while (await cursor.FetchAsync())
            {
                var record = cursor.Current;
                var manufacturer = MyUtils.DeserializeNode<Manufacturer>(record["n"].As<INode>());
                manufacturers.Add(manufacturer!);
            }

            return manufacturers;
        });
    }
    public async Task<bool> ManufacturerExistsAsync(string name)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var query = @"OPTIONAL MATCH (n:MANUFACTURER {name: $name}) 
                          RETURN n IS NOT NULL AS exists";
            var result = await tx.RunAsync(query, new { name });
            return (await result.SingleAsync())["exists"].As<bool>();
        });
    }
    public async Task<bool> IsFragranceCreatedByManufacturerAsync(int fragranceId, string name)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var query = @"MATCH (n:MANUFACTURER) WHERE n.name = $name
                        MATCH (m:FRAGRANCE) WHERE id(m) = $fragranceId
                        OPTIONAL MATCH (n) -[r:MANUFACTURES]-> (m)
                        RETURN r IS NOT NULL AS exists";
            var result = await tx.RunAsync(query, new { name, fragranceId });
            return (await result.SingleAsync())["exists"].As<bool>();
        });
    }
    public async Task<Manufacturer?> GetManufacturerAsync(string name)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var query = @"MATCH (n:MANUFACTURER {name: $name})
                          OPTIONAL MATCH (n) -[:MANUFACTURES]-> (f:FRAGRANCE) 
                          RETURN n, COLLECT(DISTINCT f{.*, id: id(f)}) AS fragrances";
            var result = await tx.RunAsync(query, new { name });
            var record = await result.PeekAsync();
            if (record is null)
            {
                return null;
            }

            var fragrances = MyUtils.DeserializeMap<List<Fragrance>>(record["fragrances"]);
            var manufacturer = MyUtils.DeserializeNode<Manufacturer>(record["n"].As<INode>());
            manufacturer!.Fragrances = fragrances!;
            return manufacturer;
        });
    }
    public async Task AddManufacturerAsync(string name)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"CREATE (:MANUFACTURER {name: $name, image: ''})";
            await tx.RunAsync(query, new { name });
        });
    }

    public async Task AddFragranceToManufacturerAsync(int fragranceId, string name)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"MATCH (m:MANUFACTURER) WHERE m.name = $name
                          MATCH (f:FRAGRANCE) WHERE id(f) = $fragranceId
                          CREATE (m) -[:MANUFACTURES]-> (f)";

            await tx.RunAsync(query, new { name, fragranceId });
        });
    }

    public async Task DeleteManufacturerAsync(string name)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"MATCH (m:MANUFACTURER) WHERE m.name = $name
                          DETACH DELETE m";
            await tx.RunAsync(query, new { name });
        });
    }
}