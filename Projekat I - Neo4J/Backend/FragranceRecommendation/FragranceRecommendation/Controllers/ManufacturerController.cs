namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class ManufacturerController(IDriver driver) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all manufacturers as nodes")]
    [HttpGet]
    public async Task<IActionResult> GetAllManufacturers()
    {
        try
        {
            await using var session = driver.AsyncSession();
            var manufacturerList = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync("MATCH (n:MANUFACTURER) RETURN n");
                var nodes = new List<INode>();
                await foreach (var record in result)
                {
                    var node = record["n"].As<INode>();
                    nodes.Add(node);
                }
                return nodes;
            });
            return Ok(manufacturerList);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("get manufacturer by name")]
    [HttpGet("{name}")]
    public async Task<IActionResult> GetManufacturer(string name)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var manufacturer = await session.ExecuteReadAsync(async tx =>
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

                var fragrances =
                    JsonConvert.DeserializeObject<List<Fragrance>>(JsonConvert.SerializeObject(record["fragrances"]));
                var manufacturer = JsonConvert.DeserializeObject<Manufacturer>(Helper.GetJson(record["n"].As<INode>()));
                manufacturer.Fragrances = fragrances;
                return manufacturer;
            });
            if (manufacturer is null)
            {
                return NotFound($"Manufacturer {name} not found!");
            }
            return Ok(manufacturer);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("add manufacturer")]
    [HttpPost("{name}")]
    public async Task<IActionResult> AddManufacturer(string name)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var exists = await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (n:MANUFACTURER {name: $name})
                              CREATE (:MANUFACTURER {name: $name, image: ''})
                              RETURN n IS NOT NULL AS exists";
                var result = await tx.RunAsync(query, new { name });
                return await result.SingleAsync(record => record["exists"].As<bool>());
            });
            if (exists)
            {
                return Conflict($"Manufacturer {name} already exists!");
            }
            return Ok($"Manufacturer {name} successfully added!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("add manufacturer to fragrance")]
    [HttpPatch("{manufacturerName}/{fragranceId}")]
    public async Task<IActionResult> AddFragranceToManufacturer(string manufacturerName, int fragranceId)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var result = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (m:MANUFACTURER {name: $manufacturerName})
                              OPTIONAL MATCH (f:FRAGRANCE) WHERE id(f) = $fragranceId
                              OPTIONAL MATCH (f) <-[r:MANUFACTURES]- (:MANUFACTURER)
                              RETURN m IS NOT NULL AS manufacturerExists,
                                     f IS NOT NULL AS fragranceExists,
                                     r IS NOT NULL AS connectionExists";
                var result = await tx.RunAsync(query, new { manufacturerName, fragranceId });
                var record = await result.SingleAsync();
                return (record["manufacturerExists"].As<bool>(), record["fragranceExists"].As<bool>(),
                    record["connectionExists"].As<bool>());
            });
            var (manufacturerExists, fragranceExists, connectionExists) = result;
            if (manufacturerExists is false)
            {
                return NotFound($"Manufacturer {manufacturerName} doesn't exist!");
            }
            if (fragranceExists is false)
            {
                return NotFound($"Fragrance with id {fragranceId} doesn't exist!");
            }
            if (connectionExists)
            {
                return Conflict($"Fragrance with id {fragranceId} already has a manufacturer!");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"MATCH (m:MANUFACTURER {name: $manufacturerName})
                              MATCH (n:FRAGRANCE) WHERE id(n) = $fragranceId
                              CREATE (m) -[:MANUFACTURES]-> (n)";
                await tx.RunAsync(query, new { manufacturerName, fragranceId });
            });
            return Ok($"Successfully added fragrance with the id {fragranceId} to manufacturer {manufacturerName}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete manufacturer")]
    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteManufacturer(string name)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var deleted = await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (n:MANUFACTURER {name: $name})
                              DETACH DELETE n
                              RETURN n IS NOT NULL AS deleted";
                var result = await tx.RunAsync(query, new { name });
                return await result.SingleAsync(record => record["deleted"].As<bool>());
            });
            if (deleted)
            {
                return Ok($"Manufacturer {name} successfully deleted!");
            }
            return NotFound($"Manufacturer {name} not found!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
