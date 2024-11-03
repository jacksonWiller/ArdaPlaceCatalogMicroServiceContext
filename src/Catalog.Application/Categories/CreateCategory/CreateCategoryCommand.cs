using Ardalis.Result;
using MediatR;
namespace Catalog.Application.Categories.CreateCategory;
public class CreateCategoryCommand : IRequest<Result<CreateCategoryResponse>>
{
    public string Name { get; set; }
    public string Description { get; set; }
  
}
