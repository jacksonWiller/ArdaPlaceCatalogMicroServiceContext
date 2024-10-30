using Catalog.Core.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Mappings;

internal class EventStoreConfiguration : IEntityTypeConfiguration<EventStore>
{
    public void Configure(EntityTypeBuilder<EventStore> builder)
    {
        builder.ToTable("EventStores");

        builder
            .Property(eventStore => eventStore.Id)
            .IsRequired() // NOT NULL
            .ValueGeneratedNever();

        builder
            .Property(eventStore => eventStore.AggregateId)
            .IsRequired(); // NOT NULL

        builder
            .Property(eventStore => eventStore.MessageType)
            .IsRequired() // NOT NULL
            .HasMaxLength(100);

        builder
            .Property(eventStore => eventStore.Data)
            .IsRequired() // NOT NULL
            .HasColumnType("NVARCHAR(MAX)")
            .HasComment("JSON serialized event");

        builder
            .Property(eventStore => eventStore.OccurredOn)
            .IsRequired() // NOT NULL
            .HasColumnName("CreatedAt");
    }
}