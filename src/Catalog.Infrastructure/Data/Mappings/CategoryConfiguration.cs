using System.Collections.Generic;
using Catalog.Domain.Entities.ProductAggregate;
using Catalog.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Mappings;

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
            .HasMaxLength(100);

        builder
            .Property(category => category.Description)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .HasMany(category => category.Products)
            .WithMany(product => product.Categories)
            .UsingEntity<Dictionary<string, object>>(
                "ProductCategory",
                j => j
                    .HasOne<Product>()
                    .WithMany()
                    .HasForeignKey("ProductId")
                    .HasConstraintName("FK_ProductCategory_Products_ProductId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Category>()
                    .WithMany()
                    .HasForeignKey("CategoryId")
                    .HasConstraintName("FK_ProductCategory_Categories_CategoryId")
                    .OnDelete(DeleteBehavior.Cascade));
    }
}