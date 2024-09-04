using Offices.API.Extensions;
using Offices.API.Middlewares;
using Offices.Infrastructure.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<BlobStorageConfigurations>(builder.Configuration
	.GetSection("BlobStorageConfigurations"));

builder.ConfigureCrossOriginRessourceSharing();
builder.ConfigureJwtAuthentication();

builder.Services.ConfigureBlobStorageServices();
builder.ConfigureServices()
	.ConfigureSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Offices API v1");
	});
}

app.UseHttpsRedirection();

app.UseExceptionMiddleware();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
