namespace Catalog.Domain.Entities.ProductAggregate.Events;
public class ProductCreatedEvent(Product product) : ProductBaseEvent(product)
{
}
