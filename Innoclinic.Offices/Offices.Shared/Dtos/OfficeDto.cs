namespace Offices.Shared.Dtos;

/// <summary>
/// Data Transfer Object (DTO) representing an office
/// </summary>
/// <param name="OfficeId">The office Id</param>
/// <param name="Address">The address of the office</param>
/// <param name="RegistryPhoneNumber">The contact phone number of the office registry</param>
/// <param name="PhotoId">The id of the photo associated with the office</param>
/// <param name="IsActive">Indicates whether the office is active or not</param>
public record OfficeDto(Guid OfficeId, string Address, string RegistryPhoneNumber,
	string PhotoId, bool IsActive);
