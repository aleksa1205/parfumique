namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class ParfemController : ControllerBase
{
    private readonly IDriver _driver;

    public ParfemController()
    {
        _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "12345678"));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("GetAllParfem")]
    public async Task<IActionResult> GetAllParfem()
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var listaParfema = await session.ExecuteReadAsync(async tx =>
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
            return Ok(listaParfema);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetParfemByNaziv/{naziv}")]
    public async Task<IActionResult> GetParfemByNaziv(string naziv)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var parfem = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (p: PARFEM) WHERE p.naziv = $naziv " +
                            "OPTIONAL MATCH (p) -[:PRIPADA] -> (a: PROIZVODJAC) " +
                            "OPTIONAL MATCH (p) -[:GORNJA] -> (g: NOTA) " +
                            "OPTIONAL MATCH (p) -[:SREDNJA] -> (s: NOTA) " +
                            "OPTIONAL MATCH (p) -[:DONJA] -> (d: NOTA)" +
                            "RETURN p, a, COLLECT(g) as gornjeNote, COLLECT(s) as srednjeNote, COLLECT(d) as donjeNote";
                var result = await tx.RunAsync(query, new { naziv });
                var record = await result.PeekAsync();
                if (record is null)
                {
                    return null;
                }

                var nodeParfem = record["p"].As<INode>();
                var godinaIzlaska = nodeParfem.Properties["godina_izlaska"].As<int>();
                var za = nodeParfem.Properties["za"].As<char>();
                Parfem parfem = new Parfem(naziv, godinaIzlaska, za);

                var nodeProizvodjac = record["a"].As<INode>();
                if (nodeProizvodjac is not null)
                {
                    Proizvodjac proizvodjac = new Proizvodjac(nodeProizvodjac.Properties["naziv"].As<string>());
                    parfem.Proizvodjac = proizvodjac;
                }

                var gornjeNote = record["gornjeNote"].As<List<INode>>();
                foreach (var nota in gornjeNote)
                {
                    var naziv = nota.Properties["naziv"].As<string>();
                    var tip = nota.Properties["tip"].As<string>();
                    var gornjaNota = new Nota(naziv, tip);
                    parfem.Gornje.Add(gornjaNota);
                }

                var srednjeNote = record["srednjeNote"].As<List<INode>>();
                foreach (var nota in srednjeNote)
                {
                    var naziv = nota.Properties["naziv"].As<string>();
                    var tip = nota.Properties["tip"].As<string>();
                    var srednjaNota = new Nota(naziv, tip);
                    parfem.Srednje.Add(srednjaNota);
                }

                var donjeNote = record["donjeNote"].As<List<INode>>();
                foreach (var nota in gornjeNote)
                {
                    var naziv = nota.Properties["naziv"].As<string>();
                    var tip = nota.Properties["tip"].As<string>();
                    var donjaNota = new Nota(naziv, tip);
                    parfem.Donje.Add(donjaNota);
                }
                return parfem;
            });
            if (parfem is null)
            {
                return NotFound($"Parfem {naziv} nije pronađen!");
            }
            return Ok(parfem);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("DodajParfem")]
    public async Task<IActionResult> PostParfem(DodajParfemDTO parfem) 
    {
        try
        {
            await using var session = _driver.AsyncSession();
            bool exists = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (p: PARFEM) WHERE p.naziv = $naziv RETURN p";
                var result = await tx.RunAsync(query, new { naziv = parfem.Naziv });
                return await result.FetchAsync();
            });
            if (exists)
            {
                return Conflict($"Parfem {parfem.Naziv} već postoji!");
            }
            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"CREATE (:PARFEM {naziv: $naziv, godina_izlaska: $godina, za: $pol})";
                var parameters = new
                {
                    naziv = parfem.Naziv,
                    godina = parfem.GodinaIzlaska,
                    pol = parfem.Pol
                };
                await tx.RunAsync(query, parameters);
            });
            return Ok("Uspešno dodat parfem!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("ObrisiParfem/{naziv}")]
    public async Task<IActionResult> DeleteParfem(string naziv)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            bool exists = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (p: PARFEM) WHERE p.naziv = $naziv RETURN p";
                var result = await tx.RunAsync(query, new { naziv });
                return await result.FetchAsync();
            });
            if (!exists)
            {
                return NotFound($"Parfem {naziv} ne postoji!");
            }
            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"MATCH (p: PARFEM) WHERE p.naziv = $naziv DETACH DELETE (p)";
                await tx.RunAsync(query, new { naziv });
            });
            return Ok("Uspešno obrisan parfem!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
