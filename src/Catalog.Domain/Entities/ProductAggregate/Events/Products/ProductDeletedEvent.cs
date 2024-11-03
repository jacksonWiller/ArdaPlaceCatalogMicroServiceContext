namespace Catalog.Domain.Entities.ProductAggregate.Events.Products;

public class ProductDeletedEvent(Product product) : CategoryBaseEvent(product)
{
}
