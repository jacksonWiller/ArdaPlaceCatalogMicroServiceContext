using Ardalis.Result;
using MediatR;
namespace Catalog.Application.Products.GetAllProducts;
public class GetAllProductsQuery : IRequest<Result<GetAllProductsQueryResponse>>
{
    public string Filter { get; set; } = string.Empty;
    public string Order { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
