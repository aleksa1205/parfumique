namespace FragranceRecommendation.Services.UserService;

public interface IUserService
{
    public Task<bool> UserExistsAsync(string username);
    public Task<bool> UserOwnsFragranceAsync(string username, int id);
    public Task<IList<INode>> GetUsersAsync();
    public Task<User?> GetUserAsync(string username);
    public Task AddUserAsync(AddUserDto user);
    public Task UpdateUserAsync(UpdateUserDto user);
    public Task AddFragranceToUserAsync(AddFragranceToUser dto);
    public Task DeleteUserAsync(DeleteUserDto user);
}