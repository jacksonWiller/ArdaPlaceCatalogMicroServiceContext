//using Catalog.Core.SharedKernel;
//using Microsoft.EntityFrameworkCore;
//using Catalog.Infrastructure.Data.Mappings;

//namespace Catalog.Infrastructure.Data.Context;

//public class EventStoreDbContext(DbContextOptions<EventStoreDbContext> dbOptions) : BaseDbContext<EventStoreDbContext>(dbOptions)
//{
//    public DbSet<EventStore> EventStores => Set<EventStore>();

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        base.OnModelCreating(modelBuilder);

//        modelBuilder.ApplyConfiguration(new EventStoreConfiguration());
//    }
//}