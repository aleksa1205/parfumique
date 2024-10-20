namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class ParfemController : ControllerBase
{
    private readonly IDriver _driver;

    public ParfemController()
    {
        _driver = GraphDatabase.Driver("1", AuthTokens.Basic("neo4j", "0"));
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
                var result = await tx.RunAsync("MATCH (n: PARFEM)" +
                                               "RETURN n");
                var lista = new List<Parfem>();
                await foreach(var record in result)
                {
                    var node = record["n"].As<INode>();
                    //slika
                    var naziv = node.Properties["naziv"].As<string>();
                    var godina = node.Properties["godina_izlaska"].As<int>();
                    var pol = node.Properties["za"].As<char>();
                    var parfem = new Parfem(naziv, godina, pol);
                    lista.Add(parfem);
                }
                return lista;
            });
            return Ok(listaParfema);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
            throw;
        }
    }
}
