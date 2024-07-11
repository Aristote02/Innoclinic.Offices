namespace Offices.Application.Contracts.Services.Interfaces;

/// <summary>
/// Interface for Blob Service operations that provides methods to interact with blob storage,
/// such as uploading, deleting, and retrieving blobs
/// </summary>
public interface IBlobService
{
	/// <summary>
	/// Uploads a blob to the storage
	/// </summary>
	/// <param name="content">The content of the blob as a stream</param>
	/// <param name="contentType">The MIME type of the blob</param>
	/// <param name="blobName">The name of the blob</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>The URL of the uploaded blob as a string</returns>
	Task<string> UploadBlobAsync(Stream content, string contentType, string blobName, CancellationToken cancellationToken = default);

	/// <summary>
	/// Method to delete a blob from the storage
	/// </summary>
	/// <param name="blobName">The name of the blob to delete</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns></returns>
	Task DeleteBlobAsync(string blobName, CancellationToken cancellationToken = default);

	/// <summary>
	/// Methods to retrieve the URL of a blob by its Id
	/// </summary>
	/// <param name="blobId"></param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>Url</returns>
	Task<string> GetBlobUrlByIdAsync(string blobId, CancellationToken cancellationToken = default);
}