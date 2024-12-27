namespace FragranceRecommendation.Services.ManufacturerService;

public interface IManufacturerService
{
    public Task<List<Manufacturer>> GetAllManufacturers();
    public Task<bool> ManufacturerExistsAsync(string name);
    public Task<bool> IsFragranceCreatedByManufacturerAsync(int fragranceId, string name);
    public Task<Manufacturer?> GetManufacturerAsync(string name);
    public Task AddManufacturerAsync(string name);
    public Task AddFragranceToManufacturerAsync(int fragranceId, string name);
    public Task DeleteManufacturerAsync(string name);
}