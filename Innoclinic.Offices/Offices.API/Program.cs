using Microsoft.OpenApi.Models;
using Offices.API.Extensions;
using Offices.Infrastructure.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Office API", Version = "v1" });
	var xmlPath = Path.Combine(AppContext.BaseDirectory, "Offices.Presentation.xml");
	c.IncludeXmlComments(xmlPath);
});

builder.Services.Configure<BlobStorageConfigurations>(builder.Configuration
	.GetSection("BlobStorageConfigurations"));

builder.Services.ConfigureBlobStorageServices();
builder.ConfigureServices();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
