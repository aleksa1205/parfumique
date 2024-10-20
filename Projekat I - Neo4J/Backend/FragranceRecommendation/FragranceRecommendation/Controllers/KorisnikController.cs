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
                var result = await tx.RunAsync("MATCH (n: KORISNIK)" +
                                                "RETURN n");
                var lista = new List<Korisnik>();
                await foreach (var record in result)
                {
                    var node = record["n"].As<INode>();
                    //slika
                    var ime = node.Properties["ime"].As<string>();
                    var prezime = node.Properties["prezime"].As<string>();
                    var pol = node.Properties["pol"].As<char>();
                    var korisnickoIme = node.Properties["korisnicko_ime"].As<string>();
                    var sifra = node.Properties["sifra"].As<string>();
                    var korisnik = new Korisnik(ime, prezime, pol, korisnickoIme, sifra);
                    lista.Add(korisnik);
                };
                return lista;
            });
            return Ok(listaKorisnika);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
