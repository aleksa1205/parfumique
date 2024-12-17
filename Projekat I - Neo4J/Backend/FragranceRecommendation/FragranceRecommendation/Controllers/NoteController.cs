namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class NoteController(IDriver driver) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all notes as nodes")]
    [HttpGet]
    public async Task<IActionResult> GetAllNotes()
    {
        try
        {
            await using var session = driver.AsyncSession();
            var notesList = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync("MATCH (n:NOTE) RETURN n");
                var nodes = new List<INode>();
                await foreach (var record in result)
                {
                    var node = record["n"].As<INode>();
                    nodes.Add(node);
                }
                return nodes;
            });
            return Ok(notesList);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("get note by id")]
    [HttpGet("{name}")]
    public async Task<IActionResult> GetNoteByName(string name)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var note = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync(@"MATCH (n:NOTE {name: $name}) RETURN n", new { name });
                var record = await result.PeekAsync();
                return record is null
                    ? null
                    : JsonConvert.DeserializeObject<Note>(Helper.GetJson(record["n"].As<INode>()));
            });
            if (note is null)
            {
                return NotFound($"Note with name {name} does not exist!");
            }
            return Ok(note);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("add note")]
    [HttpPost("{name}/{type}")]
    public async Task<IActionResult> AddNote(string name, string type)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var exists = await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (n:NOTE {name: $name})
                              CREATE (:NOTE {name: $name, type: $type})
                              RETURN n IS NOT NULL AS exists";
                var result = await tx.RunAsync(query, new { name, type });
                return await result.SingleAsync(record => record["exists"].As<bool>());
            });
            if (exists)
            {
                return Conflict($"Note {name} already exists!");
            }
            return Ok($"Nota {name} added!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("update note")]
    [HttpPatch("{name}/{type}")]
    public async Task<IActionResult> UpdateNote(string name, string type)
    {
        await using var session = driver.AsyncSession();
        var updated = await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"OPTIONAL MATCH (n:NOTE {name: $name})
                          SET n.type = $type
                          RETURN n IS NOT NULL AS updated";
            var result = await tx.RunAsync(query, new { name, type });
            return await result.SingleAsync(record => record["updated"].As<bool>());
        });
        if (updated)
        {
            return Ok($"Note {name} successfully updated!");
        }
        return Conflict($"Note {name} not found!");
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("add notes to fragrance")]
    [HttpPatch("add-notes/{id}")]
    public async Task<IActionResult> AddNotesToFragrance(int id, [FromBody] IList<NoteDto> notes)
    {
        try
        {
            if (notes.Count == 0)
            {
                return BadRequest("No notes were passed to fragrance!");
            }
            if (notes.Any(note => note.TMB < 0 || note.TMB > 2))
            {
                return BadRequest("TMB for note has to be between 0 and 2");
            }
            await using var session = driver.AsyncSession();
            var fragranceExists = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (n:FRAGRANCE)
                              WHERE id(n) = $id
                              RETURN n IS NOT NULL AS exists";
                var result = await tx.RunAsync(query, new { id });
                return await result.SingleAsync(record => record["exists"].As<bool>());
            });
            if (fragranceExists is false)
            {
                return NotFound($"Fragrance with id {id} not found!");
            }
            var noteNames = notes.Select(n => n.Name).ToList();
            var existingNotes = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (n:NOTE)
                              WHERE n.name IN $noteNames
                              RETURN n.name AS name";
                var result = await tx.RunAsync(query, new { noteNames });
                return await result.ToListAsync(record => record["name"].As<string>());
            });
            var missingNotes = noteNames.Except(existingNotes).ToList();
            if (missingNotes.Any())
            {
                return NotFound($"Notes not found: {string.Join(", ", missingNotes)}");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"MATCH (f:FRAGRANCE) WHERE id(f) = $id
                              UNWIND $notes AS note
                              MATCH (n:NOTE {name: note.Name})
                              FOREACH (_ IN CASE WHEN note.TMB = 0 THEN [1] ELSE [] END |
                                MERGE (f)-[:TOP]->(n))
                              FOREACH (_ IN CASE WHEN note.TMB = 1 THEN [1] ELSE [] END |
                                MERGE (f)-[:MIDDLE]->(n))
                              FOREACH (_ IN CASE WHEN note.TMB = 2 THEN [1] ELSE [] END |
                                MERGE (f)-[:BASE]->(n))";
                await tx.RunAsync(query, new { notes, id });
            });
            return Ok($"Notes successfully added to fragrance with id {id}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete notes from fragrance")]
    [HttpPatch("delete-notes/{id}")]
    public async Task<IActionResult> DeleteNotesFromFragrance(int id, [FromBody]IList<NoteDto> notes)
    {
        try
        {
            if (notes.Count == 0)
            {
                return BadRequest("No notes were passed to fragrance!");
            }
            if (notes.Any(note => note.TMB < 0 || note.TMB > 2))
            {
                return BadRequest("TMB for note has to be between 0 and 2");
            }

            await using var session = driver.AsyncSession();
            var fragranceExists = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (n:FRAGRANCE)
                              WHERE id(n) = $id
                              RETURN n IS NOT NULL AS exists";
                var result = await tx.RunAsync(query, new { id });
                return await result.SingleAsync(record => record["exists"].As<bool>());
            });
            if (fragranceExists is false)
            {
                return NotFound($"Fragrance with id {id} not found!");
            }

            var noteNames = notes.Select(n => n.Name).ToList();
            var existingNotes = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (n:NOTE)
                              WHERE n.name IN $noteNames
                              RETURN n.name AS name";
                var result = await tx.RunAsync(query, new { noteNames });
                return await result.ToListAsync(record => record["name"].As<string>());
            });
            var missingNotes = noteNames.Except(existingNotes).ToList();
            if (missingNotes.Any())
            {
                return NotFound($"Notes not found: {string.Join(", ", missingNotes)}");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"UNWIND $notes AS note
                              MATCH (f:FRAGRANCE) WHERE id(f) = $id
                              MATCH (n:NOTE {name: note.Name})
                              OPTIONAL MATCH (f) -[r:TOP]-> (n) WHERE note.TMB = 0 DELETE r
                              WITH f, n, note
                              OPTIONAL MATCH (f) -[r:MIDDLE]-> (n) WHERE note.TMB = 1 DELETE r
                              WITH f, n, note
                              OPTIONAL MATCH (f) -[r:BASE]-> (n) WHERE note.TMB = 2 DELETE r";
                await tx.RunAsync(query, new { notes, id });
            });
            return Ok($"Notes successfully deleted from fragrance with id {id}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete note")]
    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteNote(string name)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var deleted = await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (n:NOTE {name: $name})
                              DETACH DELETE n
                              RETURN n IS NOT NULL AS deleted";
                var result = await tx.RunAsync(query, new { name });
                return await result.SingleAsync(record => record["deleted"].As<bool>());
            });
            if (deleted)
            {
                return Ok($"Note {name} successfully deleted!");
            }
            return NotFound($"Note {name} not found!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}