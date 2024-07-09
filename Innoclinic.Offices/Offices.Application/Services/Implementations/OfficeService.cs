using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Offices.Application.Contracts.Services.Interfaces;
using Offices.Contracts.Repositories.Interfaces;
using Offices.Domain.Entities;
using Offices.Domain.Exceptions;
using Offices.Shared.Dtos;
using Offices.Shared.Requests;

namespace Offices.Application.Services.Implementations;

/// <summary>
/// Service for managing office operations
/// </summary>
public class OfficeService : IOfficeService
{
	private readonly IOfficeRepository _officeRepository;
	private readonly IMapper _mapper;
	private readonly ILogger<OfficeService> _logger;
	private readonly IValidator<OfficeRequest> _officeValidator;
	private readonly IBlobService _blobService;

	/// <summary>
	/// Initializes a new instance of the OfficeService class
	/// </summary>
	/// <param name="officeRepository">The office repository</param>
	/// <param name="mapper">The mapper</param>
	/// <param name="logger">The logger</param>
	/// <param name="officeValidator">The office request validator</param>
	/// <param name="blobService">The blob service for handling file upload</param>
	public OfficeService(IOfficeRepository officeRepository, IMapper mapper,
		ILogger<OfficeService> logger,
		IValidator<OfficeRequest> officeValidator,
		IBlobService blobService)
	{
		_officeRepository = officeRepository;
		_mapper = mapper;
		_logger = logger;
		_officeValidator = officeValidator;
		_blobService = blobService;
	}

	/// <summary>
	/// Method to add a new office
	/// </summary>
	/// <param name="officeRequest">The office request</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>The added office as OfficeDto</returns>
	/// <exception cref="ValidationException">Thrown when validation fails</exception>
	public async Task<OfficeDto> AddOfficeAsync(OfficeRequest officeRequest, CancellationToken cancellationToken)
	{
		var validationResult = await _officeValidator.ValidateAsync(officeRequest, cancellationToken);
		if (!validationResult.IsValid)
		{
			_logger.LogError("Validation failed: {ValidationErrors}", validationResult.Errors);
			throw new ValidationException(validationResult.Errors);
		}

		var office = _mapper.Map<Office>(officeRequest);
		var officeId = Guid.NewGuid();
		if (officeRequest.Photo is not null)
		{
			var photoStream = officeRequest.Photo.OpenReadStream();
			var photoUri = await _blobService.UploadBlobAsync(photoStream, officeRequest.Photo.ContentType,
				$"{officeId}");
			office.PhotoId = photoUri;
		}

		office.Id = officeId;
		await _officeRepository.AddOfficeAsync(office);
		var addedOffice = _mapper.Map<OfficeDto>(office);

		return addedOffice;
	}

	/// <summary>
	/// Method to delete and office by its Id
	/// </summary>
	/// <param name="officeId">The office Id</param>
	/// <returns>The deleted office as OfficeDto</returns>
	/// <exception cref="NotFoundException">Thrown when the office is not found</exception>
	public async Task<OfficeDto> DeleteOfficeAsync(Guid officeId)
	{
		var office = await _officeRepository.GetOfficeByIdAsync(officeId);
		if (office is null)
		{
			_logger.LogError("There is not any office with that id");
			throw new NotFoundException($"There is no  office with this id: {officeId}");
		}

		if (!string.IsNullOrEmpty(office.PhotoId))
		{
			await _blobService.DeleteBlobAsync(office.PhotoId);
			_logger.LogInformation("The office picture with the id: {id} was successfully deleted", office.PhotoId);
		}

		await _officeRepository.RemoveOfficeAsync(office);

		return _mapper.Map<OfficeDto>(office);
	}

	/// <summary>
	/// Method to get all offices
	/// </summary>
	/// <returns>A list of OfficeDto objects</returns>
	public async Task<List<OfficeDto>> GetAllOfficesAsync()
	{
		var offices = await _officeRepository.GetAllOfficesAsync();

		return _mapper.Map<List<OfficeDto>>(offices);
	}

	/// <summary>
	/// Method to get an office by its Id
	/// </summary>
	/// <param name="officeId">The office Id</param>
	/// <returns></returns>
	/// <exception cref="NotFoundException">Thrown when the office is not found</exception>
	public async Task<OfficeDto> GetOfficeByIdAsync(Guid officeId)
	{
		var office = await _officeRepository.GetOfficeByIdAsync(officeId);
		if (office is null)
		{
			_logger.LogError("There is not any office with that id : {officeId}", officeId);
			throw new NotFoundException($"There is not any office with this id: {officeId}");
		}

		return _mapper.Map<OfficeDto>(office);
	}

	public async Task<string> GetOfficePictureUrlByIdAsync(Guid officeId)
	{
		var office = await _officeRepository.GetOfficeByIdAsync(officeId);
		if (office is null)
		{
			_logger.LogError("There is not any office with that id : {officeId}", officeId);
			throw new NotFoundException($"There is not any office with this id: {officeId}");
		}

		var photoId = office.PhotoId;
		if (string.IsNullOrEmpty(photoId))
		{
			_logger.LogError("The office with id: {officeId} does not have a picture", officeId);
			throw new NotFoundException($"The office with id: {officeId} does not have a picture");
		}

		return await _blobService.GetBlobUrlByIdAsync(officeId.ToString());
	}

	/// <summary>
	/// Method to update and office by its Id
	/// </summary>
	/// <param name="officeId">The office Id</param>
	/// <param name="officeRequest">The office request</param>
	/// <returns>The updated office as OfficeDto</returns>
	/// <exception cref="ValidationException">Thrown when the validation fails</exception>
	/// <exception cref="NotFoundException">Thrown when the office is not found</exception>
	public async Task<OfficeDto> UpdateOfficeAsync(Guid officeId, OfficeRequest officeRequest)
	{
		var validationResult = await _officeValidator.ValidateAsync(officeRequest);
		if (!validationResult.IsValid)
		{
			_logger.LogError("Validation failed: {ValidationErrors}", validationResult.Errors);
			throw new ValidationException(validationResult.Errors);
		}

		var office = await _officeRepository.GetOfficeByIdAsync(officeId);
		if (office is null)
		{
			_logger.LogError("There is not any office to be updated with this id : {officeId}", officeId);
			throw new NotFoundException($"There is not any office with this id: {officeId}");
		}

		if (officeRequest.Photo is not null)
		{
			if (!string.IsNullOrEmpty(office.PhotoId))
			{
				await _blobService.DeleteBlobAsync(office.PhotoId);
			}

			var photoStream = officeRequest.Photo.OpenReadStream();
			var photoUri = await _blobService.UploadBlobAsync(photoStream, officeRequest.Photo.ContentType,
				$"{officeId}_{officeRequest.Photo.FileName}");
			office.PhotoId = photoUri;
		}

		_mapper.Map(officeRequest, office);
		await _officeRepository.UpdateOfficeAsync(officeId, office);

		return _mapper.Map<OfficeDto>(office);
	}

	/// <summary>
	/// Method to update the status of an office by its Id
	/// </summary>
	/// <param name="officeId">The office Id</param>
	/// <param name="isActive">The new status of the office</param>
	/// <returns>The updated office as OfficeDto</returns>
	/// <exception cref="NotFoundException">Thrown when the office is not found</exception>
	public async Task<OfficeDto> UpdateOfficeStatusAsync(Guid officeId, bool isActive)
	{
		var office = await _officeRepository.GetOfficeByIdAsync(officeId);
		if (office is null)
		{
			_logger.LogError("There is not any office with that id : {officeId}", officeId);
			throw new NotFoundException($"There is not any office with this id: {officeId}");
		}

		await _officeRepository.UpdateOfficeStatusAsync(officeId, isActive);

		return _mapper.Map<OfficeDto>(office);
	}
}
