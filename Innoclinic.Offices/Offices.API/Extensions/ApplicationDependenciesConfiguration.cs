#region NamespaceImportsAndDependencies
using Azure.Storage.Blobs;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Offices.Application.Contracts.Services.Interfaces;
using Offices.Application.Profiles;
using Offices.Application.Services.Implementations;
using Offices.Contracts.Repositories.Interfaces;
using Offices.Infrastructure.Configurations;
using Offices.Infrastructure.Data;
using Offices.Infrastructure.Repositories;
using Offices.Infrastructure.Storage;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
#endregion

namespace Offices.API.Extensions;

/// <summary>
/// Configures all the application service
/// </summary>
public static class ApplicationDependenciesConfiguration
{
	/// <summary>
	/// Adds and configure all the services of the application
	/// </summary>
	/// <param name="builder">The WebApplicationBuilder</param>
	/// <returns>The service collection</returns>
	public static IServiceCollection ConfigureServices(this WebApplicationBuilder builder)
	{
		builder.ConfigureMongoDb();
		builder.AddLogger()
			.AddServices()
			.AddValidators()
			.AddAutoMapper(typeof(MappingProfile));

		return builder.Services;
	}

	/// <summary>
	/// Adds services to the <paramref name="services"/> collection
	/// </summary>
	///<param name="services">The <see cref="IServiceCollection"/> to which services are added</param>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddServices(this IServiceCollection services)
	{
		services.AddScoped<IMongoDbContext, MongoDbContext>()
			.AddScoped<IOfficeService, OfficeService>()
			.AddScoped<IOfficeRepository, OfficeRepository>()
			.AddScoped<IBlobService, BlobService>()
			.AddScoped<IBlobClientFactory, BlobClientFactory>();

		return services;
	}

	/// <summary>
	/// Adds Blob Storage related services to the <paramref name="services"/> collection
	/// </summary>
	/// <param name="services">The <see cref="IServiceCollection"/> to which services are added</param>
	/// <returns>The service collection with added Blob Storage services</returns>
	public static IServiceCollection ConfigureBlobStorageServices(this IServiceCollection services)
	{
		services.AddScoped<BlobServiceClient>(x =>
		{
			var options = x.GetRequiredService<IOptions<BlobStorageConfigurations>>().Value;
			if (string.IsNullOrEmpty(options.ConnectionString))
			{
				throw new ArgumentException("The connection string for the blob storage was not found, it can't be null or empty");
			}

			return new BlobServiceClient(options.ConnectionString);
		});

		return services;
	}

	/// <summary>
	/// Configures Cross-Origin Resource Sharing (CORS) for the application
	/// </summary>
	/// <param name="builder">The WebApplicationBuilder used to configure services and middleware</param>
	public static void ConfigureCrossOriginRessourceSharing(this WebApplicationBuilder builder)
	{
		builder.Services.AddCors(options =>
		{
			options.AddDefaultPolicy(
				policy =>
				{
					policy.WithOrigins("*")
					.AllowAnyHeader()
					.AllowAnyMethod();
				});
		});
	}

	/// <summary>
	/// Configures fluent validation
	/// </summary>
	/// <param name="services"></param>
	/// <returns>The service collection with added validators</returns>
	public static IServiceCollection AddValidators(this IServiceCollection services)
	{
		return services.AddValidatorsFromAssembly(Application.AssemblyReference.Assembly);
	}

	/// <summary>
	/// Configures the JWT authentication
	/// </summary>
	/// <param name="builder">The WebApplicationBuilder</param>
	/// <returns>The service collection</returns>
	/// <exception cref="InvalidOperationException">Thrown if the operation fails</exception>
	public static IServiceCollection ConfigureJwtAuthentication(this WebApplicationBuilder builder)
	{
		var jwtSection = builder.Configuration.GetSection("Jwt");
		if (!jwtSection.Exists() || !ValidateJwtSettings(jwtSection))
		{
			throw new InvalidOperationException("JWT configuration values are missing or invalid");
		}

		builder.Services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSection["Issuer"],
					ValidAudience = jwtSection["Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!))
				};
			});

		return builder.Services;
	}

	/// <summary>
	/// Configure swagger for API documentation
	/// </summary>
	/// <param name="services"></param>
	/// <returns>The service collection with added Swagger configuration</returns>
	public static IServiceCollection ConfigureSwaggerGen(this IServiceCollection services)
	{
		services.AddSwaggerGen(c =>
		{
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Description = "Standard Authorisation header using the scheme (\"bearer {token}\")",
				In = ParameterLocation.Header,
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey
			});

			c.OperationFilter<SecurityRequirementsOperationFilter>();

			c.SwaggerDoc("v1", new OpenApiInfo { Title = "Office API", Version = "v1" });
			var xmlPath = Path.Combine(AppContext.BaseDirectory, "Offices.Presentation.xml");
			c.IncludeXmlComments(xmlPath);
		});

		return services;
	}

	/// <summary>
	/// Validates JWT settings
	/// </summary>
	/// <param name="jwtSection">The configuration section containing JWT settings</param>
	/// <returns>True if JWT settings are valid, otherwise false</returns>
	private static bool ValidateJwtSettings(IConfigurationSection jwtSection)
	{
		return !string.IsNullOrEmpty(jwtSection["Issuer"]) &&
			   !string.IsNullOrEmpty(jwtSection["Audience"]) &&
			   !string.IsNullOrEmpty(jwtSection["Key"]);
	}
}