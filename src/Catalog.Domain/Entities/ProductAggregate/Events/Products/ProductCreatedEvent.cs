namespace Catalog.Domain.Entities.ProductAggregate.Events.Products;

public class ProductCreatedEvent(Product product) : CategoryBaseEvent(product)
{
}
