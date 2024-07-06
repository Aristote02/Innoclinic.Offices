using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Serilog;

namespace Offices.Infrastructure.Configurations;

/// <summary>
/// Static class to configure external dependencies and services
/// </summary>
public static class ConfigureServices
{
	/// <summary>
	/// Extension method to configure MongoDB services
	/// </summary>
	/// <param name="builder">The web application builder</param>
	/// <returns>The service collection</returns>
	public static IServiceCollection ConfigureMongoDb(this WebApplicationBuilder builder)
	{
		builder.Services.Configure<OfficeMongoDbSettings>(builder.Configuration.GetSection("OfficeMongoDbSettings"));

		builder.Services.AddSingleton<IMongoClient>(_ =>
		{
			var connectionString = builder.Configuration.GetSection("OfficeMongoDbSettings:ConnectionString").Value;

			return new MongoClient(connectionString);
		});

		return builder.Services;
	}

	/// <summary>
	/// Extension method to configure Serilog logging
	/// </summary>
	/// <param name="builder">The web application builder</param>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddLogger(this WebApplicationBuilder builder)
	{
		builder.Host.UseSerilog((context, configuration) =>
		{
			configuration.ReadFrom.Configuration(builder.Configuration)
			.Enrich.FromLogContext()
			.WriteTo.Console()
			.Enrich.WithEnvironmentName()
			.Enrich.WithMachineName();
		});

		return builder.Services;
	}
}
