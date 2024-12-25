using FragranceRecommendation.Services.NoteService;

namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class  FragranceController(IFragranceService fragranceService, INoteService noteService) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EndpointSummary("get all fragrances as nodes")]
    [HttpGet]
    public async Task<IActionResult> GetAllFragrances()
    {
        return Ok(await fragranceService.GetFragrancesAsync());
    }

    //will change after test on frontend
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all fragrances as nodes with pagination")]
    [HttpGet("{pageNumber}/{pageSize}")]
    public async Task<IActionResult> GetAllFragrancesWithPagination(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            return BadRequest("Page number has to be greater than 1!");
        if(pageSize < 1)
            return BadRequest("Page size has to be greater than 1!");

        var (skip, totalCount, totalPages, fragrances) =
            await fragranceService.GetFragrancesAsyncPagination(pageNumber, pageSize);
        return Ok(new { skip, totalCount, totalPages, fragrances });
    }
    
    [ProducesResponseType((StatusCodes.Status200OK))]
    [EndpointSummary("get all fragrances without manufacturer")]
    [HttpGet("without-manufacturer")]
    public async Task<IActionResult> GetAllFragrancesWithoutManufacturer()
    {
        return Ok(await fragranceService.GetFragrancesWithouthManufacturerAsync());
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("get fragrance by id")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFragranceById(int id)
    {
        if (id < 0)
            return BadRequest("Fragrance ID must be a positive integer!");

        var fragrance = await fragranceService.GetFragranceAsync(id);
        if (fragrance is null)
            return NotFound($"Fragrance with id {id} not found!");
        
        return Ok(fragrance);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("add fragrance")]
    [HttpPost]
    public async Task<IActionResult> AddFragrance([FromBody] AddFragranceDto fragrance)
    {
        var (isValid, errorMessage) = fragrance.Validate();
        if (!isValid)
            return BadRequest(errorMessage);
        
        await fragranceService.AddFragranceAsync(fragrance);
        return Ok($"Fragrance {fragrance.Name} ({fragrance.BatchYear}) for {fragrance.Gender} successfully added!");
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("update fragrance")]
    [HttpPatch]
    public async Task<IActionResult> UpdateFragrance([FromBody] UpdateFragranceDto fragrance)
    {
        var (isValid, errorMessage) = fragrance.Validate();
        if (!isValid)
            return BadRequest(errorMessage);
        
        if (!await fragranceService.FragranceExistsAsync(fragrance.Id))
            return NotFound($"Fragrance with id {fragrance.Id} not found!");

        await fragranceService.UpdateFragranceAsync(fragrance);
        return Ok($"Fragrance {fragrance.Id} updated!");
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("add notes to fragrance")]
    [HttpPatch("add-notes")]
    public async Task<IActionResult> AddNotesToFragrance([FromBody] NotesToFragranceDto dto)
    {
        var (isValid, errorMessage) = dto.Validate();
        if (!isValid)
            return BadRequest(errorMessage);
        
        if(!await fragranceService.FragranceExistsAsync(dto.Id))
            return NotFound($"Fragrance with id {dto.Id} does not exist!");

        foreach (var note in dto.Notes)
        {
            if (!await noteService.NoteExistsAsync(note.Name))
                return NotFound($"Note {note.Name} not found!");
        }

        await fragranceService.AddNotesToFragrance(dto);
        return Ok($"Added notes to fragrance with id {dto.Id}!");
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete notes from fragrance")]
    [HttpPatch("delete-notes")]
    public async Task<IActionResult> DeleteNotesFromFragrance([FromBody] NotesToFragranceDto dto)
    {
        var (isValid, errorMessage) = dto.Validate();
        if (!isValid)
            return BadRequest(errorMessage);
        
        if(!await fragranceService.FragranceExistsAsync(dto.Id))
            return NotFound($"Fragrance with id {dto.Id} does not exist!");

        foreach (var note in dto.Notes)
        {
            if (!await noteService.NoteExistsAsync(note.Name))
                return NotFound($"Note {note.Name} not found!");
        }

        await fragranceService.DeleteNotesFromFragrance(dto);
        return Ok($"Deleted notes from fragrance with id {dto.Id}!");
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete fragrance")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteFragranceDto fragrance)
    {
        var(isValid, errorMessage) = fragrance.Validate();
        if (!isValid)
            return BadRequest(errorMessage);
        
        if (!await fragranceService.FragranceExistsAsync(fragrance.Id))
            return NotFound($"Fragrance with id {fragrance.Id} not found!");
        
        await fragranceService.DeleteFragranceAsync(fragrance);
        return Ok($"Fragrance with id {fragrance.Id} deleted!");
    }
}
