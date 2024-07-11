using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Offices.Application.Contracts.Services.Interfaces;
using Offices.Contracts.Repositories.Interfaces;
using Offices.Domain.Exceptions;
using Offices.Infrastructure.Configurations;

namespace Offices.Infrastructure.Storage;

/// <summary>
/// Service for handling operations related to Azure Blob Storage
/// </summary>
public class BlobService : IBlobService
{
	private readonly BlobStorageConfigurations _options;
	private readonly ILogger<BlobService> _logger;
	private readonly BlobContainerClient _blobContainerClient;
	private readonly IBlobClientFactory _blobClientFactory;

	/// <summary>
	/// Initializes a new instance of the <see cref="BlobService"/> class
	/// </summary>
	/// <param name="options">The Blob Storage Configurations options</param>
	/// <param name="logger">The logger</param>
	/// <param name="blobServiceClient">The Blob service client</param>
	/// <param name="blobClientFactory">The Blob client factory</param>
	public BlobService(IOptions<BlobStorageConfigurations> options,
		ILogger<BlobService> logger,
		BlobServiceClient blobServiceClient,
		IBlobClientFactory blobClientFactory)
	{
		_options = options.Value;
		_logger = logger;
		_blobContainerClient = blobServiceClient.GetBlobContainerClient(_options.ContainerName);
		_blobContainerClient.CreateIfNotExists();
		_blobClientFactory = blobClientFactory;
	}

	/// <summary>
	/// Method to delete a blob from the storage
	/// </summary>
	/// <param name="blobName">The name of the blob to delete</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns></returns>
	/// <exception cref="NotFoundException">Thrown if the blob does not exist</exception>
	public async Task DeleteBlobAsync(string blobName, CancellationToken cancellationToken = default)
	{
		try
		{
			var blobClient = _blobClientFactory.CreateBlobClient(blobName);
			_logger.LogInformation("Attempting to delete blob with name: {blobName}", blobName);

			await blobClient.DeleteIfExistsAsync();
		}
		catch (RequestFailedException exception) when (exception.Status == 404)
		{
			_logger.LogError("A problem occured while trying to delete the image, Message: {message}", exception.Message);

			throw new NotFoundException($"This image does not exist");
		}
	}

	/// <summary>
	/// Retrieves the URL of a blob by its Id
	/// </summary>
	/// <param name="blobId">The url or id of the blob as a string</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns></returns>
	/// <exception cref="NotFoundException">Thrown when the blob does not exist</exception>
	public async Task<string> GetBlobUrlByIdAsync(string blobId, CancellationToken cancellationToken = default)
	{
		var blobClient = _blobClientFactory.CreateBlobClient($"{_blobContainerClient.Uri}/{blobId}");
		_logger.LogInformation("Uploading blob {blobId} to container {ContainerName}", blobId, _options.ContainerName);

		if (!await blobClient.ExistsAsync())
		{
			_logger.LogError("The file with the url: {url} does not exist", blobId);
			throw new NotFoundException($"The file with the url: {blobId} does not exist");
		}

		return blobClient.Uri.ToString();
	}

	/// <summary>
	/// Method to upload a blob to the storage
	/// </summary>
	/// <param name="content">The content stream of the blob</param>
	/// <param name="contentType">The content type of the blob</param>
	/// <param name="blobName">The name of the Sblob</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>The URL of the uploaded blob</returns>
	public async Task<string> UploadBlobAsync(Stream content, string contentType, string blobName, CancellationToken cancellationToken = default)
	{
		var blobClient = _blobClientFactory.CreateBlobClient($"{_blobContainerClient.Uri}/{blobName}");
		_logger.LogInformation("Uploading blob {blobName} to container {ContainerName}", blobName, _options.ContainerName);

		await blobClient.UploadAsync(content, new BlobUploadOptions
		{
			HttpHeaders = new BlobHttpHeaders
			{
				ContentType = contentType
			}
		});

		_logger.LogInformation("{blobName} uploaded successfully", blobName);

		return blobClient.Uri.ToString();
	}
}