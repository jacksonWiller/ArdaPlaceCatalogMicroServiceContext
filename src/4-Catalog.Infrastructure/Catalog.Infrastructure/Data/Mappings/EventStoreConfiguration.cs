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
            .IsRequired()
            .ValueGeneratedNever();
        builder
            .Property(eventStore => eventStore.AggregateId)
            .IsRequired();
        builder
            .Property(eventStore => eventStore.MessageType)
            .IsRequired()
            .HasMaxLength(255); // Aumentei para 255 para manter consistência
        builder
            .Property(eventStore => eventStore.Data)
            .IsRequired()
            .HasColumnType("text") // Mudança aqui - usando text que é ilimitado no PostgreSQL
            .HasComment("JSON serialized event");
        builder
            .Property(eventStore => eventStore.OccurredOn)
            .IsRequired()
            .HasColumnName("OccurredOn"); // Corrigi para manter o nome original da coluna
    }
}