namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class KreatorController : ControllerBase
{
    private readonly IDriver _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "12345678"));

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all perfumers as nodes")]
    [HttpGet]
    public async Task<IActionResult> GetAllKreator()
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var listaKreatora = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync("MATCH (n: KREATOR) RETURN n");
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
    [EndpointSummary("get perfumer by name and surname")]
    [HttpGet("{ime}/{prezime}")]
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
    [EndpointSummary("create perfumer")]
    [HttpPost]
    public async Task<IActionResult> AddKreator([FromBody]DodajKreatoraDTO Kreator)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var kreatorExists = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (k: KREATOR) WHERE k.ime = $ime AND k.prezime = $prezime RETURN k";
                var result = await tx.RunAsync(query, new { ime = Kreator.Ime, prezime = Kreator.Prezime });
                return await result.PeekAsync() is not null;
            });
            if (kreatorExists)
            {
                return Conflict($"Kreator {Kreator.Ime} {Kreator.Prezime} već postoji!");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query = "CREATE (:KREATOR {ime: $ime, prezime: $prezime, drzava: $drzava, godina: $godina})";
                var parameters = new
                {
                    ime = Kreator.Ime,
                    prezime = Kreator.Prezime,
                    drzava = Kreator.Drzava,
                    godina = Kreator.GodinaRodjenja
                };
                await tx.RunAsync(query, parameters);
            });
            return Ok($"Uspešno dodat kreator {Kreator.Ime} {Kreator.Prezime}!");
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
    public async Task<IActionResult> UpdateKreator([FromBody] UpdateKreatorDTO Kreator)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var kreatorExists = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH(k: KREATOR) WHERE k.ime = $ime AND k.prezime = $prezime RETURN k";
                var result = await tx.RunAsync(query, new { ime = Kreator.Ime, prezime = Kreator.Prezime });
                return await result.PeekAsync() is not null;
            });
            if (kreatorExists is false)
            {
                return NotFound($"Kreator {Kreator.Ime} {Kreator.Prezime} nije pronađen!");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"MATCH(k: KREATOR {ime: $ime, prezime: $prezime})
                            SET k.drzava = $drzava, k.godina = $godina";
                await tx.RunAsync(query,
                    new
                    {
                        ime = Kreator.Ime, prezime = Kreator.Prezime, drzava = Kreator.Drzava,
                        godina = Kreator.GodinaRodjenja
                    });
            });
            return Ok($"Kreator {Kreator.Ime} {Kreator.Prezime} je uspešno ažuriran!");
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
    [EndpointSummary("add fragrance to the perfumer")]
    [HttpPatch("{ime}/{prezime}/{parfem}")]
    public async Task<IActionResult> AddParfemToKreator(string ime, string prezime, string parfem)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var result = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (k:KREATOR {ime: $ime, prezime: $prezime})
                                OPTIONAL MATCH (p:PARFEM {naziv: $parfem})
                                OPTIONAL MATCH (p) <-[r:KREIRA]- (:KREATOR)
                            RETURN k IS NOT NULL AS kreatorExists,
                                   p IS NOT NULL AS parfemExists,
                                   r IS NOT NULL AS connectionExists";
                var checkResult = await tx.RunAsync(query, new { ime, prezime, parfem });
                var record = await checkResult.SingleAsync();
                //nepotrebno je signleasync vraca rezultat uvek kakav god da je i record nikad nije null!!!
                // if (record is null)
                // {
                //     return (false, false, false);
                // }

                bool kreatorExists = record["kreatorExists"].As<bool>();
                bool parfemExists = record["parfemExists"].As<bool>();
                bool connectionExists = record["connectionExists"].As<bool>();
                return (kreatorExists, parfemExists, connectionExists);
            });
            
            var (kreatorExists, parfemExists, connectionExists) = result;
            if (kreatorExists is false)
            {
                return NotFound($"Kreator {ime} {prezime} nije pronađen!");
            }
            if (parfemExists is false)
            {
                return NotFound($"Parfem {parfem} nije pronađen!");
            }
            if (connectionExists)
            {
                return Conflict($"Parfem {parfem} već ima kreatora!");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"MATCH (k:KREATOR {ime: $ime, prezime: $prezime}),
                            (p:PARFEM {naziv: $parfem})
                            CREATE (k) -[:KREIRA]-> (p)";
                await tx.RunAsync(query, new { ime, prezime, parfem });
            });
            return Ok($"Uspešno dodat parfem {parfem} kreatoru {ime} {prezime}!");
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
    [HttpDelete("{ime}/{prezime}")]
    public async Task<IActionResult> DeleteKreator(string ime, string prezime)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var kreatorExists = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (k: KREATOR) WHERE k.ime = $ime AND k.prezime = $prezime RETURN k";
                var result = await tx.RunAsync(query, new { ime, prezime });
                return await result.PeekAsync() is not null;
            });
            if(kreatorExists is false)
            {
                return NotFound($"Kreator {ime} {prezime} nije pronađen!");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"MATCH (k: KREATOR) WHERE k.ime = $ime AND k.prezime = $prezime DETACH DELETE (k)";
                await tx.RunAsync(query, new { ime, prezime });
            });
            return Ok($"Uspešno obrisan kreator {ime} {prezime}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}