namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class ProizvodjacController : ControllerBase
{
    private readonly IDriver _driver;

    public ProizvodjacController()
    {
        _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "12345678"));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("GetAllProizvodjac")]
    public async Task<IActionResult> GetAllProizvodjac()
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var listaProizvodjaca = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync("MATCH (n: PROIZVODJAC) RETURN n");
                var nodes = new List<INode>();
                await foreach(var record in result)
                {
                    var node = record["n"].As<INode>();
                    nodes.Add(node);
                }
                return nodes;
            });
            return Ok(listaProizvodjaca);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)] 
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetProizvodjacByNaziv/{naziv}")]
    public async Task<IActionResult> GetProizvodjacByNaziv(string naziv)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var proizvodjac = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (p: PROIZVODJAC) WHERE p.naziv = $naziv OPTIONAL MATCH (p) -[:PROIZVODI] -> (a: PARFEM) RETURN p, COLLECT(a) AS parfemi";
                var result = await tx.RunAsync(query, new { naziv });
                var record = await result.PeekAsync();
                if (record is null)
                {
                    return null;
                }
                var proizvodjac = new Proizvodjac(naziv);
                var parfemi = record["parfemi"].As<List<INode>>();
                foreach (var node in parfemi)
                {
                    var nazivParfema = node.Properties["naziv"].As<string>();
                    var pol = node.Properties["za"].As<char>();
                    var godinaIzlaska = node.Properties["godina_izlaska"].As<int>();
                    var parfem = new Parfem(nazivParfema, godinaIzlaska, pol);
                    proizvodjac.ListaParfema.Add(parfem);
                }
                return proizvodjac;
            });
            if(proizvodjac is null)
            {
                return NotFound($"Proizvođač {naziv} ne postoji!");
            }
            return Ok(proizvodjac);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("DodajProizvodjaca/{naziv}")]
    public async Task<IActionResult> PostProizvodjac(string naziv)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var exists = false;
            await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (p: PROIZVODJAC) WHERE p.naziv = $naziv RETURN p";
                var result = await tx.RunAsync(query, new { naziv });
                exists = await result.FetchAsync();
            });
            if (exists)
            {
                return Conflict($"Proizvođač {naziv} već postoji!");
            }
            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"CREATE (:PROIZVODJAC {naziv: $naziv})";
                await tx.RunAsync(query, new { naziv });
            });
            return Ok($"Uspešno dodat proivođač {naziv}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("ObrisiProizvodjaca/{naziv}")]
    public async Task<IActionResult> DeleteProizvodjac(string naziv)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var exists = false;
            await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (p: PROIZVODJAC) WHERE p.naziv = $naziv RETURN p";
                var results = await tx.RunAsync(query, new { naziv });
                exists = await results.FetchAsync();
            });
            if (!exists)
            {
                return NotFound($"Proizvođač sa nazivom {naziv} nije pronađen!");
            }
            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"MATCH (p: PROIZVODJAC) WHERE p.naziv = $naziv DETACH DELETE (p)";
                await tx.RunAsync(query, new { naziv });
            });
            return Ok("Uspešno obrisan proizvođač!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
