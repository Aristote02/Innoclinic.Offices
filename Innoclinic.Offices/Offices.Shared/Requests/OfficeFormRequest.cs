using Microsoft.AspNetCore.Http;

namespace Offices.Shared.Requests;

public class OfficeFormRequest
{
	public required string Address { get; init; }
	public required string RegistryPhoneNumber { get; init; }
	public IFormFile? Photo { get; init; }
	public required bool IsActive { get; init; }
}
