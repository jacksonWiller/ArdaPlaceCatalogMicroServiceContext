using System;
using System.Collections.Generic;
using Catalog.Domain.Entities.ProductAggregate;
using Catalog.Infrastructure.PostgreSql.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.PostgreSql.Data.Mappings;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.ConfigureBaseEntity();

        builder
            .Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(product => product.Description)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(product => product.Price)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder
            .Property(product => product.StockQuantity)
            .HasColumnType("int");

        builder
            .Property(product => product.SKU)
            .IsRequired() 
            .HasMaxLength(100);

        builder
            .Property(product => product.Brand)
            .IsRequired()
            .HasMaxLength(100);

        // Configuração para a coleção de imagens
        builder.OwnsMany(product => product.Images, p =>
        {
            p.WithOwner().HasForeignKey("ProductId");
            p.Property<Guid>("Id");
            p.HasKey("Id");
            p.Property(image => image.Name).IsRequired().HasMaxLength(255);
            p.Property(image => image.Prefix).IsRequired().HasMaxLength(255);
            p.Property(image => image.Url).IsRequired().HasMaxLength(255);
            p.ToTable("ProductImages");
        });

        // Configuração para a coleção de tags
        builder.OwnsMany(product => product.Tags, p =>
        {
            p.WithOwner().HasForeignKey("ProductId");
            p.Property<Guid>("Id");
            p.HasKey("Id");
            p.Property(tag => tag.Name).IsRequired().HasMaxLength(255);
            p.ToTable("ProductTags");
        });

        builder
            .HasMany(product => product.Categories)
            .WithMany(category => category.Products)
            .UsingEntity<Dictionary<string, object>>(
                "ProductCategory",
                j => j
                    .HasOne<Category>()
                    .WithMany()
                    .HasForeignKey("CategoryId")
                    .HasConstraintName("FK_ProductCategory_Categories_CategoryId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Product>()
                    .WithMany()
                    .HasForeignKey("ProductId")
                    .HasConstraintName("FK_ProductCategory_Products_ProductId")
                    .OnDelete(DeleteBehavior.Cascade));
    }
}