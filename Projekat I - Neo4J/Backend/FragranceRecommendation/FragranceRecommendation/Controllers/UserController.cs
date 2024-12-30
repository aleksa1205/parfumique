using FragranceRecommendation.Auth;
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
    [HttpPost]
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

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointSummary("generate JWT for login credentials")]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginDto login)
    {
        try
        {
            var user = await userService.GetUserAsync(login.Username!);
            if (user is null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
                return Unauthorized("Invalid username or password");

            var token = new JwtProvider(config).Generate(user);
            return Ok(token);
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
    [EndpointSummary("update user that is currently logged in")]
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

            if(!await fragranceService.FragranceExistsAsync(dto.Id))
                return NotFound($"Fragrance {dto.Id} doesn't exists!");

            if (await userService.UserOwnsFragranceAsync(dto.Username!, dto.Id))
                return Conflict($"User {dto.Username} already owns fragrance with id {dto.Id}!");

            await userService.AddFragranceToUserAsync(dto);
            return Ok($"Successfully added fragrance with id {dto.Id} to user {dto.Username}!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete user")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserDto user)
    {
        try
        {
            if (!await userService.UserExistsAsync(user.Username!))
                return Conflict($"User {user.Username} doesn't exists!");

            await userService.DeleteUserAsync(user);
            return Ok($"User {user.Username} successfully deleted!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}