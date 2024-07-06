using MongoDB.Driver;
using Offices.Domain.Entities;

namespace Offices.Contracts.Repositories.Interfaces;

/// <summary>
/// The interface for MongoDB context, that provides access to collections of Office entities
/// </summary>
public interface IMongoDbContext
{
	/// <summary>
	/// Method to get the MongoDB collection of Office entities
	/// </summary>
	IMongoCollection<Office> Offices { get; }
}
