namespace Catalog.Domain.Entities.ProductAggregate.Events;

public class ProductDeletedEvent(Product product) : ProductBaseEvent(product)
{
}
