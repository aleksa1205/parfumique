using BCrypt.Net;
using FragranceRecommendation.Auth.JWT;
using FragranceRecommendation.DTOs.UserDTOs;
using FragranceRecommendation.Utils;

namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IDriver driver, IConfiguration config) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all users as nodes")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers(){
        try
        {
            await using var session = driver.AsyncSession();
            var userList = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync("MATCH (n:USER) RETURN n");
                var nodes = new List<INode>();
                await foreach(var record in result)
                {
                    var node = record["n"].As<INode>();
                    nodes.Add(node);
                }
                return nodes;
            });
            return Ok(userList);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("get user by username")]
    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        await using var session = driver.AsyncSession();
        var user = await session.ExecuteReadAsync(async tx =>
        {
            var query = @"MATCH (n:USER) 
                          WHERE n.username = $username 
                          OPTIONAL MATCH (n) -[:OWNS]-> (f:FRAGRANCE)
                          RETURN n{.*, id: id(n)} AS user, COLLECT(f{.*, id: id(f)}) AS fragrances";
            var result = await tx.RunAsync(query, new { username });
            var record = await result.PeekAsync();
            if (record is null)
            {
                return null;
            }

            var fragrances =
                JsonConvert.DeserializeObject<List<Fragrance>>(JsonConvert.SerializeObject(record["fragrances"]));
            var user = JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(record["user"]));
            user.Collection = fragrances;
            return user;
        });
        if(user is null)
        {
            return NotFound($"User {username} doesn't exist!");
        }
        return Ok(user);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointSummary("create/register user")]
    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody]AddUserDto user)
    {
        try
        {
            var (isValid, errorMessage) = MyUtils.IsValidString(user.Name, "Name");
            if (!isValid)
                return BadRequest(errorMessage);

            (isValid, errorMessage) = MyUtils.IsValidString(user.Surname, "Surname");
            if (!isValid)
                return BadRequest(errorMessage);

            (isValid, errorMessage) = MyUtils.IsValidString(user.Username, "Surname");
            if (!isValid)
                return BadRequest(errorMessage);

            (isValid, errorMessage) = MyUtils.IsValidPassword(user.Password);
            if (!isValid)
                return BadRequest(errorMessage);

            (isValid, errorMessage) = MyUtils.IsValidGender(user.Gender);
            if (!isValid)
                return BadRequest(errorMessage);


            await using var session = driver.AsyncSession();
            var exists = await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (n:USER {username: $username})
                              CREATE (:USER {username: $username, name: $name, surname: $surname, gender: $gender, password: $password, image: ''})
                              RETURN n IS NOT NULL AS exists";

                var hashedPass = BCrypt.Net.BCrypt.HashPassword(user.Password);

                var result = await tx.RunAsync(query, new
                {
                    username = user.Username,
                    name = user.Name,
                    surname = user.Surname,
                    gender = user.Gender,
                    password = hashedPass
                });

                return await result.SingleAsync(record => record["exists"].As<bool>());
            });
            if (exists)
            {
                return Conflict($"User {user.Username} already exists!");
            }
            return Ok($"User {user.Username} successfully created!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Generate JWT for login credentials")]
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDto login)
    {
        try
        {
            var username  = login.Username;
            var password = login.Password;

            await using var session = driver.AsyncSession();
            var user = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"MATCH (n:USER)
                             WHERE n.username = $username
                             RETURN n";

                var result = await tx.RunAsync(query, new { username });
                var record  = await result.PeekAsync();

                if (record is null)
                    return null;

                var user = JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(record["user"]));
                return user;
            });

            if (user is null)
                return Unauthorized("Invalid username or password");
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                return Unauthorized("Invalid username or password");

            var token = new JwtProvider(config).Generate(user);

            return Ok(token);

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("update user")]
    [HttpPatch]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto user)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var updated = await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (n:USER {username: $username})
                              SET n.name = $name, n.surname = $surname, n.gender = $gender
                              RETURN n IS NOT NULL AS updated";
                var result = await tx.RunAsync(query, new
                {
                    username = user.Username,
                    name = user.Name,
                    surname = user.Surname,
                    gender = user.Gender
                });
                return await result.SingleAsync(record => record["updated"].As<bool>());
            });
            if (updated)
            {
                return Ok($"User {user.Username} successfully updated!");
            }
            return NotFound($"User {user.Username} doesn't exist!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("add fragrance to user")]
    [HttpPatch("{username}/{fragranceId}")]
    public async Task<IActionResult> AddFragranceToUser(string username, int fragranceId)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var result = await session.ExecuteReadAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (u:USER {username: $username})
                              OPTIONAL MATCH (f:FRAGRANCE) WHERE id(f) = $fragranceId
                              OPTIONAL MATCH (u) -[r:OWNS]-> (f)
                              RETURN u IS NOT NULL AS userExists,
                                     f IS NOT NULL AS fragranceExists,
                                     r IS NOT NULL AS connectionExists";
                var result = await tx.RunAsync(query, new { username, fragranceId });
                var record = await result.SingleAsync();
                return (record["userExists"].As<bool>(), record["fragranceExists"].As<bool>(), record["connectionExists"].As<bool>());
            });
            var (userExists,fragranceExists,connectionExists) = result;
            if (userExists is false)
            {
                return NotFound($"User {username} doesn't exist!");
            }
            if (fragranceExists is false)
            {
                return NotFound($"Fragrance with id {fragranceId} doesn't exist!");
            }
            if (connectionExists)
            {
                return Conflict($"User {username} already has fragrance with id {fragranceId}!");
            }

            await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"MATCH (u:USER {username: $username})
                              MATCH (f:FRAGRANCE) WHERE id(f) = $fragranceId
                              CREATE (u) -[:OWNS]-> (f)";
                await tx.RunAsync(query, new { username, fragranceId });
            });
            return Ok($"Fragrance with id {fragranceId} successfully added to user {username}!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete user")]
    [HttpDelete("{username}")]
    public async Task<IActionResult> DeleteUser(string username)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var deleted = await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (n:USER {username: $username}) 
                              DETACH DELETE n
                              RETURN n IS NOT NULL AS deleted";
                var result = await tx.RunAsync(query, new { username });
                return await result.SingleAsync(record => record["deleted"].As<bool>());
            });
            if (deleted)
            {
                return Ok($"User {username} successfully deleted!");
            }
            return NotFound($"User {username} doesn't exist!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}