using Innoclinic.Shared.Constants;
using Innoclinic.Shared.Requests.Offices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Offices.Application.Contracts.Services.Interfaces;

namespace Offices.Presentation.Controllers;

[Route("api/offices")]
[ApiController]
public class OfficeController : ControllerBase
{
	private readonly IOfficeService _officeService;
	private readonly IBlobService _blobService;
	public OfficeController(IOfficeService officeService, IBlobService blobService)
	{
		_officeService = officeService;
		_blobService = blobService;
	}

	#region GetAllOffices
	/// <summary>
	/// Retrieves all offices
	/// </summary>
	/// <remarks>
	/// Sample request:
	/// 
	///		GET api/offices
	///		[
	///			{
	///				"officeId": "20d9379c-6b01-445b-b93f-29dc63758550"
	///				"address": "Biliechkova  4001",
	///				"registryPhoneNumber": "+44 25 1200 6466",
	///				"photoId": "20d9379c-6b01-445b-b93f-29dc63758550_sws.jpg",
	///				"isActive": true
	///			},
	///			{
	///				"officeId": "9190327b-ebc1-41f7-a127-b2d70275f558",
	///				"address": "Biliechkova  4001",
	///				"registryPhoneNumber": "+44 25 1200 6466",
	///				"photoId": "9190327b-ebc1-41f7-a127-b2d70275f558_architecture.png",
	///				"isActive": false
	///			}
	///		]
	/// </remarks>
	/// <returns>A list of offices</returns>
	/// <response code="200">Returns all the offices successfully</response>
	/// <response code="404">If there are not any single office in the database</response>
	#endregion
	[HttpGet]
	[Authorize(Roles = $"{nameof(UserRole.Patient)},{nameof(UserRole.Doctor)},{nameof(UserRole.Receptionist)}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetAllOfficesAsync()
	{
		var offices = await _officeService.GetAllOfficesAsync();

		return Ok(offices);
	}

	#region GetOfficeById
	/// <summary>
	/// Retrieves an office by its Id
	/// </summary>
	/// <param name="id">The existing office id that must be specified</param>
	/// <remarks>
	/// Sample request:
	/// 
	///		GET api/offices/office/GetOfficeById
	///		{
	///			"officeId": "9190327b-ebc1-41f7-a127-b2d70275f558",
	///		},
	///		{
	///			"officeId": "9190327b-ebc1-41f7-a127-b2d70275f558",
	///			"address": "Biliechkova  4001",
	///			"registryPhoneNumber": "+44 25 1200 6466",
	///			"photoId": "9190327b-ebc1-41f7-a127-b2d70275f558_architecture.png",
	///			"isActive": false
	///		}
	/// </remarks>
	/// <returns>The existing office</returns>
	/// <response code="200">Returns an office successful</response>
	/// <response code="400">If there is not any office with the given id</response>
	#endregion
	[HttpGet("office/{id:guid}", Name = "GetOfficeById")]
	[Authorize(Roles = $"{nameof(UserRole.Patient)},{nameof(UserRole.Doctor)},{nameof(UserRole.Receptionist)}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetOfficeByIdAsync(Guid id)
	{
		var result = await _officeService.GetOfficeByIdAsync(id);

		return Ok(result);
	}

	#region GetOfficePictureByIdAsync
	/// <summary>
	/// Retrieve the office url by its id
	/// </summary>
	/// <param name="id"></param>
	/// <remarks>
	/// Sample request
	/// 
	///		GET api/offices/office/GetOfficePictureById
	///		{
	///			"officeId": "9190327b-ebc1-41f7-a127-b2d70275f558",
	///		},
	///		{
	///			"url" : "http://host.docker.internal:10020/devstoreaccount1/innoclinicimages/9190327b-ebc1-41f7-a127-b2d70275f558"
	///		}
	/// </remarks>
	/// <returns>the office's picture url</returns>
	/// <returns>The officeUrl</returns>
	/// <response code="200">Returns the office picture url successful</response>
	/// <response code="400">If there is not any office with the given id</response>
	#endregion
	[HttpGet("{id:guid}/picture", Name = "GetOfficePictureById")]
	[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Receptionist)}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetOfficePictureByIdAsync(Guid id)
	{
		var pictureUrl = await _officeService.GetOfficePictureUrlByIdAsync(id);

		return Ok(new { Url = pictureUrl });
	}

	#region CreateOfficeAsync
	/// <summary>
	/// Creates a new office
	/// </summary>
	/// <remarks>
	/// Sample request:
	/// 
	///		POST api/offices/office
	///		{
	///			"address": "Biliechkova  4001",
	///			"registryPhoneNumber": "+44 25 1200 6466",
	///			"photoId": "20d9379c-6b01-445b-b93f-29dc63758550_sws.jpg",
	///			"isActive": true
	///		}
	///	
	/// Valid phone numbers format:
	///		- "+375 25-710-33-51"
	///		- "+1 555-123-4567"
	///		- "555-123-4567"
	///		- "(555) 123-4567"
	///		- "+44 20 1234 5678"
	/// </remarks>
	/// <param name="formRequest">The office request object</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A newly created office</returns>
	/// <response code="201">Returns if an office was successfully created</response>
	/// <response code="400">If the requested entity to be created was invalid </response>
	/// <response code="500">If there was an internal server error</response>
	#endregion
	[HttpPost("office")]
	[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Receptionist)}")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> CreateOfficeAsync([FromForm] OfficeFormRequest formRequest, CancellationToken cancellationToken)
	{
		var officeRequest = new OfficeRequest
		{
			Address = formRequest.Address,
			RegistryPhoneNumber = formRequest.RegistryPhoneNumber,
			IsActive = formRequest.IsActive
		};

		if (formRequest.Photo is not null)
		{
			officeRequest.Photo = formRequest.Photo.OpenReadStream();
			officeRequest.PhotoContentType = formRequest.Photo.ContentType;
		}

		var result = await _officeService.AddOfficeAsync(officeRequest, cancellationToken);

		return Created(nameof(CreateOfficeAsync), result);
	}

	#region UpdateOfficeAsync
	/// <summary>
	/// Updates an existing office
	/// </summary>
	/// <param name="officeId">The Id of the office</param>
	/// <param name="formRequest">The office request object</param>
	/// <remarks>
	/// Sample request:
	///	
	///		PUT api/offices/office/officeId
	///		{
	///			"officeId" : "9190327b-ebc1-41f7-a127-b2d70275f558"
	///		},
	///		{
	///			"address": "any address you wanna replace",
	///			"registryPhoneNumber": "+44 25 1200 6466",
	///			"photoId": "20d9379c-6b01-445b-b93f-29dc63758550_sws.jpg",
	///			"isActive": true
	///		}
	///		
	///	Valid phone numbers format:
	///		- "+375 25-710-33-51"
	///		- "+1 555-123-4567"
	///		- "555-123-4567"
	///		- "(555) 123-4567"
	///		- "+44 20 1234 5678"
	/// </remarks>
	/// <response code="204">Returns if the office was successfully updated</response>
	/// <response code="404">If there is no office with the given id</response>
	/// <response code="500">If there was an internal server error</response>
	#endregion
	[HttpPut("office/{officeId:guid}")]
	[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Receptionist)}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> UpdateOfficeAsync(Guid officeId, [FromForm] OfficeFormRequest formRequest)
	{
		var officeRequest = new OfficeRequest
		{
			Address = formRequest.Address,
			RegistryPhoneNumber = formRequest.RegistryPhoneNumber,
			IsActive = formRequest.IsActive
		};

		if (formRequest.Photo is not null)
		{
			officeRequest.Photo = formRequest.Photo.OpenReadStream();
			officeRequest.PhotoContentType = formRequest.Photo.ContentType;
		}

		await _officeService.UpdateOfficeAsync(officeId, officeRequest);

		return NoContent();
	}

	#region DeleteOfficeAsync
	/// <summary>
	/// Deletes an office
	/// </summary>
	/// <param name="officeId">The Id of the office</param>
	/// <remarks>
	/// 
	/// Sample request:
	///	
	///		DELETE api/offices/office/officeId
	///		{
	///			"officeId" : "9190327b-ebc1-41f7-a127-b2d70275f558"
	///		}
	/// </remarks>
	/// <returns></returns>
	/// <response code="204">Returns if the office was successfully deleted</response>
	/// <response code="404">If there is no office with the given id</response>
	#endregion
	[HttpDelete("office/{officeId:guid}")]
	[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Receptionist)}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteOfficeAsync(Guid officeId)
	{
		await _officeService.DeleteOfficeAsync(officeId);

		return NoContent();
	}
}