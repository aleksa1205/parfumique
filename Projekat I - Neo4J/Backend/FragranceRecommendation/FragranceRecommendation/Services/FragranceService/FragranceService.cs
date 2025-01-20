namespace FragranceRecommendation.Services.FragranceService;

public class FragranceService(IDriver driver) : IFragranceService
{
    public async Task<bool> FragranceExistsAsync(int id)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var query = @"OPTIONAL MATCH (n:FRAGRANCE)
                          WHERE id(n) = $id
                          RETURN n IS NOT NULL AS exists";
            var result = await tx.RunAsync(query, new { id });
            return (await result.SingleAsync())["exists"].As<bool>();
        });
    }

    public async Task<bool> FragranceHasManufacturerAsync(int id)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var query = @"OPTIONAL MATCH (n:FRAGRANCE) WHERE id(n) = $id
                          OPTIONAL MATCH (:MANUFACTURER)-[r:MANUFACTURES]->(n)
                          RETURN r IS NOT NULL AS exists";
            var cursor = await tx.RunAsync(query, new { id });
            return (await cursor.SingleAsync())["exists"].As<bool>();
        });
    }

    public async Task<IList<Fragrance>> GetFragrancesAsync()
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var result = await tx.RunAsync("MATCH (n:FRAGRANCE) RETURN n{.*, id:id(n)}");
            var list = new List<Fragrance>();
            await foreach (var record in result)
            {
                list.Add(MyUtils.DeserializeMap<Fragrance>(record["n"]));
            } 
            return list;
        });
    }
    
    public async Task<PagintaionResponseDto> GetFragrancesAsyncPagination(int pageNumber, int pageSize)
    {
        await using var session = driver.AsyncSession();
        int skip = pageSize * (pageNumber - 1);
        var totalCount = (await GetFragrancesAsync()).Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var fragrances = await session.ExecuteReadAsync(async tx =>
        {
            var result = await tx.RunAsync("MATCH (n:FRAGRANCE) RETURN n{.*, id:id(n)} SKIP $skip LIMIT $limit",
                new { skip, limit = pageSize });
            var list = new List<Fragrance>();
            await foreach (var record in result)
            {
                list.Add(MyUtils.DeserializeMap<Fragrance>(record["n"]));
            }
            return list;
        });
        return new(skip, totalCount, totalPages, fragrances);
    }

    public async Task<IList<Fragrance>> GetFragrancesWithouthManufacturerAsync()
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var query = @"MATCH (n:FRAGRANCE) 
                          WHERE NOT (n) <-[:MANUFACTURES]- (:MANUFACTURER)  
                          RETURN n{.*, id: id(n)} AS fragrance";
            var result = await tx.RunAsync(query);
            var records = await result.ToListAsync();
            return records.Select(record => MyUtils.DeserializeMap<Fragrance>(record["fragrance"]))
                .ToList();
        });
    }

    public async Task<Fragrance?> GetFragranceAsync(int id)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"MATCH (n:FRAGRANCE)
                              WHERE id(n) = $id
                              OPTIONAL MATCH (n) <-[:MANUFACTURES]- (m:MANUFACTURER)
                              OPTIONAL MATCH (n) <-[:CREATES]- (p:PERFUMER)
                              OPTIONAL MATCH (n) -[:TOP]-> (t:NOTE)
                              OPTIONAL MATCH (n) -[:MIDDLE]-> (k:NOTE)
                              OPTIONAL MATCH (n) -[:BASE]-> (b:NOTE)
                              RETURN n{.*, id: id(n)} AS fragrance, m AS manufacturer, COLLECT(DISTINCT p{.*, id: id(p)}) AS perfumers, COLLECT(DISTINCT t) AS topNotes, COLLECT(DISTINCT k) AS middleNotes, COLLECT(DISTINCT b) AS baseNotes";
            var result = await tx.RunAsync(query, new { id });
            var record = await result.PeekAsync();
            if (record is null)
                return null;
            
            var manufacturerNode = record["manufacturer"].As<INode>();
            var manufacturer = manufacturerNode != null
                ? MyUtils.DeserializeNode<Manufacturer>(manufacturerNode)
                : null;

            var perfumers = MyUtils.DeserializeMap<List<Perfumer>>(record["perfumers"]);
                //JsonConvert.DeserializeObject<List<Perfumer>>(JsonConvert.SerializeObject(record["perfumers"]));
            var topNotes = record["topNotes"].As<List<INode>>()
                .Select(MyUtils.DeserializeNode<Note>).ToList();
            var middleNotes = record["middleNotes"].As<List<INode>>()
                .Select(MyUtils.DeserializeNode<Note>).ToList();
            var baseNotes = record["baseNotes"].As<List<INode>>()
                .Select(MyUtils.DeserializeNode<Note>).ToList();

            var fragrance =
                JsonConvert.DeserializeObject<Fragrance>(JsonConvert.SerializeObject(record["fragrance"]));
            fragrance!.Manufacturer = manufacturer;
            fragrance.Perfumers = perfumers!;
            fragrance.Top = topNotes!;
            fragrance.Middle = middleNotes!;
            fragrance.Base = baseNotes!;
            return fragrance;
        });
    }

    public async Task AddFragranceAsync(AddFragranceDto fragrance)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"CREATE (:FRAGRANCE {name: $name, year: $year, gender: $gender, image: ''})";
            await tx.RunAsync(query,
                new { name = fragrance.Name, year = fragrance.BatchYear, gender = fragrance.Gender });
        });
    }

    public async Task UpdateFragranceAsync(UpdateFragranceDto fragrance)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"MATCH (n:FRAGRANCE)
                          WHERE id(n) = $id
                          SET n.name = $name, n.year = $year, n.gender = $gender";
            await tx.RunAsync(query,
                new { id=fragrance.Id, name = fragrance.Name, year = fragrance.BatchYear, gender = fragrance.Gender });
        });
    }
    
    public async Task AddNotesToFragrance(NotesToFragranceDto dto)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"MATCH (f:FRAGRANCE) WHERE id(f) = $id
                              UNWIND $notes AS note
                              MATCH (n:NOTE {name: note.Name})
                              FOREACH (_ IN CASE WHEN note.TMB = 0 THEN [1] ELSE [] END |
                                MERGE (f)-[:TOP]->(n))
                              FOREACH (_ IN CASE WHEN note.TMB = 1 THEN [1] ELSE [] END |
                                MERGE (f)-[:MIDDLE]->(n))
                              FOREACH (_ IN CASE WHEN note.TMB = 2 THEN [1] ELSE [] END |
                                MERGE (f)-[:BASE]->(n))";
            await tx.RunAsync(query, new { notes = dto.Notes, id = dto.Id });
        });
    }

    public async Task DeleteNotesFromFragrance(NotesToFragranceDto dto)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"UNWIND $notes AS note
                              MATCH (f:FRAGRANCE) WHERE id(f) = $id
                              MATCH (n:NOTE {name: note.Name})
                              OPTIONAL MATCH (f) -[r:TOP]-> (n) WHERE note.TMB = 0 DELETE r
                              WITH f, n, note
                              OPTIONAL MATCH (f) -[r:MIDDLE]-> (n) WHERE note.TMB = 1 DELETE r
                              WITH f, n, note
                              OPTIONAL MATCH (f) -[r:BASE]-> (n) WHERE note.TMB = 2 DELETE r";
            await tx.RunAsync(query, new { notes = dto.Notes, id = dto.Id });
        });
    }

    public async Task DeleteFragranceAsync(DeleteFragranceDto fragrance)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"MATCH (n:FRAGRANCE)
                        WHERE id(n) = $id
                        DETACH DELETE (n)";
            await tx.RunAsync(query, new { id = fragrance.Id });
        });
    }

    public async Task<List<FragranceRecommendationDto>> RecommendFragrance(int fragranceId)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var query =
                @"MATCH (inputFrag:FRAGRANCE)-[:TOP|MIDDLE|BASE]->(n:NOTE)<-[r:TOP|MIDDLE|BASE]-(recommendedFrag:FRAGRANCE)
                          WHERE id(inputFrag) = $fragranceId and inputFrag <> recommendedFrag and (inputFrag.gender = recommendedFrag.gender OR recommendedFrag.gender = 'U')
                          WITH inputFrag, recommendedFrag, n, COLLECT(type(r)) as relationshipTypes
                          WITH inputFrag, recommendedFrag,
                            SUM(
                                REDUCE(
                                    acc = 0,
                                    relType IN relationshipTypes |
                                    acc + CASE
                                        WHEN relType = 'BASE' THEN 3
                                        WHEN relType = 'MIDDLE' THEN 2
                                        WHEN relType = 'BASE' THEN 1
                                        ELSE 0
                                    END
                                )
                            ) as score
                        ORDER BY score DESC
                        LIMIT 5
                        RETURN recommendedFrag{.*, id:id(recommendedFrag)}, score";

            var cursor = await tx.RunAsync(query, new {fragranceId});

            var recommendedFragrances = new List<FragranceRecommendationDto>();
            while (await cursor.FetchAsync())
            {
                var record = cursor.Current;
                var fragrance = MyUtils.DeserializeMap<Fragrance>(record["recommendedFrag"]);
                var score = record["score"].As<double>();
                recommendedFragrances.Add(new FragranceRecommendationDto
                {
                    Fragrance = fragrance,
                    Score = score
                });
            }

            return recommendedFragrances;
        });
    }
}