using Catalog.Domain.Entities.ProductAggregate;
using Catalog.Infrastructure.PostgreSql.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.PostgreSql.Data.Mappings;

internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder
            .ConfigureBaseEntity();
        builder
            .Property(category => category.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("varchar(100)"); // Explicitamente definindo o tipo
        builder
            .Property(category => category.Description)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("varchar(100)"); // Explicitamente definindo o tipo
    }
}