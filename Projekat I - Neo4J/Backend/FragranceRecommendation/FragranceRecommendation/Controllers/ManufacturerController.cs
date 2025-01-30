using FragranceRecommendation.Auth;
using FragranceRecommendation.DTOs.ManufacturerDTOs;
using FragranceRecommendation.Services.ManufacturerService;
using Microsoft.AspNetCore.Authorization;

namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class ManufacturerController(
    IManufacturerService manufacturerService,
    IFragranceService fragranceService) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all manufacturers")]
    [HttpGet]
    public async Task<IActionResult> GetAllManufacturers()
    {
        try
        {
            var manufacturers = await manufacturerService.GetAllManufacturers();

            return Ok(manufacturers);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("get manufacturer by name")]
    [HttpGet("{name}")]
    public async Task<IActionResult> GetManufacturer(string name)
    {
        try
        {
            if (!await manufacturerService.ManufacturerExistsAsync(name))
                return NotFound($"Manufacturer {name} does not exist");

            var manufacturer = await manufacturerService.GetManufacturerAsync(name);

            return Ok(manufacturer);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [Authorize]
    [RequiresRole(Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("add manufacturer")]
    [HttpPost]
    public async Task<IActionResult> AddManufacturer([FromBody] AddManufacturerDto manufacturerDto)
    {
        try
        {
            var name = manufacturerDto.Name!;

            if (await manufacturerService.ManufacturerExistsAsync(name))
                return NotFound($"Manufacturer {name} already exists.");

            await manufacturerService.AddManufacturerAsync(name);
            return Ok($"Manufacturer {name} successfully added!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [RequiresRole(Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("add manufacturer to fragrance")]
    [HttpPatch]
    public async Task<IActionResult> AddFragranceToManufacturer([FromBody] AddFragranceToManufacturerDto addFragranceToManufacturerDto)
    {
        try
        {
            var manufacturerName = addFragranceToManufacturerDto.ManufacturerName!;
            var fragranceId = addFragranceToManufacturerDto.FragranceId!;

            if (!await manufacturerService.ManufacturerExistsAsync(manufacturerName))
                return NotFound($"Manufacturer {manufacturerName} doesn't exist!");

            if (!await fragranceService.FragranceExistsAsync(fragranceId))
                return NotFound($"Fragrance with id {fragranceId} doesn't exist!");

            if (await manufacturerService.IsFragranceCreatedByManufacturerAsync(fragranceId, manufacturerName))
                return Conflict($"Manufacturer {manufacturerName} already manufactures fragrance with id {fragranceId}");

            if (await fragranceService.FragranceHasManufacturerAsync(fragranceId))
                return Conflict($"Fragrance with id {fragranceId} already has manufacturer!");

            await manufacturerService.AddFragranceToManufacturerAsync(fragranceId, manufacturerName);

            return Ok($"Successfully added fragrance with the id {fragranceId} to manufacturer {manufacturerName}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [RequiresRole(Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("Update manufacturer")]
    [HttpPatch("update")]
    public async Task<IActionResult> UpdateManufacturer(UpdateManufacturerDto dto)
    {
        try
        {
            var name = dto.Name!;
            var image = dto.Image!;

            if (!await manufacturerService.ManufacturerExistsAsync(name))
                return NotFound($"Manufacturer {name} doesn't exist!");

            await manufacturerService.UpdateManufacturer(name, image);
            return Ok($"Successfully updated fragrance {name}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [RequiresRole(Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete manufacturer")]
    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteManufacturer(string name)
    {
        try
        {
            if (!await manufacturerService.ManufacturerExistsAsync(name))
                return NotFound($"Manufacturer {name} doesn't exist!");

            await manufacturerService.DeleteManufacturerAsync(name);

            return Ok($"Manufacturer {name} successfully deleted!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
