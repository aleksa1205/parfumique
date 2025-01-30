using FragranceRecommendation.Auth;
using FragranceRecommendation.DTOs.UserDTOs.SelfDTOs;
using Microsoft.AspNetCore.Authorization;

namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService, IFragranceService fragranceService, IConfiguration config) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all users")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            return Ok(await userService.GetUsersAsync());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("get user by username")]
    [HttpGet("{username}")]
    public async Task<IActionResult> GetUser(string username)
    {
        try
        {
            var (isValid, errorMessage) = MyUtils.IsValidString(username, "Username");
            if (!isValid)
                return BadRequest(errorMessage);

            var user = await userService.GetUserDtoAsync(username);
            if (user is null)
            {
                return NotFound($"User {username} doesn't exists!");
            }

            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("create/register user")]
    [HttpPost("register")]
    public async Task<IActionResult> AddUser([FromBody] AddUserDto user)
    {
        try
        {
            if (await userService.UserExistsAsync(user.Username!))
                return Conflict($"User {user.Username} already exists!");

            await userService.AddUserAsync(user);
            return Ok($"User {user.Username} added!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("get fragrances that are owned by the user")]
    [HttpGet("fragrances/{username}/{page}")]
    public async Task<IActionResult> GetUserFragrances(string username, int page)
    {
        try
        {
            if (!await userService.UserExistsAsync(username))
                return NotFound($"User {username} doesn't exist!");

            return Ok(await userService.GetUserFragrancesPaginationAsync(username, page));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointSummary("generate JWT for login credentials")]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        try
        {
            var user = await userService.GetUserAsync(login.Username!);
            if (user is null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
                return Unauthorized("Invalid username or password");

            var token = new JwtProvider(config).Generate(user);
            return Ok(new LoginResponseDto()
            {
                Role = user.Admin ? Roles.Admin : Roles.User,
                Username = user.Username,
                Token = token
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [RequiresRole(Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("update user")]
    [HttpPatch("update-user")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto user)
    {
        try
        {
            if (!await userService.UserExistsAsync(user.Username!))
                return NotFound($"User {user.Username} doesn't exists!");

            await userService.UpdateUserAsync(user.Username!, user.Name!, user.Surname!, user.Gender!.Value);
            return Ok($"User {user.Username} successfully updated!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [RequiresRole(Roles.User)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointSummary("update user that is logged in")]
    [HttpPatch("update-self")]
    public async Task<IActionResult> UpdateSelf([FromBody] UpdateSelfDto userDto)
    {
        try
        {
            var username = HttpContext.User.Identity?.Name;
            if (username is null)
                return Unauthorized();

            if (!await userService.UserExistsAsync(username))
                return NotFound($"User {username} doesn't exists!");

            await userService.UpdateUserAsync(username, userDto.Name!, userDto.Surname!, userDto.Gender!.Value);
            return Ok($"User {username} successfully updated!");
        }
        catch (Exception e)
        {
           return BadRequest(e.Message);
        }
    }

    [Authorize]
    [RequiresRole(Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("add fragrance to user")]
    [HttpPatch("add-fragrance-to-user")]
    public async Task<IActionResult> AddFragranceToUser([FromBody] AddFragranceToUser dto)
    {
        try
        {
            if(!await userService.UserExistsAsync(dto.Username!))
                return NotFound($"User {dto.Username} doesn't exists!");

            if(!await fragranceService.FragranceExistsAsync(dto.FragranceId))
                return NotFound($"Fragrance {dto.FragranceId} doesn't exists!");

            if (await userService.UserOwnsFragranceAsync(dto.Username!, dto.FragranceId))
                return Conflict($"User {dto.Username} already owns fragrance with id {dto.FragranceId}!");

            await userService.AddFragranceToUserAsync(dto.Username!, dto.FragranceId);
            return Ok($"Successfully added fragrance with id {dto.FragranceId} to user {dto.Username}!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [RequiresRole(Roles.User)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("add fragrance to logged in user")]
    [HttpPatch("add-fragrance-to-self")]
    public async Task<IActionResult> AddFragranceToSelf([FromBody] AddFragranceToSelfDto dto)
    {
        try
        {
            var username = HttpContext.User.Identity?.Name;
            if (username is null)
                return Unauthorized();

            if(!await userService.UserExistsAsync(username!))
                return NotFound($"User {username} doesn't exists!");

            if(!await fragranceService.FragranceExistsAsync(dto.FragranceId))
                return NotFound($"Fragrance {dto.FragranceId} doesn't exists!");

            if (await userService.UserOwnsFragranceAsync(username!, dto.FragranceId))
                return Conflict($"User {username} already owns fragrance with id {dto.FragranceId}!");

            await userService.AddFragranceToUserAsync(username, dto.FragranceId);
            return Ok($"Successfully added fragrance with id {dto.FragranceId} to user {username}!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [Authorize]
    [RequiresRole(Roles.User)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete fragrance from logged in user")]
    [HttpPatch("delete-fragrance-from-self")]
    public async Task<IActionResult> DeleteFragranceFromSelf([FromBody] DeleteFragranceFromSelfDto dto)
    {
        try
        {
            var username = HttpContext.User.Identity?.Name;
            if (username is null)
                return Unauthorized();

            if(!await userService.UserExistsAsync(username!))
                return NotFound($"User {username} doesn't exists!");

            if(!await fragranceService.FragranceExistsAsync(dto.FragranceId))
                return NotFound($"Fragrance {dto.FragranceId} doesn't exists!");

            if (!await userService.UserOwnsFragranceAsync(username!, dto.FragranceId))
                return Conflict($"User {username} doesn't owns fragrance with id {dto.FragranceId}!");

            await userService.DeleteFragranceFromUserAsync(username, dto.FragranceId);
            return Ok($"Successfully deleted fragrance with id {dto.FragranceId} from user {username}!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [RequiresRole(Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointSummary("delete user")]
    [HttpDelete("delete-user")]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserDto user)
    {
        try
        {
            if (!await userService.UserExistsAsync(user.Username!))
                return Conflict($"User {user.Username} doesn't exists!");

            await userService.DeleteUserAsync(user.Username!);
            return Ok($"User {user.Username} successfully deleted!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [RequiresRole(Roles.User)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete logged in user")]
    [HttpDelete("delete-self")]
    public async Task<IActionResult> DeleteSelf()
    {
        try
        {
            var username = HttpContext.User.Identity?.Name;
            if (username is null)
                return Unauthorized();

            if (!await userService.UserExistsAsync(username))
                return Conflict($"User {username} doesn't exists!");

            await userService.DeleteUserAsync(username);
            return Ok($"User {username} successfully deleted!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}