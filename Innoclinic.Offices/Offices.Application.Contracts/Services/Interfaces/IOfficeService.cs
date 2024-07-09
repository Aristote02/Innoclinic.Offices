using Offices.Shared.Dtos;
using Offices.Shared.Requests;

namespace Offices.Application.Contracts.Services.Interfaces;

/// <summary>
/// Interface for Office Service operations
/// It Provides methods to manage office data such as retrieving, adding, updating, and deleting office records
/// </summary>
public interface IOfficeService
{
	/// <summary>
	/// Method to retrieve all offices
	/// </summary>
	/// <returns>List of OfficeDto objects representing all offices</returns>
	Task<List<OfficeDto>> GetAllOfficesAsync();

	/// <summary>
	/// Method to retrieve office by its Id
	/// </summary>
	/// <param name="officeId">The ID of the office to retrieve</param>
	/// <returns>An OfficeDto object representing the office</returns>
	Task<OfficeDto> GetOfficeByIdAsync(Guid officeId);

	/// <summary>
	/// Method to retrieve the office's picture by its Id
	/// </summary>
	/// <param name="officeId">The office Id</param>
	/// <returns></returns>
	Task<string> GetOfficePictureUrlByIdAsync(Guid officeId);

    /// <summary>
    /// Method to add a new office
    /// </summary>
    /// <param name="officeRequest">The request object containing the details of the office to add</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation</param>
    /// <returns>An OfficeDto object representing the added office</returns>
    Task<OfficeDto> AddOfficeAsync(OfficeRequest officeRequest, CancellationToken cancellationToken);

	/// <summary>
	/// Method to update the existing office
	/// </summary>
	/// <param name="officeId">The Id of the office to update</param>
	/// <param name="officeRequest">The request object containing the updated details of the office</param>
	/// <returns>an OfficeDto object representing the updated office</returns>
	Task<OfficeDto> UpdateOfficeAsync(Guid officeId, OfficeRequest officeRequest);

	/// <summary>
	/// Method to update the status of an office
	/// </summary>
	/// <param name="officeId">The Id of the office to update</param>
	/// <param name="isActive">The new status of the office (active or inactive)</param>
	/// <returns></returns>
	Task<OfficeDto> UpdateOfficeStatusAsync(Guid officeId, bool isActive);

	/// <summary>
	/// Method to deletes an office by its Id
	/// </summary>
	/// <param name="officeId">The Id of the office to delete</param>
	/// <returns>Returns an OfficeDto object representing the deleted office</returns>
	Task<OfficeDto> DeleteOfficeAsync(Guid officeId);
}
