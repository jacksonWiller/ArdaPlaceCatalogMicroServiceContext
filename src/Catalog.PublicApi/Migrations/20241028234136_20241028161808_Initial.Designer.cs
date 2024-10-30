﻿// <auto-generated />
using System;
using Catalog.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Catalog.PublicApi.Migrations
{
    [DbContext(typeof(CatalogDbContext))]
    [Migration("20241028234136_20241028161808_Initial")]
    partial class _20241028161808_Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("Latin1_General_CI_AI")
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Catalog.Core.SharedKernel.EventStore", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AggregateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("NVARCHAR(MAX)")
                        .HasComment("JSON serialized event");

                    b.Property<string>("MessageType")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("OccurredOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("EventStores", (string)null);

                    b.HasDiscriminator().HasValue("EventStore");
                });

            modelBuilder.Entity("Catalog.Domain.Entities.ProductAggregate.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("SKU")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<int>("StockQuantity")
                        .HasColumnType("int");

                    b.Property<bool>("_isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Products", (string)null);
                });

            modelBuilder.Entity("Catalog.Domain.Entities.ProductAggregate.Product", b =>
                {
                    b.OwnsMany("Catalog.Domain.ValueObjects.Image", "Images", b1 =>
                        {
                            b1.Property<Guid>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Nome")
                                .IsRequired()
                                .HasMaxLength(255)
                                .IsUnicode(false)
                                .HasColumnType("varchar(255)");

                            b1.Property<string>("Prefix")
                                .IsRequired()
                                .HasMaxLength(255)
                                .IsUnicode(false)
                                .HasColumnType("varchar(255)");

                            b1.Property<Guid>("ProductId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Url")
                                .IsRequired()
                                .HasMaxLength(255)
                                .IsUnicode(false)
                                .HasColumnType("varchar(255)");

                            b1.HasKey("Id");

                            b1.HasIndex("ProductId");

                            b1.ToTable("ProductImages", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("ProductId");
                        });

                    b.Navigation("Images");
                });
#pragma warning restore 612, 618
        }
    }
}
