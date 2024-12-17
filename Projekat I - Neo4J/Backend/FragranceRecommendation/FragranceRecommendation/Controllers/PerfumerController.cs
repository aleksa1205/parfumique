namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class PerfumerController(IDriver driver) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all perfumers as nodes")]
    [HttpGet]
    public async Task<IActionResult> GetAllPerfumers()
    {
        try
        {
            await using var session = driver.AsyncSession();
            var listOfCreators = await session.ExecuteReadAsync(async tx =>
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
            return Ok(listOfCreators);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("get perfumer by id")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPerfumerById(int id)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var perfumer = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (n:PERFUMER)
                              WHERE id(n) = $id
                              OPTIONAL MATCH (n) -[:CREATES]-> (f:FRAGRANCE)
                              RETURN n{.*, id: id(n)} AS perfumer, COLLECT (f{.*, id: id(f)}) AS fragrances";
                var result = await tx.RunAsync(query, new { id });
                var record = await result.PeekAsync();
                if (record is null)
                {
                    return null;
                }

                var fragrances =
                    JsonConvert.DeserializeObject<List<Fragrance>>(JsonConvert.SerializeObject(record["fragrances"]));
                var perfumer = JsonConvert.DeserializeObject<Perfumer>(JsonConvert.SerializeObject(record["perfumer"]));
                perfumer.CreatedFragrances = fragrances;
                return perfumer;
            });
            if (perfumer is null)
            {
                return NotFound($"Perfumer with id {id} not found!");
            }
            return Ok(perfumer);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("create perfumer")]
    [HttpPost]
    public async Task<IActionResult> AddPerfumer([FromBody]AddPerfumerDto perfumer)
    {
        try
        {
            await using var session = driver.AsyncSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"CREATE (:PERFUMER {name: $name, surname: $surname, country: $country, gender: $gender, image: ''})";
                await tx.RunAsync(query, new
                {
                    name = perfumer.Name,
                    surname = perfumer.Surname,
                    gender = perfumer.Gender,
                    country = perfumer.Country,
                });
            });
            return Ok($"Perfumer {perfumer.Name} {perfumer.Surname} was successfully created!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("update perfumer")]
    [HttpPatch]
    public async Task<IActionResult> UpdatePerfumer([FromBody] UpdatePerfumerDto perfumer)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var perfumerExists = await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (n:PERFUMER)
                              WHERE id(n) = $id
                              SET n.name = $name, n.surname = $surname, n.gender = $gender, n.country = $country
                              RETURN n IS NOT NULL AS perfumerUpdated";
                var result = await tx.RunAsync(query, new
                {
                    id = perfumer.Id,
                    name = perfumer.Name,
                    surname = perfumer.Surname,
                    gender = perfumer.Gender,
                    country = perfumer.Country,
                });
                return await result.SingleAsync(record => record["perfumerUpdated"].As<bool>());
            });
            if (perfumerExists is false)
            {
                return NotFound($"Perfumer with id {perfumer.Id} was not found!");
            }

            return Ok($"Perfumer with id {perfumer.Id} was successfully updated!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("add fragrance to perfumer")]
    [HttpPatch("{perfumerId}/{fragranceId}")]
    public async Task<IActionResult> AddFragranceAToPerfumer(int perfumerId, int fragranceId)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var result = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (n:PERFUMER) WHERE id(n) = $perfumerId
                              OPTIONAL MATCH (m:FRAGRANCE) WHERE id(m) = $fragranceId
                              OPTIONAL MATCH (n) -[r:CREATES]-> (m)
                              RETURN n IS NOT NULL AS perfumerExists,
                                     m IS NOT NULL AS fragranceExists,
                                     r IS NOT NULL AS connectionExists";
                var result = await tx.RunAsync(query, new { perfumerId, fragranceId });
                var record = await result.SingleAsync();
                return (record["perfumerExists"].As<bool>(), record["fragranceExists"].As<bool>(),
                    record["connectionExists"].As<bool>());
            });
            var (perfumerExists, fragranceExists, connectionExists) = result;
            if (perfumerExists is false)
            {
                return NotFound($"Perfumer with id {perfumerId} was not found!");
            }
            if (fragranceExists is false)
            {
                return NotFound($"Fragrance with id {fragranceId} was not found!");
            }

            if (connectionExists)
            {
                return Conflict($"Perfumer with id {perfumerId} is already connected to fragrance with id {fragranceId}!");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"MATCH (n:PERFUMER) WHERE id(n) = $perfumerId
                              MATCH (m:FRAGRANCE) WHERE id(m) = $fragranceId
                              CREATE (n) -[:CREATES]-> (m)";
                await tx.RunAsync(query, new { perfumerId, fragranceId });
            });
            return Ok($"Perfumer with id {perfumerId} is connected to fragrance with id {fragranceId}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete perfumer")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePerfumer(int id)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var perfumerDeleted = await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (n:PERFUMER)
                              WHERE id(n) = $id
                              DETACH DELETE n
                              RETURN n IS NOT NULL AS deleted";
                var result = await tx.RunAsync(query, new { id });
                return await result.SingleAsync(record => record["deleted"].As<bool>());
            });
            if (perfumerDeleted)
            {
                return Ok($"Perfumer with id {id} deleted!");
            }

            return NotFound($"Perfumer with id {id} not found!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}