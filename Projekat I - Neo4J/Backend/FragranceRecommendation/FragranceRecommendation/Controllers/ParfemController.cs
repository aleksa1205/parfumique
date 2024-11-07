using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class ParfemController : ControllerBase
{
    private readonly IDriver _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "12345678"));

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all fragrances as nodes")]
    [HttpGet]
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
    [EndpointSummary("get fragrance by name")]
    [HttpGet("{naziv}")]
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("get fragrances that are not associated with a perfumer as nodes")]
    [HttpGet("GetParfemWithouthKreator")]
    public async Task<IActionResult> GetParfemWithouthKreator()
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var listaParfema = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync("MATCH (p:PARFEM) WHERE NOT (p) <-[:KREIRA]- (:KREATOR) RETURN p");
                var nodes = new List<INode>();
                await foreach (var record in result)
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
    [EndpointDescription("get fragrances that are not associated with a manufacturers as nodes")]
    [HttpGet("GetParfemWithouthProizvodjac")]
    public async Task<IActionResult> GetParfemWithouthProizvodjac()
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var listaParfema = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync("MATCH (p:PARFEM) WHERE NOT (p) <-[:PROIZVODI]- (:PROIZVODJAC) RETURN p");
                var nodes = new List<INode>();
                await foreach (var record in result)
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
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("add fragrance")]
    [HttpPost]
    public async Task<IActionResult> AddPafem([FromBody]DodajParfemDTO Parfem) 
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var parfemExists = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (p: PARFEM) WHERE p.naziv = $naziv RETURN p";
                var result = await tx.RunAsync(query, new { naziv = Parfem.Naziv });
                return await result.PeekAsync() is not null;
            });
            if (parfemExists)
            {
                return Conflict($"Parfem {Parfem.Naziv} već postoji!");
            }
            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"CREATE (:PARFEM {naziv: $naziv, godina_izlaska: $godina, za: $pol})";
                var parameters = new
                {
                    naziv = Parfem.Naziv,
                    godina = Parfem.GodinaIzlaska,
                    pol = Parfem.Pol
                };
                await tx.RunAsync(query, parameters);
            });
            return Ok($"Uspešno dodat parfem {Parfem.Naziv}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("update fragrance")]
    [HttpPatch]
    public async Task<IActionResult> UpdateParfem([FromBody] UpdateParfemDTO Parfem)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var parfemExists = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (p: PARFEM) WHERE p.naziv = $parfem RETURN p";
                var result = await tx.RunAsync(query, new { parfem = Parfem.Naziv });
                return await result.PeekAsync() is not null;
            });
            if (parfemExists is false)
            {
                return NotFound($"Parfem {Parfem.Naziv} nije pronađen!");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query = "MATCH (p:PARFEM {naziv: $parfem}) SET p.za = $pol, p.godina_izlaska = $godina";
                var result = await tx.RunAsync(query,
                    new { parfem = Parfem.Naziv, pol = Parfem.Pol, godina = Parfem.GodinaIzlaska });
            });
            return Ok($"Parfem {Parfem.Naziv} uspešno ažuriran!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //trebalo bi da se napravi provera da li su te note već na tom sloju parfema
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("add notes to fragrance")]
    [HttpPatch("{parfem}")]
    public async Task<IActionResult> AddNoteToParfem(string parfem, [FromBody] IList<NotaDTO> Note)
    {
        try
        {
            if (Note.Count == 0)
            {
                return BadRequest("Morate uneti bar jednu notu!");
            }
            await using var session = _driver.AsyncSession();
            var parfemExists = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (p: PARFEM) WHERE p.naziv = $parfem RETURN p";
                var result = await tx.RunAsync(query, new { parfem });
                return await result.PeekAsync() is not null;
            });
            if (parfemExists is false)
            {
                return NotFound($"Parfem {parfem} nije pronađen!");
            }

            foreach (var nota in Note)
            {
                var notaExists = await session.ExecuteReadAsync(async tx =>
                {
                    var query = (@"MATCH (n:NOTA) WHERE n.naziv = $naziv RETURN n");
                    var result = await tx.RunAsync(query, new { naziv = nota.Naziv });
                    return await result.PeekAsync() is not null;
                });
                if (notaExists is false)
                {
                    return NotFound($"Nota {nota.Naziv} nije pronađena!");
                }
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query = new StringBuilder();
                foreach (var nota in Note)
                {
                    query.Clear();
                    query.Append("MATCH (p:PARFEM {naziv: $parfem}) RETURN EXISTS((p)");
                    switch (nota.GDS)
                    {
                        case 0:
                            query.Append("-[:GORNJA]->");
                            break;
                        case 1:
                            query.Append("-[:SREDNJA]->");
                            break;
                        case 2:
                            query.Append("-[:DONJA]->");
                            break;
                        default:
                            throw new ArgumentException("GDS mora biti 1, 2 ili 3!");
                    }
                    query.Append("(:NOTA {naziv: $naziv})) AS linkExists");
                    var result = await tx.RunAsync(query.ToString(), new { parfem, naziv = nota.Naziv });
                    var record = await result.SingleAsync();
                    if (record["linkExists"].As<bool>())
                    {
                        continue;
                    }
                    query.Clear();
                    
                    query.Append(@"MATCH (p: PARFEM {naziv: $parfem}), (n: NOTA {naziv: $naziv}) ");
                    switch (nota.GDS)
                    {
                        case 0:
                            query.Append("CREATE (p) -[:GORNJA]-> (n)");
                            break;
                        case 1:
                            query.Append("CREATE (p) -[:SREDNJA]-> (n)");
                            break;
                        case 2:
                            query.Append("CREATE (p) -[:DONJA]-> (n)");
                            break;
                        default:
                            throw new ArgumentException("GDS mora biti 1, 2 ili 3!");
                    }

                    await tx.RunAsync(query.ToString(), new { parfem, naziv = nota.Naziv });
                }
            });
            return Ok($"Uspešno dodate note parfemu {parfem}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("detach note from fragrance")]
    [HttpDelete]
    public async Task<IActionResult> DeleteNotaFromParfem([FromBody] DetachNotaDTO Nota)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var result = await session.ExecuteReadAsync(async tx =>
            {
                var query = new StringBuilder();
                query.Append(@"OPTIONAL MATCH (p: PARFEM) WHERE p.naziv = $parfem ");
                query.Append(@"OPTIONAL MATCH (n: NOTA) WHERE n.naziv = $nota ");
                switch (Nota.GDS)
                {
                    case 0:
                        query.Append("OPTIONAL MATCH (p) -[r:GORNJA]-> (n) ");
                        break;
                    case 1:
                        query.Append("OPTIONAL MATCH (p) -[r:SREDNJA]-> (n) ");
                        break;
                    case 2:
                        query.Append("OPTIONAL MATCH (p) -[r:DONJA]-> (n) ");
                        break;
                    default:
                        throw new ArgumentException("GDS mora biti 0, 1 ili 2!");
                }

                query.Append(
                    "RETURN p IS NOT NULL AS parfemExists, " +
                    "n IS NOT NULL AS notaExists, " +
                    "r IS NOT NULL AS connectionExists");
                var checkResult = await tx.RunAsync(query.ToString(), new { parfem = Nota.Parfem, nota = Nota.Naziv });
                var record = await checkResult.SingleAsync();
                bool parfemExists = record["parfemExists"].As<bool>();
                bool notaExists = record["notaExists"].As<bool>();
                bool connectionExists = record["connectionExists"].As<bool>();
                return (parfemExists, notaExists, connectionExists);
            });

            var (parfemExists, notaExists, connectionExists) = result;
            if (parfemExists is false)
            {
                return NotFound($"Parfem {Nota.Parfem} nije pronađen!");
            }
            if (notaExists is false)
            {
                return NotFound($"Nota {Nota.Naziv} nije pronađena!");
            }
            if (connectionExists is false)
            {
                return NotFound($"Nota {Nota.Naziv} nije povezana sa parfemom {Nota.Parfem}!");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query = new StringBuilder();
                query.Append(@"MATCH (p:PARFEM {naziv: $parfem}), (n:NOTA {naziv: $naziv})");
                switch (Nota.GDS)
                {
                    case 0:
                        query.Append("MATCH (p) -[r:GORNJA]-> (n)");
                        break;
                    case 1:
                        query.Append("MATCH (p) -[r:SREDNJA]-> (n)");
                        break;
                    case 2:
                        query.Append("MATCH (p) -[r:DONJA]-> (n)");
                        break;
                    default:
                        throw new ArgumentException("GDS mora biti 0, 1 ili 2!");
                }
                query.Append("DETACH DELETE r");
                await tx.RunAsync(query.ToString(), new { parfem = Nota.Parfem, naziv = Nota.Naziv });
            });
            return Ok($"Uspešno raskinuta veza između parfema {Nota.Parfem} i note {Nota.Naziv}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete fragrance")]
    [HttpDelete("{naziv}")]
    public async Task<IActionResult> DeleteParfem(string naziv)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var parfemExists = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (p: PARFEM) WHERE p.naziv = $naziv RETURN p";
                var result = await tx.RunAsync(query, new { naziv });
                return await result.PeekAsync() is not null;
            });
            if (parfemExists is false)
            {
                return NotFound($"Parfem {naziv} ne postoji!");
            }
            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"MATCH (p: PARFEM) WHERE p.naziv = $naziv DETACH DELETE (p)";
                await tx.RunAsync(query, new { naziv });
            });
            return Ok($"Uspešno obrisan parfem {naziv}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
