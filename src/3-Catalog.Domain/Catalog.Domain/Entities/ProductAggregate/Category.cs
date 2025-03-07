using System.Collections.Generic;
using Catalog.Core.SharedKernel;
using Catalog.Domain.Entities.ProductAggregate.Events.Categories;

namespace Catalog.Domain.Entities.ProductAggregate;
public class Category : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public List<Product> Products { get; private set; } = [];

    public bool _isDeleted { get; private set; } = false;

    public Category() {}

    public Category(string nome, string descricao)
    {
        Name = nome;
        Description = descricao;
        AddDomainEvent(new CategoryCreatedEvent(this));
    }

    public void Update(string nome, string descricao)
    {
        Name = nome;
        Description = descricao;
        AddDomainEvent(new CategoryUpdatedEvent(this));
    }

    public void AddProduct(Product product)
    {
        product.Categories.Add(this);
        Products.Add(product);
        AddDomainEvent(new CategoryUpdatedEvent(this));
    }

    public void Delete()
    {
        if (_isDeleted) return;

        _isDeleted = true;
        AddDomainEvent(new CategoryDeletedEvent(this));
    }
}
