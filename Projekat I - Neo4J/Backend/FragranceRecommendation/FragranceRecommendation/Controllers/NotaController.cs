namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class NotaController : ControllerBase
{
    private readonly IDriver _driver;

    public NotaController()
    {
        _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "12345678"));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("GetAllNota")]
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
    [HttpGet("GetNotaByNaziv/{naziv}")]
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
}
