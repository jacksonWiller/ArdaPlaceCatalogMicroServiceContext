using Microsoft.EntityFrameworkCore;
using Catalog.Infrastructure.Data.Mappings;

using Catalog.Domain.Entities.ProductAggregate;
using Catalog.Domain.DataContext;
using Catalog.Core.SharedKernel;

namespace Catalog.Infrastructure.Data.Context;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> dbOptions) : BaseDbContext<CatalogDbContext>(dbOptions), ICatalogDbContext
{
    public DbSet<EventStore> EventStores { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ProductConfiguration());

        modelBuilder.ApplyConfiguration(new CategoryConfiguration());

        modelBuilder.ApplyConfiguration(new EventStoreConfiguration());
    }
}