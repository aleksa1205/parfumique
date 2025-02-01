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
                          RETURN DISTINCT(r) IS NOT NULL AS exists";
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

    public async Task<PaginationInfiniteResponseDto> GetUserFragrancesPaginationAsync(string username, int page)
    {
        await using var session = driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            int limit = 8;
            int skip = (page - 1) * limit;
            var query = @"MATCH (n:USER {username: $username})
                          OPTIONAL MATCH (n) -[:OWNS]-> (f:FRAGRANCE)
                          RETURN f{.*, id: id(f)}
                          SKIP $skip
                          LIMIT $limit";
            var result = await tx.RunAsync(query, new { username, skip, limit });
            var fragrances = new List<Fragrance>();
            await foreach (var record in result)
            {
                fragrances.Add(MyUtils.DeserializeMap<Fragrance>(record["f"]));
            }
            return new PaginationInfiniteResponseDto(fragrances, page, fragrances.Count == limit);
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
                @"CREATE (:USER {username: $username, password: $password, name: $name, surname: $surname, gender: $gender, image: '', admin: false})";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await tx.RunAsync(query,
                new
                {
                    username = user.Username, password = hashedPassword, name = user.Name, surname = user.Surname,
                    gender = user.Gender
                });
        });
    }

    public async Task UpdateUserAsync(UpdateUserDto user)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var updates = new List<string>();
            var parameters = new Dictionary<string, object> { { "username", user.Username! } };

            if (!string.IsNullOrEmpty(user.Name))
            {
                updates.Add("n.name = $name");
                parameters["name"] = user.Name;
            }

            if (!string.IsNullOrEmpty(user.Surname))
            {
                updates.Add("n.surname = $surname");
                parameters["surname"] = user.Surname;
            }

            if (user.Gender is not null)
            {
                updates.Add("n.gender = $gender");
                parameters["gender"] = user.Gender;
            }

            var query = $"MATCH (n:USER) WHERE n.username = $username SET {string.Join(", ", updates)}";
            await tx.RunAsync(query, parameters);
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

    public async Task DeleteFragranceFromUserAsync(string username, int fragranceId)
    {
        await using var session = driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            var query = @"MATCH (n:USER {username: $username})
                          MATCH (f:FRAGRANCE) WHERE id(f) = $id
                          MATCH (n) -[r:OWNS]-> (f)
                          DELETE r";
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