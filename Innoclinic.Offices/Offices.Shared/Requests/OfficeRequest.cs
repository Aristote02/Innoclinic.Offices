using Microsoft.AspNetCore.Http;

namespace Offices.Shared.Requests;

/// <summary>
/// Request model for creating or updating an office.
/// used for receiving office data from client requests
/// </summary>
public class OfficeRequest
{
	/// <summary>
	/// The address of the office as a required field
	/// </summary>
	public required string Address { get; init; }

	/// <summary>
	/// Contact phone number of the office registry
	/// This field is required
	/// </summary>
	public required string RegistryPhoneNumber { get; init; }

	/// <summary>
	/// An optional photo of the office
	/// </summary>
	public IFormFile? Photo { get; init; }

	/// <summary>
	/// Indicates whether the office is active or not
	/// This field is also required
	/// </summary>
	public required bool IsActive { get; init; }
}
