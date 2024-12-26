namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class NoteController(IDriver driver, INoteService noteService, IFragranceService fragranceService) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all notes")]
    [HttpGet]
    public async Task<IActionResult> GetAllNotes()
    {
        return Ok(await noteService.GetNotesAsync());
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("get note by name")]
    [HttpGet("{name}")]
    public async Task<IActionResult> GetNoteByName(string name)
    {
        var (isValid, errorMessage) = MyUtils.IsValidString(name, "Name");
        if (!isValid)
            return BadRequest(errorMessage);

        var note = await noteService.GetNoteAsync(name);
        if(note is null)
            return NotFound($"Note {name} not found!");
        
        return Ok(note);
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
    public async Task<IActionResult> UpdateNote([FromBody] UpdateNoteDto note)
    {
        var (isValid, errorMessage) = note.Validate();
        if (!isValid)
            return BadRequest(errorMessage);
        
        if(!await noteService.NoteExistsAsync(note.Name))
            return NotFound($"Note {note.Name} does not exist!");
        
        await noteService.UpdateNoteAsync(note);
        return Ok($"Note {note.Name} {note.Type} updated!");
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