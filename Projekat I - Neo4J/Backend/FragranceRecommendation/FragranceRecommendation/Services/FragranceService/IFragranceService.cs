namespace FragranceRecommendation.Services.FragranceService;

public interface IFragranceService
{
    public Task<bool> FragranceExistsAsync(int id);
    public Task<IList<INode>> GetFragrancesAsync();
    public Task<(int,int,int,IList<INode>)> GetFragrancesAsyncPagination(int pageNumber, int pageSize);
    public Task<IList<Fragrance>> GetFragrancesWithouthManufacturerAsync();
    public Task<Fragrance?> GetFragranceAsync(int id);
    public Task AddFragranceAsync(AddFragranceDto fragrance);
    public Task UpdateFragranceAsync(UpdateFragranceDto fragrance);
    public Task AddNotesToFragrance(NotesToFragranceDto dto);
    public Task DeleteNotesFromFragrance(NotesToFragranceDto dto);
    public Task DeleteFragranceAsync(DeleteFragranceDto fragrance);
}