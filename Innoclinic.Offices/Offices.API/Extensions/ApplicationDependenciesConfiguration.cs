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

public static class ApplicationDependenciesConfiguration
{
	public static IServiceCollection ConfigureServices(this WebApplicationBuilder builder)
	{
		builder.ConfigureMongoDb();
		builder.AddLogger()
			.AddServices()
			.AddValidators()
			.AddAutoMapper(typeof(MappingProfile));

		return builder.Services;
	}

	public static IServiceCollection AddServices(this IServiceCollection services)
	{
		services.AddScoped<IMongoDbContext, MongoDbContext>()
			.AddScoped<IOfficeService, OfficeService>()
			.AddScoped<IOfficeRepository, OfficeRepository>()
			.AddScoped<IBlobService, BlobService>()
			.AddScoped<IBlobClientFactory, BlobClientFactory>();

		return services;
	}

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

	public static IServiceCollection AddValidators(this IServiceCollection services)
	{
		return services.AddValidatorsFromAssembly(Application.AssemblyReference.Assembly);
	}

	public static IServiceCollection ConfigureJwtAuth(this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JwtSecretKey:Key")))
				};
			});

		return services;
	}

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
}