namespace Catalog.Domain.Entities.ProductAggregate.Events.Categories;
public class CategoryDeletedEvent(Category category) : CategoryBaseEvent(category)
{
}
