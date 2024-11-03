using System.Collections.Generic;
using Ardalis.Result;
using Catalog.Application.Products.Dtos;
using Catalog.Core.SharedKernel;

namespace Catalog.Application.Categories.GetAllCategorys;

public class GetAllCategorysQueryResponse() : IResponse
{
    public PagedInfo PagedInfo { get; set; }
    public List<CategoryDto> Categorys { get; set; }
}
