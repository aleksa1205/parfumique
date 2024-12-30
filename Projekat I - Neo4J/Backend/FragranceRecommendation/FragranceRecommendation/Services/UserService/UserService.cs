namespace FragranceRecommendation.Services.UserService;

public class UserService(IDriver driver, IConfiguration config) : IUserService
{
    public async Task<bool> UserExistsAsync(string username)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var query = @"OPTIONAL MATCH (n:USER {username: $username}) 
                          RETURN n IS NOT NULL AS exists";
            var result = await tx.RunAsync(query, new { username });
            return (await result.SingleAsync())["exists"].As<bool>();
        });
    }

    public async Task<bool> UserOwnsFragranceAsync(string username, int id)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var query = @"MATCH (n:USER {username: $username})
                          MATCH (f:FRAGRANCE) WHERE id(f) = $id
                          OPTIONAL MATCH (n) -[r:OWNS]-> (f)
                          RETURN r IS NOT NULL AS exists";
            var result = await tx.RunAsync(query, new { username, id });
            return (await result.SingleAsync())["exists"].As<bool>();
        });
    }
    
    public async Task<IList<ReturnUserDto>> GetUsersAsync()
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var result = await tx.RunAsync("MATCH (n:USER) RETURN n");
            var list = new List<ReturnUserDto>();
            await foreach (var record in result)
            {
                list.Add(MyUtils.DeserializeNode<ReturnUserDto>(record["n"].As<INode>()));
            }
            return list;
        });
    }

    public async Task<User?> GetUserAsync(string username)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var query = @"MATCH (n:USER {username: $username})
                          OPTIONAL MATCH (n) -[:OWNS]-> (f:FRAGRANCE)
                          RETURN n AS user, COLLECT(f{.*, id: id(f)}) AS fragrances";
            var result = await tx.RunAsync(query, new { username });
            var record = await result.PeekAsync();
            if (record is null)
                return null;
            
            var fragrances = MyUtils.DeserializeMap<List<Fragrance>>(record["fragrances"]);
            var user = MyUtils.DeserializeNode<User>(record["user"].As<INode>());

            user!.Collection = fragrances!;
            return user;
        });
    }

    public async Task<ReturnUserDto?> GetUserDtoAsync(string username)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var query = @"MATCH (n:USER {username: $username})
                          OPTIONAL MATCH (n) -[:OWNS]-> (f:FRAGRANCE)
                          RETURN n AS user, COLLECT(f{.*, id: id(f)}) AS fragrances";
            var result = await tx.RunAsync(query, new { username });
            var record = await result.PeekAsync();
            if (record is null)
                return null;

            var fragrances = MyUtils.DeserializeMap<List<Fragrance>>(record["fragrances"]);
            var user = MyUtils.DeserializeNode<ReturnUserDto>(record["user"].As<INode>());

            user!.Collection = fragrances!;
            return user;
        });
    }

    public async Task<User?> GetUserWithoutFragrancesAsync(string username)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var query = @"MATCH (n:USER {username: $username})
                          RETURN n AS user";

            var cursor = await tx.RunAsync(query, new { username });
            var record = await cursor.PeekAsync();

            if (record is null)
                return null;

            var user = MyUtils.DeserializeNode<User>(record["user"].As<INode>());
            return user;
        });
    }

    public async Task AddUserAsync(AddUserDto user)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query =
                @"CREATE (:USER {username: $username, password: $password, name: $name, surname: $surname, gender: $gender, image: ''})";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await tx.RunAsync(query,
                new
                {
                    username = user.Username, password = hashedPassword, name = user.Name, surname = user.Surname,
                    gender = user.Gender
                });
        });
    }

    public async Task UpdateUserAsync(string username, string name, string surname, char gender)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"MATCH (n:USER {username: $username})
                          SET n.name = $name, n.surname = $surname, n.gender = $gender";
            await tx.RunAsync(query,
                new { username, name, surname, gender });
        });
    }

    public async Task AddFragranceToUserAsync(string username, int fragranceId)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"MATCH (n:USER {username: $username})
                          MATCH (f:FRAGRANCE) WHERE id(f) = $id
                          CREATE (n) -[:OWNS]-> (f)";
            await tx.RunAsync(query, new
            {
                username, id = fragranceId
            });
        });
    }

    public async Task DeleteUserAsync(string username)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"MATCH(n:USER {username: $username})
                          DETACH DELETE n";
            await tx.RunAsync(query, new { username });
        });
    }
}