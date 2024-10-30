using System.Diagnostics.CodeAnalysis;
using Catalog.Core.SharedKernel;
using Catalog.Domain.DataContext;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Data.Context;
using Catalog.Infrastructure.Data.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure;

[ExcludeFromCodeCoverage]
public static class ConfigureServices
{
    /// <summary>
    /// Adds the memory cache service to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static void AddMemoryCacheService(this IServiceCollection services) =>
        services.AddScoped<ICacheService, MemoryCacheService>();

    /// <summary>
    /// Adds the distributed cache service to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static void AddDistributedCacheService(this IServiceCollection services) =>
        services.AddScoped<ICacheService, DistributedCacheService>();

    /// <summary>
    /// Adds the infrastructure services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services) =>
        services
            .AddScoped<ICatalogDbContext, CatalogDbContext>()
            .AddScoped<IUnitOfWork, UnitOfWork>();

}