using System.Collections.Generic;
using Ardalis.Result;
using Catalog.Application.Products.Dtos;
using Catalog.Core.SharedKernel;

namespace Catalog.Application.Products.GetAllProducts;

public class GetAllProductsQueryResponse() : IResponse
{
    public PagedInfo PagedInfo { get; set; }
    public List<ProductDto> Products { get; set; }
}
