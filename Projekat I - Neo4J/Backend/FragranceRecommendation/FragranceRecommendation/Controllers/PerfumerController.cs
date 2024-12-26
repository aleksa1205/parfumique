namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class PerfumerController(IPerfumerService perfumerService, IFragranceService fragranceService) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EndpointSummary("get all perfumers")]
    [HttpGet]
    public async Task<IActionResult> GetAllPerfumers()
    {
        return Ok(await perfumerService.GetPerfumersAsync());
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("get perfumer by id")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPerfumerById(int id)
    {
        if (id < 0)
            return BadRequest("Perfumer ID must be a positive integer!");
        
        var perfumer = await perfumerService.GetPerfumerAsync(id);
        if (perfumer is null)
            return NotFound($"Perfumer with id {id} not found!");
        
        return Ok(perfumer);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("create perfumer")]
    [HttpPost]
    public async Task<IActionResult> AddPerfumer([FromBody]AddPerfumerDto perfumer)
    {
        var (isValid, errorMessage) = perfumer.Validate();
        if (!isValid)
            return BadRequest(errorMessage);
        
        await perfumerService.AddPerfumerAsync(perfumer);
        return Ok($"Perfumer {perfumer.Name} {perfumer.Surname} {perfumer.Gender} from {perfumer.Country} has been added!");
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("update perfumer")]
    [HttpPatch]
    public async Task<IActionResult> UpdatePerfumer([FromBody] UpdatePerfumerDto perfumer)
    {
        var (isValid, errorMessage) = perfumer.Validate();
        if (!isValid)
            return BadRequest(errorMessage);
        
        if (!await perfumerService.PerfumerExistsAsync(perfumer.Id))
            return NotFound($"Perfumer with id {perfumer.Id} was not found!");
        
        await perfumerService.UpdatePerfumerAsync(perfumer);
        return Ok($"Perfumer with id {perfumer.Id} was successfully updated!");
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("add fragrance to perfumer")]
    [HttpPatch("add-fragrance-to-perfumer")]
    public async Task<IActionResult> AddFragranceAToPerfumer([FromBody] AddFragranceToPerfumer dto)
    {
        var (isValid, errorMessage) = dto.Validate();
        if (!isValid)
            return BadRequest(errorMessage);
        
        if(!await perfumerService.PerfumerExistsAsync(dto.PerfumerId))
            return NotFound($"Perfumer with id {dto.PerfumerId} was not found!");

        if (!await fragranceService.FragranceExistsAsync(dto.FragranceId))
            return NotFound($"Fragrance with id {dto.FragranceId} was not found!");
        
        if(await perfumerService.IsFragranceCreatedByPerfumer(dto.PerfumerId,dto.FragranceId))
            return Conflict($"Perfumer with id {dto.PerfumerId} is already a creator of fragrance with id {dto.FragranceId}!");

        await perfumerService.AddFragranceToPerfumerAsync(dto);
        return Ok(
            $"Perfumer with id {dto.PerfumerId} has been successfully added as a creator for fragrance with id {dto.FragranceId}!");
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete perfumer")]
    [HttpDelete]
    public async Task<IActionResult> DeletePerfumer([FromBody] DeletePerfumerDto perfumer)
    {
        var (isValid, errorMessage) = perfumer.Validate();
        if(!isValid)
            return BadRequest(errorMessage);
        
        if (!await perfumerService.PerfumerExistsAsync(perfumer.Id))
            return NotFound($"Perfumer with id {perfumer.Id} not found!");
        
        await perfumerService.DeletePerfumerAsync(perfumer);
        return Ok($"Perfumer with id {perfumer.Id} deleted!");
    }
}