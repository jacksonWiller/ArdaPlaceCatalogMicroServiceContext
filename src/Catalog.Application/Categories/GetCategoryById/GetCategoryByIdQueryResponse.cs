using Catalog.Core.SharedKernel;
using Catalog.Domain.Entities.ProductAggregate;

namespace Catalog.Application.Categories.GetCategoryById;

public class GetCategoryByIdQueryResponse(Category category) : IResponse
{
    public string Name { get; set; } = category.Name;
    public string Description { get; set; } = category.Description;
}
