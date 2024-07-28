namespace Offices.Shared.Requests;

/// <summary>
/// Request model for creating or updating an office.
/// used for receiving office data from client requests
/// </summary>
public class OfficeRequest : IDisposable
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
	public Stream? Photo { get; set; }

	/// <summary>
	/// Content type of the photo
	/// </summary>
	public string? PhotoContentType { get; set; }

	/// <summary>
	/// Indicates whether the office is active or not
	/// This field is also required
	/// </summary>
	public required bool IsActive { get; init; }

	/// <summary>
	/// Dispose method to realease unmanaged resources
	/// </summary>
	public void Dispose()
	{
		Photo?.Dispose();
	}
}
