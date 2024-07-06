using MongoDB.Bson.Serialization.Attributes;

namespace Offices.Domain.Entities;


/// <summary>
/// Represents an office entity
/// </summary>
public class Office
{
	/// <summary>
	/// Gets or sets the unique identifier for the office
	/// </summary>
	[BsonId]
	public Guid Id { get; set; }

	/// <summary>
	/// Gets or sets the address of the office
	/// </summary>
	[BsonElement("address")]
	public string Address { get; set; } = null!;

	/// <summary>
	/// Gets or sets the registry phone number of the office
	/// </summary>
	[BsonElement("registryPhoneNumber")]
	public string RegistryPhoneNumber { get; set; } = null!;

	/// <summary>
	/// Gets or sets the identifier for the photo associated with the office
	/// </summary>
	[BsonElement("photoId")]
	public string? PhotoId { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the office is active
	/// </summary>
	[BsonElement("isActive")]
	public bool IsActive { get; set; }
}
