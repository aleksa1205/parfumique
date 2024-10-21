using FragranceRecommendation.DTOs;
using FragranceRecommendation.Models;

namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class KorisnikController : ControllerBase
{
    private readonly IDriver _driver;

    public KorisnikController()
    {
        _driver = GraphDatabase.Driver("1", AuthTokens.Basic("neo4j", "0"));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("GetAllKorisnik")]
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

    //refactor kasnije
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetKorisnikByUsername/{korisnickoIme}")]
    public async Task<IActionResult> GetKorisnikByUsername(string korisnickoIme)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var korisnik = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (k: KORISNIK) WHERE k.korisnicko_ime = $korisnickoIme OPTIONAL MATCH (k) -[:POSEDUJE] -> (p: PARFEM) RETURN k, p";
                var result = await tx.RunAsync(query, new { korisnickoIme });
                var listaParfema = new List<Parfem>();
                Korisnik k = null;
                await foreach(var record in result)
                {
                    var nodeKorisnik = record["k"].As<INode>();
                    if(nodeKorisnik is null)
                    {
                        return null;
                    }
                    var ime = nodeKorisnik.Properties["ime"].As<string>();
                    var prezime = nodeKorisnik.Properties["prezime"].As<string>();
                    var korisnicko = nodeKorisnik.Properties["korisnicko_ime"].As<string>();
                    var sifra = nodeKorisnik.Properties["sifra"].As<string>();
                    var pol = nodeKorisnik.Properties["pol"].As<char>();
                    if(k is null)
                    {
                        k = new Korisnik(ime, prezime, pol, korisnicko, sifra);
                    }

                    var nodeParfem = record["p"].As<INode>();
                    if(nodeParfem is not null)
                    {
                        var naziv = nodeParfem.Properties["naziv"].As<string>();
                        var godina = nodeParfem.Properties["godina_izlaska"].As<int>();
                        var polZa = nodeParfem.Properties["za"].As<char>();
                        Parfem p = new Parfem(naziv, godina, pol);
                        listaParfema.Add(p);    
                    }
                }
                if(k is not null)
                {
                    k.Kolekcija = listaParfema;
                }
                return k;
            });
            if(korisnik is null)
            {
                return NotFound($"Korisnik sa korisnickim imenom {korisnickoIme} nije pronadjen!");
            }
            return Ok(korisnik);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("DodajKorisnika")]
    public async Task<IActionResult> PostKorisnik(RegisterKorisnikDTO korisnik)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var exists = false;
            await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (k: KORISNIK) WHERE k.korisnicko_ime = $korisnickoIme RETURN k";
                var result = await tx.RunAsync(query, new { korisnickoIme = korisnik.KorisnickoIme });
                exists = await result.FetchAsync();
            });
            if (exists)
            {
                return Conflict($"Korisnik sa korisničkim imenom {korisnik.KorisnickoIme} već postoji!");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"CREATE (n: KORISNIK {ime: $ime, prezime: $prezime, pol: $pol, korisnicko_ime: $korisnicko, sifra: $sifra})";
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
    [HttpDelete("ObrisiKorisnika/{korisnickoIme}")]
    public async Task<IActionResult> DeleteKorisnik(string korisnickoIme)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var exists = false;
            await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (k: KORISNIK) WHERE k.korisnicko_ime = $korisnickoIme RETURN k";
                var result = await tx.RunAsync(query, new { korisnickoIme });
                exists = await result.FetchAsync();
            });

            if(exists is false)
            {
                return NotFound($"Korisnik sa korisničkim imenom {korisnickoIme} ne postoji!");
            }
            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"MATCH (k: KORISNIK) WHERE k.korisnicko_ime = $korisnickoIme DETACH DELETE (k)";
                await tx.RunAsync(query, new { korisnickoIme });
            });
            return Ok($"Uspešno obrisan korisnik!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}