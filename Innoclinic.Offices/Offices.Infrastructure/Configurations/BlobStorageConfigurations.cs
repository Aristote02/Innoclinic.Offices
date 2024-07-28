namespace Offices.Infrastructure.Configurations;

/// <summary>
/// Configuration settings for Blob Storage
/// </summary>
public class BlobStorageConfigurations
{
	public required string ConnectionString { get; init; }
	public required string AccountName { get; init; }
	public required string ContainerName { get; init; }
	public required string PrimaryKey { get; init; }
}