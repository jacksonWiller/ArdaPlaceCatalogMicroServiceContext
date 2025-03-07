using Catalog.Core.SharedKernel;

namespace Catalog.Domain.Entities.ProductAggregate.Events.Products;

public abstract class ProductBaseEvent(Product product) : BaseEvent
{
    public Product Product { get; private init; } = product;
}
