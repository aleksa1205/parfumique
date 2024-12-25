namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class  FragranceController(IFragranceService fragranceService) : ControllerBase
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
        
        if (!await fragranceService.FragranceExistsAsync(id))
            return NotFound($"Fragrance with id {id} not found!");
        
        return Ok(await fragranceService.GetFragranceAsync(id));
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
