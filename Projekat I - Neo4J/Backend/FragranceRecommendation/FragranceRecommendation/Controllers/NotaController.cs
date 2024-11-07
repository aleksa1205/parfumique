using System.Runtime.InteropServices.ComTypes;

namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class NotaController : ControllerBase
{
    private readonly IDriver _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "12345678"));

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all notes as nodes")]
    [HttpGet]
    public async Task<IActionResult> GetAllNota()
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var listaNota = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync("MATCH (n: NOTA) RETURN n");
                var nodes = new List<INode>();
                await foreach (var record in result)
                {
                    var node = record["n"].As<INode>();
                    nodes.Add(node);
                }
                return nodes;
            });
            return Ok(listaNota);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("get note by name")]
    [HttpGet("{naziv}")]
    public async Task<IActionResult> GetNotaByNaziv(string naziv)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var nota = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (n: NOTA) WHERE n.naziv = $naziv RETURN n";
                var result = await tx.RunAsync(query, new { naziv });
                var record = await result.PeekAsync();
                if (record is null)
                {
                    return null;
                }
                var nodeNota = record["n"].As<INode>();
                var tip = nodeNota.Properties["tip"].As<string>();
                return new Nota(naziv, tip);
            });
            if(nota is null)
            {
                return NotFound($"Nota {naziv} ne postoji!");
            }
            return Ok(nota);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("add nota")]
    [HttpPost]
    public async Task<IActionResult> AddNota(string naziv, string tip)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var notaExists = await session.ExecuteReadAsync(async tx =>
            {
                var query=@"MATCH (n:NOTA) WHERE n.naziv = $naziv RETURN n";
                var result = await tx.RunAsync(query, new { naziv });
                return await result.PeekAsync() is not null;
            });
            if (notaExists)
            {
                return Conflict($"Nota {naziv} već postoji!");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(@"CREATE (n:NOTA {naziv: $naziv, tip: $tip})", new { naziv, tip });
            });
            return Ok($"Nota {naziv} uspešno dodata!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointDescription("delete nota")]
    [HttpDelete]
    public async Task<IActionResult> DeleteNota(string naziv)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var notaExists = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (n:NOTA) WHERE n.naziv = $naziv RETURN n";
                var result = await tx.RunAsync(query, new { naziv });
                return await result.PeekAsync() is not null;
            });
            if (notaExists is false)
            {
                return NotFound($"Nota {naziv} nije pronađena!");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(@"MATCH (n:NOTA {naziv: $naziv}) DETACH DELETE n", new { naziv });
            });
            return Ok($"Nota {naziv} je obrisana!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
