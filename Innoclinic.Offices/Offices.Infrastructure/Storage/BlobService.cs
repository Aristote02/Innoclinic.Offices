using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Offices.Application.Contracts.Services.Interfaces;
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

	/// <summary>
	/// Initializes a new instance of the <see cref="BlobService"/> class
	/// </summary>
	/// <param name="options">The Blob Storage Configurations options</param>
	/// <param name="logger">The logger</param>
	/// <param name="blobServiceClient">The Blob service client</param>
	public BlobService(IOptions<BlobStorageConfigurations> options,
		ILogger<BlobService> logger,
		BlobServiceClient blobServiceClient)
	{
		_options = options.Value;
		_logger = logger;
		_blobContainerClient = blobServiceClient.GetBlobContainerClient(_options.ContainerName);
		_blobContainerClient.CreateIfNotExists();
	}

	/// <summary>
	/// Method to delete a blob from the storage
	/// </summary>
	/// <param name="blobName">The name of the blob to delete</param>
	/// <returns></returns>
	/// <exception cref="NotFoundException">Thrown if the blob does not exist</exception>
	public async Task DeleteBlobAsync(string blobName)
	{
		try
		{
			var containerClient = new BlobClient(new Uri(blobName), new
				StorageSharedKeyCredential(_options.AccountName, _options.PrimaryKey));
			_logger.LogInformation("Attempting to delete blob with name: {blobName}", blobName);

			await containerClient.DeleteIfExistsAsync();
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
	/// <returns></returns>
	/// <exception cref="NotFoundException">Thrown when the blob does not exist</exception>
	public async Task<string> GetBlobUrlByIdAsync(string blobId)
	{
		var blobClient = new BlobClient(new Uri($"{_blobContainerClient.Uri.ToString()}/{blobId}"),
				new StorageSharedKeyCredential(_options.AccountName, _options.PrimaryKey));
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
	/// <param name="blobName">The name of the blob</param>
	/// <returns>The URL of the uploaded blob</returns>
	public async Task<string> UploadBlobAsync(Stream content, string contentType, string blobName)
	{
		var blobClient = new BlobClient(new Uri($"{_blobContainerClient.Uri.ToString()}/{blobName}"),
				new StorageSharedKeyCredential(_options.AccountName, _options.PrimaryKey));
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