using Microsoft.EntityFrameworkCore;
using Catalog.Infrastructure.Data.Context;

namespace Catalog.PublicApi.Extensions;

internal static class WebApplicationExtensions
{
    public static async Task RunAppAsync(this WebApplication app)
    {
        await using var serviceScope = app.Services.CreateAsyncScope();

        app.Logger.LogInformation("----- AutoMapper: mappings are being validated...");

        app.Logger.LogInformation("----- AutoMapper: mappings are valid!");

        app.Logger.LogInformation("----- Databases are being migrated....");

        await app.MigrateDataBasesAsync(serviceScope);

        app.Logger.LogInformation("----- Databases have been successfully migrated!");

        app.Logger.LogInformation("----- Application is starting....");

        await app.RunAsync();
    }

    private static async Task MigrateDataBasesAsync(this WebApplication app, AsyncServiceScope serviceScope)
    {
        await using var writeDbContext = serviceScope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        try
        {
            await app.MigrateDbContextAsync(writeDbContext);
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An exception occurred while initializing the application: {Message}", ex.Message);
            throw;
        }
    }

    private static async Task MigrateDbContextAsync<TContext>(this WebApplication app, TContext context)
        where TContext : DbContext
    {
        var dbName = context.Database.GetDbConnection().Database;

        app.Logger.LogInformation("----- {DbName}: {DbConnection}", dbName, context.Database.GetConnectionString());
        app.Logger.LogInformation("----- {DbName}: checking if there are any pending migrations...", dbName);

        // Check if there are any pending migrations for the context.
        if ((await context.Database.GetPendingMigrationsAsync()).Any())
        {
            app.Logger.LogInformation("----- {DbName}: creating and migrating the database...", dbName);

            await context.Database.MigrateAsync();

            app.Logger.LogInformation("----- {DbName}: database was created and migrated successfully", dbName);
        }
        else
        {
            app.Logger.LogInformation("----- {DbName}: all migrations are up to date", dbName);
        }
    }
}
