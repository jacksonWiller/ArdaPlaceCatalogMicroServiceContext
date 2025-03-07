using System.Collections.Generic;
using Catalog.Core.SharedKernel;
using Catalog.Domain.Entities.ProductAggregate.Events.Products;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Entities.ProductAggregate;

public class Product : BaseEntity, IAggregateRoot
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string SKU { get; set; }
    public string Brand { get; set; }
    public List<Category> Categories { get; set; } = [];
    public List<Image> Images { get; set; } = [];
    public List<Tag> Tags { get; set; } = [];

    public bool _isDeleted { get; set; } = false;
    
    public Product() { }

    public Product(string name, string description, decimal price, int stockQuantity, string sku, string brand)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        SKU = sku;
        Brand = brand;
        _isDeleted = false;
        AddDomainEvent(new ProductCreatedEvent(this));
    }

    public void Update(string name, string description, decimal price, int stockQuantity, string sku, string brand)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        SKU = sku;
        Brand = brand;

        AddDomainEvent(new ProductUpdatedEvent(this));
    }

    public void AddCategory(List<Category> categories)
    {
        Categories.AddRange(categories);
        AddDomainEvent(new ProductUpdatedEvent(this));
    }

    public void RemoveCategory(List<Category> categories)
    {
        Categories.RemoveAll(categories.Contains);
        AddDomainEvent(new ProductUpdatedEvent(this));
    }

    public void AddImage(List<Image> images)
    {
        AddDomainEvent(new ProductUpdatedEvent(this));
        Images = images;
    }

    public void AddTags(List<string> names)
    {
        foreach (var name in names)
        {
            Tags.Add(new Tag(name));
        }

        AddDomainEvent(new ProductUpdatedEvent(this));
    }

    public void Delete()
    {
        if (_isDeleted) return;

        _isDeleted = true;
        AddDomainEvent(new ProductDeletedEvent(this));
    }
}
