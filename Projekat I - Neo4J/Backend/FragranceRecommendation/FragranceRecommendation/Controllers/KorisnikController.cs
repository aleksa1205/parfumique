namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class KorisnikController : ControllerBase
{
    private readonly IDriver _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "12345678"));

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all users as nodes")]
    [HttpGet]
    public async Task<IActionResult> GetAllKorisnik(){
        try
        {
            await using var session = _driver.AsyncSession();
            var listaKorisnika = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync("MATCH (n: KORISNIK) RETURN n");
                var nodes = new List<INode>();
                await foreach(var record in result)
                {
                    var node = record["n"].As<INode>();
                    nodes.Add(node);
                }
                return nodes;
            });
            return Ok(listaKorisnika);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("get user by username")]
    [HttpGet("{korisnickoIme}")]
    public async Task<IActionResult> GetKorisnikByUsername(string korisnickoIme)
    {
        await using var session = _driver.AsyncSession();
        var korisnik = await session.ExecuteReadAsync(async tx =>
        {
            var query = @"MATCH (k: KORISNIK) WHERE k.korisnicko_ime = $korisnickoIme OPTIONAL MATCH (k) -[:POSEDUJE] -> (p: PARFEM) RETURN k, COLLECT(p) AS parfemi";
            var result = await tx.RunAsync(query, new { korisnickoIme });
            var record = await result.PeekAsync();
            if (record is null)
            {
                return null;
            }

            var nodeKorisnik = record["k"].As<INode>();
            var ime = nodeKorisnik.Properties["ime"].As<string>();
            var prezime = nodeKorisnik.Properties["prezime"].As<string>();
            var korisnicko = nodeKorisnik.Properties["korisnicko_ime"].As<string>();
            var sifra = nodeKorisnik.Properties["sifra"].As<string>();
            var pol = nodeKorisnik.Properties["pol"].As<char>();
            Korisnik korisnik = new Korisnik(ime, prezime, pol, korisnicko, sifra);

            var parfemi = record["parfemi"].As<List<INode>>();
            foreach (var nodeParfem in parfemi)
            {
                var naziv = nodeParfem.Properties["naziv"].As<string>();
                var godina = nodeParfem.Properties["godina_izlaska"].As<int>();
                var polZa = nodeParfem.Properties["za"].As<char>();
                Parfem p = new Parfem(naziv, godina, polZa);
                korisnik.KolekcijaParfema.Add(p);
            }
            return korisnik;
        });
        if(korisnik is null)
        {
            return NotFound($"Korisnik {korisnickoIme} ne postoji!");
        }
        return Ok(korisnik);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("create user")]
    [HttpPost]
    public async Task<IActionResult> AddKorisnik(RegisterKorisnikDTO korisnik)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var korisnikExists = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (k: KORISNIK) WHERE k.korisnicko_ime = $korisnickoIme RETURN k";
                var result = await tx.RunAsync(query, new { korisnickoIme = korisnik.KorisnickoIme });
                return await result.PeekAsync() is not null;
            });
            if (korisnikExists)
            {
                return Conflict($"Korisnik {korisnik.KorisnickoIme} već postoji!");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query =
                    @"CREATE (:KORISNIK {ime: $ime, prezime: $prezime, pol: $pol, korisnicko_ime: $korisnicko, sifra: $sifra})";
                var parameters = new
                {
                    ime = korisnik.Ime,
                    prezime = korisnik.Prezime,
                    pol = korisnik.Pol,
                    korisnicko = korisnik.KorisnickoIme,
                    sifra = korisnik.Sifra
                };
                await tx.RunAsync(query, parameters);
            });
            return Ok($"Uspešno dodat korisnik {korisnik.KorisnickoIme}!");
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
    [EndpointSummary("add fragrance to users collection")]
    [HttpPatch("{korisnickoIme}/{parfem}")]
    public async Task<IActionResult> AddParfemToKorisik(string korisnickoIme, string parfem)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var result = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (k:KORISNIK {korisnicko_ime: $korisnickoIme}) 
                              OPTIONAL MATCH(p: PARFEM {naziv: $parfem}) 
                              OPTIONAL MATCH(k) -[r:POSEDUJE]-> (p)
                            RETURN k IS NOT NULL AS korisnikExists,
                                   p IS NOT NULL AS parfemExists,
                                   r IS NOT NULL AS connectionExists";
                var checkResult = await tx.RunAsync(query, new { korisnickoIme, parfem });
                var record = await checkResult.SingleAsync();
                // if (record is null)
                // {
                //     return (false, false, false);
                // }
                bool korisnikExists = record["korisnikExists"].As<bool>();
                bool parfemExists = record["parfemExists"].As<bool>();
                bool connectionExists = record["connectionExists"].As<bool>();
                return (korisnikExists, parfemExists, connectionExists);
            });
            
            var (userExists, fragranceExists, connectionExists) = result;
            if (userExists is false)
            {
                return NotFound($"Korisnik {korisnickoIme} ne postoji!");
            }
            if (fragranceExists is false)
            {
                return NotFound($"Parfem {parfem} ne postoji!");
            }
            if (connectionExists)
            {
                return Conflict($"Korisnik {korisnickoIme} već poseduje parfem {parfem}!");
            }
            
            await session.ExecuteWriteAsync(async tx =>
            {
                var query =
                    @"MATCH(k:KORISNIK {korisnicko_ime: $korisnickoIme}), 
                    (p:PARFEM {naziv: $parfem}) 
                    CREATE (k) -[:POSEDUJE]-> (p);";
                await tx.RunAsync(query, new { korisnickoIme, parfem });
            });
            return Ok($"Uspešno dodat parfem {parfem} korisniku {korisnickoIme}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete user")]
    [HttpDelete("{korisnickoIme}")]
    public async Task<IActionResult> DeleteKorisnik(string korisnickoIme)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var korisnikExists = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (k: KORISNIK) WHERE k.korisnicko_ime = $korisnickoIme RETURN k";
                var result = await tx.RunAsync(query, new { korisnickoIme });
                return await result.PeekAsync() is not null;
            });

            if(korisnikExists is false)
            {
                return NotFound($"Korisnik {korisnickoIme} ne postoji!");
            }
            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"MATCH (k: KORISNIK) WHERE k.korisnicko_ime = $korisnickoIme DETACH DELETE (k)";
                await tx.RunAsync(query, new { korisnickoIme });
            });
            return Ok($"Uspešno obrisan korisnik {korisnickoIme}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}