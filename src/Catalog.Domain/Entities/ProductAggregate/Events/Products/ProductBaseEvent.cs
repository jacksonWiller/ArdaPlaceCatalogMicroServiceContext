using Catalog.Core.SharedKernel;

namespace Catalog.Domain.Entities.ProductAggregate.Events.Products;

public abstract class CategoryBaseEvent(Product product) : BaseEvent
{
    public Product Product { get; private init; } = product;
}
