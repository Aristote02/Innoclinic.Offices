using AutoMapper;
using Innoclinic.Shared.DTOs.Offices;
using Innoclinic.Shared.Requests.Offices;
using Offices.Domain.Entities;

namespace Offices.Application.Profiles;

/// <summary>
/// AutoMapper profile for mapping between OfficeRequest, 
/// Office, and OfficeDto objects
/// </summary>
public class MappingProfile : Profile
{
	/// <summary>
	/// Configures the mappings
	/// Map Office entity to OfficeDto and back, using a constructor for OfficeDto
	/// </summary>
	public MappingProfile()
	{
		CreateMap<OfficeRequest, Office>();


		CreateMap<Office, OfficeDto>()
			.ConstructUsing(src => new OfficeDto(
				src.Id,
				src.Address,
				src.RegistryPhoneNumber,
				src.PhotoId!,
				src.IsActive)
			).ReverseMap();
	}
}