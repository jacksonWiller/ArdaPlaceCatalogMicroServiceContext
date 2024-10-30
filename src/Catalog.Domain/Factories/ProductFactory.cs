using Ardalis.Result;
using Catalog.Domain.Entities.ProductAggregate;

namespace Catalog.Domain.Factories;

public static class ProductFactory
{
    public static Result<Product> Create(
        string name,
        string description,
        string category,
        decimal price,
        int stockQuantity,
        string sku,
        string brand)
    {
        var product = new Product(name, description, category, price, stockQuantity, sku, brand);
        return Result<Product>.Success(product);
    }
}
