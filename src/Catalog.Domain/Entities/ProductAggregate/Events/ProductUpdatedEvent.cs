namespace Catalog.Domain.Entities.ProductAggregate.Events;

public class ProductUpdatedEvent(Product product) : ProductBaseEvent(product)
{
}
