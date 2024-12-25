using System.Formats.Asn1;

namespace FragranceRecommendation.Services.PerfumerService;

public class PerfumerService(IDriver driver) : IPerfumerService
{
    public async Task<bool> PerfumerExistsAsync(int id)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var query = @"OPTIONAL MATCH (n:PERFUMER)
                         WHERE id(n) = $id
                         RETURN n IS NOT NULL AS exists";
            var result = await tx.RunAsync(query, new { id });
            return (await result.SingleAsync())["exists"].As<bool>();
        });
    }

    public async Task<bool> IsFragranceCreatedByPerfumer(int perfumerId, int fragranceId)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var query = @"MATCH (n:PERFUMER) WHERE id(n) = $perfumerId
                        MATCH (m:FRAGRANCE) WHERE id(m) = $fragranceId
                        OPTIONAL MATCH (n) -[r:CREATES]-> (m)
                        RETURN r IS NOT NULL AS exists";
            var result = await tx.RunAsync(query, new { perfumerId, fragranceId });
            return (await result.SingleAsync())["exists"].As<bool>();
        });
    }

    public async Task<IList<INode>> GetPerfumersAsync()
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var result = await tx.RunAsync("MATCH (n:PERFUMER) RETURN n");
            var nodes = new List<INode>();
            await foreach (var record in result)
            {
                var node = record["n"].As<INode>();
                nodes.Add(node);
            }

            return nodes;
        });
    }

    public async Task<Perfumer?> GetPerfumerAsync(int id)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var query = @"MATCH (n:PERFUMER)
                          WHERE id(n) = $id
                          OPTIONAL MATCH (n) -[:CREATES]-> (f:FRAGRANCE)
                          RETURN n{.*, id: id(n)} AS perfumer, COLLECT (f{.*, id: id(f)}) AS fragrances";
            var result = await tx.RunAsync(query, new { id });
            var record = await result.PeekAsync();
            if(record is null)
                return null;
            
            var fragrances =
                JsonConvert.DeserializeObject<List<Fragrance>>(JsonConvert.SerializeObject(record["fragrances"]));
            var perfumer = JsonConvert.DeserializeObject<Perfumer>(JsonConvert.SerializeObject(record["perfumer"]));
            perfumer!.CreatedFragrances = fragrances!;
            return perfumer;
        });
    }

    public async Task AddPerfumerAsync(AddPerfumerDto perfumer)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"CREATE (:PERFUMER {name: $name, surname: $surname, gender: $gender, country: $country, image: ''})";
            await tx.RunAsync(query,
                new
                {
                    name = perfumer.Name, surname = perfumer.Surname, gender = perfumer.Gender,
                    country = perfumer.Country
                });
        });
    }

    public async Task UpdatePerfumerAsync(UpdatePerfumerDto perfumer)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"MATCH (n:PERFUMER)
                          WHERE id(n) = $id
                          SET n.name = $name, n.surname = $surname, n.gender = $gender, n.country = $country";
            await tx.RunAsync(query,
                new
                {
                    id = perfumer.Id, name = perfumer.Name, surname = perfumer.Surname, gender = perfumer.Gender,
                    country = perfumer.Country
                });
        });
    }

    public async Task AddFragranceToPerfumerAsync(AddFragranceToPerfumer dto)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"MATCH (n:PERFUMER) WHERE id(n) = $perfumerId
                          MATCH (m:FRAGRANCE) WHERE id(m) = $fragranceId
                          CREATE (n) -[:CREATES]-> (m)";
            await tx.RunAsync(query, new { perfumerId = dto.PerfumerId, fragranceId = dto.FragranceId });
        });
    }

    public async Task DeletePerfumerAsync(DeletePerfumerDto perfumer)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"MATCH(n:PERFUMER)
                          WHERE id(n) = $id
                          DETACH DELETE n";
            await tx.RunAsync(query, new { id = perfumer.Id });
        });
    }
}