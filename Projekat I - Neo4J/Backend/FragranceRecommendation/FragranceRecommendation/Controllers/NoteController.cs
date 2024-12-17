using Microsoft.AspNetCore.Session;

namespace FragranceRecommendation.Controllers;

[ApiController]
[Route("[controller]")]
public class NoteController(IDriver driver) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("get all notes as nodes")]
    [HttpGet]
    public async Task<IActionResult> GetAllNotes()
    {
        try
        {
            await using var session = driver.AsyncSession();
            var notesList = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync("MATCH (n:NOTE) RETURN n");
                var nodes = new List<INode>();
                await foreach (var record in result)
                {
                    var node = record["n"].As<INode>();
                    nodes.Add(node);
                }
                return nodes;
            });
            return Ok(notesList);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointSummary("add note")]
    [HttpPost("{name}/{type}")]
    public async Task<IActionResult> AddNote(string name, string type)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var exists = await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (n:NOTE {name: $name})
                               CREATE (:NOTE {name: $name, type: $type})
                               RETURN n IS NOT NULL AS exists";
                var result = await tx.RunAsync(query, new { name, type });
                return await result.SingleAsync(record => record["exists"].As<bool>());
            });
            if (exists)
            {
                return Conflict($"Note {name} already exists!");
            }
            return Ok($"Nota {name} added!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("update note")]
    [HttpPatch("{name}/{type}")]
    public async Task<IActionResult> UpdateNote(string name, string type)
    {
        await using var session = driver.AsyncSession();
        var updated = await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"OPTIONAL MATCH (n:NOTE {name: $name})
                              SET n.type = $type
                              RETURN n IS NOT NULL AS updated";
            var result = await tx.RunAsync(query, new { name, type });
            return await result.SingleAsync(record => record["updated"].As<bool>());
        });
        if (updated)
        {
            return Ok($"Note {name} successfully updated!");
        }
        return Conflict($"Note {name} not found!");
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointSummary("delete note")]
    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteNote(string name)
    {
        try
        {
            await using var session = driver.AsyncSession();
            var deleted = await session.ExecuteWriteAsync(async tx =>
            {
                var query = @"OPTIONAL MATCH (n:NOTE {name: $name})
                              DETACH DELETE n
                              RETURN n IS NOT NULL AS deleted";
                var result = await tx.RunAsync(query, new { name });
                return await result.SingleAsync(record => record["deleted"].As<bool>());
            });
            if (deleted)
            {
                return Ok($"Note {name} successfully deleted!");
            }
            return NotFound($"Note {name} not found!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}