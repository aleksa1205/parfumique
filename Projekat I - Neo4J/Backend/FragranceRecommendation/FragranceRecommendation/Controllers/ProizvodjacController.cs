namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class ProizvodjacController : ControllerBase
{
    private readonly IDriver _driver;

    public ProizvodjacController()
    {
        _driver = GraphDatabase.Driver("1", AuthTokens.Basic("neo4j", "0"));
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
                var result = await tx.RunAsync("MATCH (n: PROIZVODJAC)" +
                                         "RETURN n");
                var lista = new List<Proizvodjac>();
                await foreach(var record in result)
                {
                    var node = record["n"].As<INode>();
                    //slika
                    var naziv = node.Properties["naziv"].As<string>();
                    var proizvodjac = new Proizvodjac(naziv);
                    lista.Add(proizvodjac);
                }
                return lista;
            });
            return Ok(listaProizvodjaca);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
