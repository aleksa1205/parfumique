namespace FragranceRecommendation.Services.ManufacturerService;

public interface IManufacturerService
{
    public Task<List<Manufacturer>> GetAllManufacturers();
    public Task<bool> ManufacturerExistsAsync(string name);
    public Task<bool> IsFragranceCreatedByManufacturerAsync(int fragranceId, string name);
    public Task<Manufacturer?> GetManufacturer(string name);
    public Task AddManufacturer(string name);
}