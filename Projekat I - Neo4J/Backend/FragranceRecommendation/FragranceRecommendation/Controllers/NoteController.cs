using FragranceRecommendation.DTOs.NoteDTOs;
using FragranceRecommendation.Services.NoteService;

namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class NoteController(IDriver driver, INoteService noteService) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all notes as nodes")]
    [HttpGet]
    public async Task<IActionResult> GetAllNotes()
    {
        return Ok(await noteService.GetNotesAsync());
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
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("add note")]
    [HttpPost]
    public async Task<IActionResult> AddNote([FromBody] AddNoteDto note)
    {
        var (isValid, errorMessage) = note.Validate();
        if (!isValid)
            return BadRequest(errorMessage);
        
        if (await noteService.NoteExistsAsync(note.Name))
            return Conflict($"Note {note.Name} already exists!");
        
        await noteService.AddNoteAsync(note);
        return Ok($"Note {note.Name} {note.Type} added!");
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("update note")]
    [HttpPatch]
    public async Task<IActionResult> UpdateNote(UpdateNoteDto note)
    {
        var (isValid, errorMessage) = note.Validate();
        if (!isValid)
            return BadRequest(errorMessage);
        
        if(await noteService.NoteExistsAsync(note.Name))
            return NotFound($"Note {note.Name} does not exist!");
        
        await noteService.UpdateNoteAsync(note);
        return Ok($"Note {note.Name} {note.Type} updated!");
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
    [HttpDelete]
    public async Task<IActionResult> DeleteNote([FromBody] DeleteNoteDto note)
    {
        var (isValid, errorMessage) = note.Validate();
        if (!isValid)
            return BadRequest(errorMessage);
        
        if (!await noteService.NoteExistsAsync(note.Name))
            return NotFound($"Note {note.Name} not found!");
        
        await noteService.DeleteNoteAsync(note);
        return Ok($"Note {note.Name} successfully deleted!");
    }
}