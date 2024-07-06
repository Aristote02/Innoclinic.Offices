using Offices.Domain.Entities;

namespace Offices.Contracts.Repositories.Interfaces;

/// <summary>
/// Interface for office repository, providing methods to perform CRUD operations on Office entities
/// </summary>
public interface IOfficeRepository
{
	/// <summary>
	/// Methods to add a new office
	/// </summary>
	/// <param name="office">The office entity to add</param>
	/// <returns></returns>
	Task AddOfficeAsync(Office office);

	/// <summary>
	/// Updates an existing office
	/// </summary>
	/// <param name="officeId">The Id of the office to update</param>
	/// <param name="office">The updated office entity</param>
	/// <returns></returns>
	Task UpdateOfficeAsync(Guid officeId, Office office);

	/// <summary>
	/// method to update the status of an existing office
	/// </summary>
	/// <param name="officeId">The Id of the office to update</param>
	/// <param name="isActive">The new status of the office</param>
	/// <returns></returns>
	Task UpdateOfficeStatusAsync(Guid officeId, bool isActive);

	/// <summary>
	/// Method to remove an existing office
	/// </summary>
	/// <param name="office">The office entity to remove</param>
	/// <returns></returns>
	Task RemoveOfficeAsync(Office office);

	/// <summary>
	/// Method to retrieves all offices
	/// </summary>
	/// <returns></returns>
	Task<List<Office>> GetAllOfficesAsync();

	/// <summary>
	/// Method to retrieves an office by its Id
	/// </summary>
	/// <param name="officeId"></param>
	/// <returns></returns>
	Task<Office> GetOfficeByIdAsync(Guid officeId);
}
