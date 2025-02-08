using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Catalog.Core.AppSettings;
using Catalog.Core.Extensions;
using Catalog.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.OpenApi.Models;
using Catalog.Infrastructure.PostgreSql.Data.Context;
using Catalog.Infrastructure.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace Catalog.Api.Extensions;

[ExcludeFromCodeCoverage]
internal static class ServicesCollectionExtensions
{
    private const int DbMaxRetryCount = 3;
    private const int DbCommandTimeout = 35;
    private const string DbMigrationAssemblyName = "Catalog.Api.Hosted";
    private const string RedisInstanceName = "master";

    private static readonly string[] DbRelationalTags = ["database", "ef-core", "sql-server", "relational"];
    private static readonly string[] DbNoSqlTags = ["database", "mongodb", "no-sql"];

    public static void AddSwagger(this IServiceCollection services)
    {
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

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            swaggerOptions.IncludeXmlComments(xmlPath, true);
        });
    }

    public static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetOptions<ConnectionOptions>();

        var healthCheckBuilder = services
            .AddHealthChecks()
            .AddDbContextCheck<CatalogDbContext>(tags: DbRelationalTags)
            //.AddDbContextCheck<EventStoreDbContext>(tags: DbRelationalTags)
            .AddMongoDb(options.NoSqlConnection, tags: DbNoSqlTags);

        if (!options.CacheConnectionInMemory())
            healthCheckBuilder.AddRedis(options.CacheConnection);
    }

    public static IServiceCollection AddWriteDbContext(this IServiceCollection services)
    {
        services.AddDbContext<CatalogDbContext>((serviceProvider, optionsBuilder) =>
            ConfigureDbContext<CatalogDbContext>(serviceProvider, optionsBuilder, QueryTrackingBehavior.TrackAll));

        //services.AddDbContext<EventStoreDbContext>((serviceProvider, optionsBuilder) =>
        //    ConfigureDbContext<EventStoreDbContext>(serviceProvider, optionsBuilder, QueryTrackingBehavior.NoTrackingWithIdentityResolution));

        return services;
    }

    public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetOptions<ConnectionOptions>();
        if (options.CacheConnectionInMemory())
        {
            services.AddMemoryCacheService();
            services.AddMemoryCache(memoryOptions => memoryOptions.TrackStatistics = true);
        }
        else
        {
            services.AddDistributedCacheService();
            services.AddStackExchangeRedisCache(redisOptions =>
            {
                redisOptions.InstanceName = RedisInstanceName;
                redisOptions.Configuration = options.CacheConnection;
            });
        }
        return services;
    }

    private static void ConfigureDbContext<TContext>(
    IServiceProvider serviceProvider,
    DbContextOptionsBuilder optionsBuilder,
    QueryTrackingBehavior queryTrackingBehavior) where TContext : DbContext
    {
        var logger = serviceProvider.GetRequiredService<ILogger<TContext>>();
        var options = serviceProvider.GetOptions<ConnectionOptions>();

        optionsBuilder
            .UseNpgsql(options.SqlConnection, sqlServerOptions =>
            {
                sqlServerOptions.MigrationsAssembly("Catalog.WebApi");
                sqlServerOptions.EnableRetryOnFailure(DbMaxRetryCount);
                sqlServerOptions.CommandTimeout(DbCommandTimeout);
            })
            .UseQueryTrackingBehavior(queryTrackingBehavior)
            .LogTo((eventId, _) => eventId.Id == CoreEventId.ExecutionStrategyRetrying, eventData =>
            {
                if (eventData is not ExecutionStrategyEventData retryEventData)
                    return;

                var exceptions = retryEventData.ExceptionsEncountered;

                logger.LogWarning(
                    "----- DbContext: Retry #{Count} with delay {Delay} due to error: {Message}",
                    exceptions.Count,
                    retryEventData.Delay,
                    exceptions[^1].Message);
            });

        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        if (!environment.IsProduction())
        {
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}