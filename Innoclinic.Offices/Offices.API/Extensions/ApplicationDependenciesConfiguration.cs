using Azure.Storage.Blobs;
using FluentValidation;
using Microsoft.Extensions.Options;
using Offices.Application.Contracts.Services.Interfaces;
using Offices.Application.Profiles;
using Offices.Application.Services.Implementations;
using Offices.Application.Validators;
using Offices.Contracts.Repositories.Interfaces;
using Offices.Infrastructure.Configurations;
using Offices.Infrastructure.Data;
using Offices.Infrastructure.Repositories;
using Offices.Infrastructure.Storage;
using Offices.Shared.Requests;

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
			.AddScoped<IBlobService, BlobService>();

		return services;
	}

	public static IServiceCollection ConfigureBlobStorageServices(this IServiceCollection services)
	{
		services.AddScoped<BlobServiceClient>(x =>
		{
			var options = x.GetRequiredService<IOptions<BlobStorageConfigurations>>().Value;

			return new BlobServiceClient(options.ConnectionString);
		});

		return services;
	}

	public static IServiceCollection AddValidators(this IServiceCollection services)
	{
		services.AddValidatorsFromAssemblyContaining<OfficeRequestValidator>()
			.AddScoped<IValidator<OfficeRequest>, OfficeRequestValidator>();

		return services;
	}
}
