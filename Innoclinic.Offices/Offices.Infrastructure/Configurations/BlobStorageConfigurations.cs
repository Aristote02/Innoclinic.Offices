namespace Offices.Infrastructure.Configurations;

/// <summary>
/// Configuration settings for Blob Storage
/// </summary>
public class BlobStorageConfigurations
{
	public string ConnectionString { get; set; } = string.Empty;
	public string AccountName { get; set; } = string.Empty;
	public string ContainerName { get; set; } = string.Empty;
	public string PrimaryKey { get; set; } = string.Empty;
}
