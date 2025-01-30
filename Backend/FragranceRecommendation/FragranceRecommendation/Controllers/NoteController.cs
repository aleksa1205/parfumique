namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class NoteController(INoteService noteService) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all notes")]
    [HttpGet]
    public async Task<IActionResult> GetAllNotes()
    {
        try
        {
            return Ok(await noteService.GetNotesAsync());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("get note by name")]
    [HttpGet("{name}")]
    public async Task<IActionResult> GetNoteByName(string name)
    {
        try
        {
            var note = await noteService.GetNoteAsync(name);
            if(note is null)
                return NotFound($"Note {name} not found!");

            return Ok(note);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("add note")]
    [HttpPost]
    public async Task<IActionResult> AddNote([FromBody] AddNoteDto note)
    {
        try
        {
            if (await noteService.NoteExistsAsync(note.Name!))
                return Conflict($"Note {note.Name} already exists!");

            await noteService.AddNoteAsync(note);
            return Ok($"Note {note.Name} {note.Type} added!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("update note")]
    [HttpPatch]
    public async Task<IActionResult> UpdateNote([FromBody] UpdateNoteDto note)
    {
        try
        {
            if(!await noteService.NoteExistsAsync(note.Name!))
                return NotFound($"Note {note.Name} does not exist!");

            await noteService.UpdateNoteAsync(note);
            return Ok($"Note {note.Name} {note.Type} updated!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete note")]
    [HttpDelete]
    public async Task<IActionResult> DeleteNote([FromBody] DeleteNoteDto note)
    {
        try
        {
            if (!await noteService.NoteExistsAsync(note.Name!))
                return NotFound($"Note {note.Name} not found!");

            await noteService.DeleteNoteAsync(note);
            return Ok($"Note {note.Name} successfully deleted!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}