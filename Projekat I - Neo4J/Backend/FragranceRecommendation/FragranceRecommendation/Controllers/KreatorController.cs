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
                var lista = new List<Kreator>();
                await foreach (var record in result)
                {
                    var node = record["n"].As<INode>();
                    //slika
                    var ime = node.Properties["ime"].As<string>();
                    var prezime = node.Properties["prezime"].As<string>();
                    var drzava = node.Properties["drzava"].As<string>();
                    var godina = node.Properties["godina"].As<int>();
                    var kreator = new Kreator(ime, prezime, drzava, godina);
                    lista.Add(kreator);
                }
                return lista;
            });
            return Ok(listaKreatora);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
