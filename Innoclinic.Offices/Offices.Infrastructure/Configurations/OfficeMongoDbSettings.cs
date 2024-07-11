namespace Offices.Infrastructure.Configurations;

/// <summary>
/// Represents the settings for the database
/// </summary>
public class OfficeMongoDbSettings
{
	/// <summary>
	/// Gets or sets the name of the offices collection
	/// </summary>
	public required string OfficesCollectionName { get; init; }

	/// <summary>
	/// Gets or sets the connection string
	/// </summary>
	public required string ConnectionString { get; init; }

	/// <summary>
	/// Gets or sets the name of the database
	/// </summary>
	public required string DataBaseName { get; init; }
}
