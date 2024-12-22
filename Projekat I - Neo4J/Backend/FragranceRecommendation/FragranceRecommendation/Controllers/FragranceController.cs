using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;

namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class  FragranceController(IDriver driver) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all fragrances as nodes")]
    [HttpGet]
    public async Task<IActionResult> GetAllFragrances()
    {
        try
        {
            await using var session = driver.AsyncSession();
            var listOfFragrances = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync("MATCH (n:FRAGRANCE) RETURN n");
                var nodes = new List<INode>();
                await foreach (var record in result)
                {
                    var node = record["n"].As<INode>();
                    nodes.Add(node);
                }

                return nodes;
            });
            return Ok(listOfFragrances);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("get fragrance by id")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFragranceById(int id)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var fragrance = await session.ExecuteReadAsync(async tx =>
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
                {
                    return null;
                }

                var manufacturer =
                    JsonConvert.DeserializeObject<Manufacturer>(Helper.GetJson(record["manufacturer"].As<INode>()));
                var perfumers =
                    JsonConvert.DeserializeObject<List<Perfumer>>(JsonConvert.SerializeObject(record["perfumers"]));
                var topNotes = record["topNotes"].As<List<INode>>()
                    .Select(node => JsonConvert.DeserializeObject<Note>(Helper.GetJson(node))).ToList();
                var middleNotes = record["middleNotes"].As<List<INode>>()
                    .Select(node => JsonConvert.DeserializeObject<Note>(Helper.GetJson(node))).ToList();
                var baseNotes = record["baseNotes"].As<List<INode>>()
                    .Select(node => JsonConvert.DeserializeObject<Note>(Helper.GetJson(node))).ToList();

                var fragrance =
                    JsonConvert.DeserializeObject<Fragrance>(JsonConvert.SerializeObject(record["fragrance"]));
                fragrance!.Manufacturer = manufacturer;
                fragrance.Perfumers = perfumers;
                fragrance.Top = topNotes;
                fragrance.Middle = middleNotes;
                fragrance.Base = baseNotes;
                return fragrance;
            });
            if (fragrance is null)
            {
                return NotFound($"Fragrance with id {id} not found!");
            }

            return Ok(fragrance);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType((StatusCodes.Status200OK))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all fragrances without manufacturer")]
    [HttpGet("without manufacturer")]
    public async Task<IActionResult> GetAllFragrancesWithoutManufacturer()
    {
        try
        {
            await using var session = driver.AsyncSession();
            var list = await session.ExecuteReadAsync(async tx =>
            {
                var result =
                    await tx.RunAsync(
                        "MATCH (n:FRAGRANCE) WHERE NOT (n) <-[:MANUFACTURES]- (:MANUFACTURER)  RETURN n{.*, id: id(n)} AS fragrance");
                var records = await result.ToListAsync();
                return records.Select(record =>
                        JsonConvert.DeserializeObject<Fragrance>(JsonConvert.SerializeObject(record["fragrance"])))
                    .ToList();
            });
            return Ok(list);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("add fragrance")]
    [HttpPost]
    public async Task<IActionResult> AddFragrance([FromBody] AddFragranceDto fragrance)
    {
        try
        {
            await using var session = driver.AsyncSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"CREATE (:FRAGRANCE {name: $name, for: $gender, year: $year})";
                var result = await tx.RunAsync(query, new
                {
                    name = fragrance.Name,
                    gender = fragrance.Gender,
                    year = fragrance.BatchYear
                });
            });
            return Ok($"Fragrance {fragrance.Name} ({fragrance.BatchYear}) for {fragrance.Gender} added!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("update fragrance")]
    [HttpPatch]
    public async Task<IActionResult> UpdateFragrance([FromBody] UpdateFragranceDto fragrance)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var updated = await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (n:FRAGRANCE)
                              WHERE id(n) = $id
                              SET n.name = $name, n.for = $gender, n.year = $year
                              RETURN n IS NOT NULL AS fragranceUpdated";
                var result = await tx.RunAsync(query, new
                {
                    id = fragrance.Id,
                    name = fragrance.Name,
                    gender = fragrance.Gender,
                    year = fragrance.BatchYear
                });
                return await result.SingleAsync(record => record["fragranceUpdated"].As<bool>());
            });
            if (updated)
            {
                return Ok($"Fragrance with id {fragrance.Id} updated!");
            }

            return NotFound($"Fragrance with id {fragrance.Id} not found!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete fragrance")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var deleted = await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (n:FRAGRANCE)
                              WHERE id(n) = $id
                              DETACH DELETE n
                              RETURN n IS NOT NULL AS deleted";
                var result = await tx.RunAsync(query, new { id });
                return await result.SingleAsync(record => record["deleted"].As<bool>());
            });
            if (deleted)
            {
                return Ok($"Fragrance with id {id} deleted!");
            }
            return NotFound($"Fragrance with id {id} doesn't exist!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
