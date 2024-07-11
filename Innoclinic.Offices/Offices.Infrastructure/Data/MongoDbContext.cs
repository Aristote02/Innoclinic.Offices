using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Offices.Contracts.Repositories.Interfaces;
using Offices.Domain.Entities;
using Offices.Infrastructure.Configurations;

namespace Offices.Infrastructure.Data;

/// <summary>
/// Represents the MongoDb context and handling database connection and collections
/// </summary>
public class MongoDbContext : IMongoDbContext
{
	/// <summary>
	/// Represents the MongoDb database instance
	/// </summary>
	private readonly IMongoDatabase _database;

	/// <summary>
	/// Configuration settings for MongoDB
	/// </summary>
	private readonly OfficeMongoDbSettings _options;

	/// <summary>
	/// Initializes a new instance of the <see cref="MongoDbContext"/> class
	/// </summary>
	/// <param name="options">The options containing MongoDB configuration settings</param>
	/// <param name="client"></param>
	public MongoDbContext(IMongoClient client, IOptions<OfficeMongoDbSettings> options)
	{
		_options = options.Value;
		_database = client.GetDatabase(_options.DataBaseName);
	}

	/// <summary>
	/// Gets the MongoDB collection for the <see cref="Office"/> entity
	/// </summary>
	public IMongoCollection<Office> Offices => _database.GetCollection<Office>(_options.OfficesCollectionName);
}