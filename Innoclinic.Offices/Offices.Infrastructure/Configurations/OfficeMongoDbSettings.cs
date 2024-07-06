namespace Offices.Infrastructure.Configurations;

/// <summary>
/// Represents the settings for the database
/// </summary>
public class OfficeMongoDbSettings
{
	/// <summary>
	/// Gets or sets the name of the offices collection
	/// </summary>
	public string OfficesCollectionName { get; set; } = null!;

	/// <summary>
	/// Gets or sets the connection string
	/// </summary>
	public string ConnectionString { get; set; } = null!;

	/// <summary>
	/// Gets or sets the name of the database
	/// </summary>
	public string DataBaseName { get; set; } = null!;
}
