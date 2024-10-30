using System;
using Catalog.Domain.Entities.ProductAggregate;
using Catalog.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Mappings;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder
            .ConfigureBaseEntity();

        builder
            .Property(product => product.Name)
            .IsRequired() // NOT NULL
            .HasMaxLength(100);

        builder
            .Property(product => product.Description)
            .IsRequired() // NOT NULL
            .HasMaxLength(100);

        builder
            .Property(product => product.Category)
            .IsRequired() // NOT NULL
            .HasMaxLength(100);

        builder
            .Property(product => product.Price)
            .IsRequired() // NOT NULL
            .HasColumnType("decimal(18,2)");

        builder
            .Property(product => product.StockQuantity)
            .IsRequired() // NOT NULL
            .HasColumnType("int");

        builder
            .Property(product => product.SKU)
            .IsRequired() // NOT NULL
            .HasMaxLength(100);

        builder
            .Property(product => product.Brand)
            .IsRequired() // NOT NULL
            .HasMaxLength(100);

        // Configuração para a coleção de imagens
        builder.OwnsMany(product => product.Images, p =>
        {
            p.WithOwner().HasForeignKey("ProductId");
            p.Property<Guid>("Id");
            p.HasKey("Id");
            p.Property(image => image.Nome).IsRequired().HasMaxLength(255);
            p.Property(image => image.Prefix).IsRequired().HasMaxLength(255);
            p.Property(image => image.Url).IsRequired().HasMaxLength(255);
            p.ToTable("ProductImages"); // Nome da tabela para armazenar as imagens
        });
    }
}