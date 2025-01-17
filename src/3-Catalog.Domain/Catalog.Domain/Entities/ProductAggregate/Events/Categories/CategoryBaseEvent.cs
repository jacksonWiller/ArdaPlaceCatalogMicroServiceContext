using Catalog.Core.SharedKernel;

namespace Catalog.Domain.Entities.ProductAggregate.Events.Categories;

public abstract class CategoryBaseEvent(Category category) : BaseEvent
{
    public Category Category { get; private init; } = category;
}
