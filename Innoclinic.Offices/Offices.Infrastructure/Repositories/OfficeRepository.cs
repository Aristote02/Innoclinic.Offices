using MongoDB.Driver;
using Offices.Contracts.Repositories.Interfaces;
using Offices.Domain.Entities;

namespace Offices.Infrastructure.Repositories;

/// <summary>
/// Repository class for Office entities
/// </summary>
public class OfficeRepository : IOfficeRepository
{
	private readonly IMongoDbContext _dbContext;

	/// <summary>
	/// Initializes a new instance of the <see cref="OfficeRepository"/> class
	/// </summary>
	/// <param name="dbContext">The database context</param>
	public OfficeRepository(IMongoDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	/// <summary>
	/// Adds a new Office entity to the database
	/// </summary>
	/// <param name="office">The office entity to add</param>
	/// <returns></returns>
	public async Task AddOfficeAsync(Office office) =>
		await _dbContext.Offices.InsertOneAsync(office);

	/// <summary>
	/// Method to retrieve all Office entities from the database
	/// </summary>
	/// <returns>A list of all Office entities</returns>
	public async Task<List<Office>> GetAllOfficesAsync() =>
		await _dbContext.Offices.Find(o => true).ToListAsync();

	/// <summary>
	/// Method to retrieve an Office entity by its Id
	/// </summary>
	/// <param name="officeId">The office Id</param>
	/// <returns>The office entity, if found</returns>
	public async Task<Office> GetOfficeByIdAsync(Guid officeId) =>
		await _dbContext.Offices.Find(o => o.Id == officeId)
			.FirstOrDefaultAsync();

	/// <summary>
	/// Method to remove an Office entity from the database
	/// </summary>
	/// <param name="office">The office entity to remove</param>
	/// <returns>A task that represents the asynchronous operation</returns>
	public async Task RemoveOfficeAsync(Office office) =>
		await _dbContext.Offices.DeleteOneAsync(o => o.Id == office.Id);

	/// <summary>
	/// Method to update an existing Office entity in the database
	/// </summary>
	/// <param name="officeId">The Id of the office entity</param>
	/// <param name="office">The updated office entity</param>
	/// <returns></returns>
	public async Task UpdateOfficeAsync(Guid officeId, Office office) =>
		await _dbContext.Offices.ReplaceOneAsync(o => o.Id == officeId, office);

	/// <summary>
	/// Method to update the status of an Office entity
	/// </summary>
	/// <param name="officeId">The office Id</param>
	/// <param name="isActive">The new status of the office entity</param>
	/// <returns></returns>
	public async Task UpdateOfficeStatusAsync(Guid officeId, bool isActive) =>
		await _dbContext.Offices.UpdateOneAsync(o => o.Id == officeId,
			Builders<Office>.Update.Set(o => o.IsActive, isActive));
}