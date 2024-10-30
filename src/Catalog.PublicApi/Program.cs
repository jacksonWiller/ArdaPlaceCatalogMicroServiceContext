using System.Reflection;
using Catalog.Application;
using Catalog.Core;
using Catalog.Core.AppSettings;
using Catalog.Core.Extensions;
using Catalog.Core.SharedKernel;
using Catalog.Domain.DataContext;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Data.Context;
using Catalog.PublicApi.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection();

var configuration = builder.Configuration;
var services = builder.Services;

// Configurar AppSettings
//services.ConfigureAppSettings();

// Adicionar infraestrutura
services.AddScoped<ICatalogDbContext, CatalogDbContext>()
        .AddScoped<IUnitOfWork, UnitOfWork>();

// Adicionar contexto de banco de dados
services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("SqlConnection"), sqlServerOptions =>
    {
        sqlServerOptions.MigrationsAssembly("Catalog.PublicApi"); // Certifique-se de que o assembly de migrações está correto
    }));

// Adicionar manipuladores de comando
services.AddCommandHandlers();

// Adicionar serviço de cache
var cacheOptions = configuration.GetOptions<ConnectionOptions>();
if (cacheOptions.CacheConnectionInMemory())
{
    services.AddMemoryCacheService();
    services.AddMemoryCache(memoryOptions => memoryOptions.TrackStatistics = true);
}
else
{
    services.AddDistributedCacheService();
    services.AddStackExchangeRedisCache(redisOptions =>
    {
        redisOptions.InstanceName = "master";
        redisOptions.Configuration = cacheOptions.CacheConnection;
    });
}

if (!cacheOptions.CacheConnectionInMemory())
    services.AddHealthChecks().AddRedis(cacheOptions.CacheConnection);

// Adicionar CORS
services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(swaggerOptions =>
{
    swaggerOptions.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Catalog (Arada Place e-commerce)",
        Description = "ASP.NET Core C# CQRS Event Sourcing, REST API, DDD, SOLID Principles and Clean Architecture",
        Contact = new OpenApiContact
        {
            Name = "Jackson Duarte",
            Email = "jacksonwillerdaurte@gmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License"
        }
    });
});

services.AddHttpClient("IgnoreSslErrors")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseErrorHandling();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();

await app.RunAppAsync();