using FragranceRecommendation.Models;

namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class KreatorController : ControllerBase
{
    private readonly IDriver _driver;

    public KreatorController()
    {
        _driver = GraphDatabase.Driver("1", AuthTokens.Basic("neo4j", "0"));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("GetAllKreator")]
    public async Task<IActionResult> GetAllKreator()
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var listaKreatora = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync("MATCH (n: KREATOR)" +
                                                "RETURN n");
                var nodes = new List<INode>();
                await foreach (var record in result)
                {
                    var node = record["n"].As<INode>();
                    nodes.Add(node);
                }
                return nodes;
            });
            return Ok(listaKreatora);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetKreator/{ime}/{prezime}")]
    public async Task<IActionResult> GetKreatorByNameAndSurname(string ime, string prezime)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var kreator = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (k: KREATOR) WHERE k.ime = $ime AND k.prezime = $prezime OPTIONAL MATCH (k) -[:KREIRA] -> (p: PARFEM) RETURN k, COLLECT(p) AS parfemi";
                var result = await tx.RunAsync(query, new { ime, prezime });
                var record = await result.PeekAsync();
                if(record is null)
                {
                    return null;
                }
                
                var nodeKreator = record["k"].As<INode>();
                var drzava = nodeKreator.Properties["drzava"].As<string>();
                var godinaRodjenja = nodeKreator.Properties["godina"].As<int>();
                Kreator kreator = new Kreator(ime, prezime, drzava, godinaRodjenja);

                var parfemi = record["parfemi"].As<List<INode>>();
                foreach (var nodeParfem in parfemi)
                {
                    var naziv = nodeParfem.Properties["naziv"].As<string>();
                    var godina = nodeParfem.Properties["godina_izlaska"].As<int>();
                    var polZa = nodeParfem.Properties["za"].As<char>();
                    Parfem p = new Parfem(naziv, godina, polZa);
                    kreator.ListaParfema.Add(p);
                }
                return kreator;
            });
            if(kreator is null)
            {
                return NotFound($"Kreator {ime} {prezime} nije pronađen!");
            }
            return Ok(kreator);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("DodajKreatora")]
    public async Task<IActionResult> PostKreator(DodajKreatoraDTO kreator)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var exists = false;
            await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (k: KREATOR) WHERE k.ime = $ime AND k.prezime = $prezime RETURN k";
                var result = await tx.RunAsync(query, new { ime = kreator.Ime, prezime = kreator.Prezime });
                exists = await result.FetchAsync();
            });
            if (exists)
            {
                return Conflict($"Kreator {kreator.Ime} {kreator.Prezime} već postoji!");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query = "CREATE (:KREATOR {ime: $ime, prezime: $prezime, drzava: $drzava, godina: $godina})";
                var parameters = new
                {
                    ime = kreator.Ime,
                    prezime = kreator.Prezime,
                    drzava = kreator.Drzava,
                    godina = kreator.GodinaRodjenja
                };
                await tx.RunAsync(query, parameters);
            });
            return Ok($"Uspešno dodat kreator {kreator.Ime} {kreator.Prezime}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("ObrisiKreatora/{ime}/{prezime}")]
    public async Task<IActionResult> DeleteKreator(string ime, string prezime)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var exists = false;
            await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (k: KREATOR) WHERE k.ime = $ime AND k.prezime = $prezime RETURN k";
                var result = await tx.RunAsync(query, new { ime, prezime });
                exists = await result.FetchAsync();
            });
            if(exists is false)
            {
                return NotFound($"Kreator {ime} {prezime} nije pronađen!");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"MATCH (k: KREATOR) WHERE k.ime = $ime AND k.prezime = $prezime DETACH DELETE (k)";
                await tx.RunAsync(query, new { ime, prezime });
            });
            return Ok("Uspešno obrisan kreator!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
