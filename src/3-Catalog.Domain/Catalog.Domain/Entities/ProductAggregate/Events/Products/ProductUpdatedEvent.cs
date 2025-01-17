namespace Catalog.Domain.Entities.ProductAggregate.Events.Products;

public class ProductUpdatedEvent(Product product) : ProductBaseEvent(product)
{
}
