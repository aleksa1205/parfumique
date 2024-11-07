using System.Runtime.InteropServices.ComTypes;

namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class ProizvodjacController : ControllerBase
{
    private readonly IDriver _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "12345678"));

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all manufacturers as nodes")]
    [HttpGet]
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
    [EndpointSummary("get manufacturer by name")]
    [HttpGet("{naziv}")]
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
    [EndpointSummary("add manufacturer")]
    [HttpPost("{naziv}")]
    public async Task<IActionResult> AddProizvodjac(string naziv)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var proizvodjacExists = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (p: PROIZVODJAC) WHERE p.naziv = $naziv RETURN p";
                var result = await tx.RunAsync(query, new { naziv });
                return await result.PeekAsync() is not null;
            });
            if (proizvodjacExists is false)
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
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("add fragrance to manufacturer")]
    [HttpPatch("{naziv}/{parfem}")]
    public async Task<IActionResult> AddParfemToProizvodjac(string naziv, string parfem)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var result = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (p: PROIZVODJAC {naziv: $naziv}) 
                              OPTIONAL MATCH (n: PARFEM {naziv: $parfem})
                              OPTIONAL MATCH (n) <-[r:PROIZVODI]- (:PROIZVODJAC)
                              RETURN p IS NOT NULL AS proizvodjacExists,
                                    n IS NOT NULL AS parfemExists,
                                    r IS NOT NULL AS connectionExists";
                var checkResult = await tx.RunAsync(query, new { naziv, parfem });
                var record = await checkResult.SingleAsync();
                
                bool proizvodjacExists = record["proizvodjacExists"].As<bool>();
                bool parfemExists = record["parfemExists"].As<bool>();
                bool connectionExists = record["connectionExists"].As<bool>();
                return (proizvodjacExists, parfemExists, connectionExists);
            });

            var (proizvodjacExists, parfemExists, connectionExists) = result;
            if (proizvodjacExists is false)
            {
                return NotFound($"Proizvođač {naziv} nije pronađen!");
            }
            if (parfemExists is false)
            {
                return NotFound($"Parfem {parfem} nije pronađen!");
            }
            if (connectionExists)
            {
                return Conflict($"Parfem {parfem} već ima proizvođača!");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"MATCH (p: PROIZVODJAC {naziv: $naziv}),
                                    (n: PARFEM {naziv: $parfem})
                              CREATE (p) -[:PROIZVODI]-> (n)";
                await tx.RunAsync(query, new { naziv, parfem });
            });
            return Ok($"Uspešno dodat parfem {{parfem}} proizvođaču {naziv}!");
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
    [HttpDelete("{naziv}")]
    public async Task<IActionResult> DeleteProizvodjac(string naziv)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var proizvodjacExists = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (p: PROIZVODJAC) WHERE p.naziv = $naziv RETURN p";
                var results = await tx.RunAsync(query, new { naziv });
                return await results.PeekAsync() is not null;
            });
            if (proizvodjacExists is false)
            {
                return NotFound($"Proizvođač sa nazivom {naziv} nije pronađen!");
            }
            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"MATCH (p: PROIZVODJAC) WHERE p.naziv = $naziv DETACH DELETE (p)";
                await tx.RunAsync(query, new { naziv });
            });
            return Ok($"Uspešno obrisan proizvođač {naziv}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
