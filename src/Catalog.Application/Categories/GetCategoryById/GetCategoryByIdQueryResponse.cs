using System.Collections.Generic;
using Catalog.Application.Categories.Dtos;
using Catalog.Core.SharedKernel;

namespace Catalog.Application.Categories.GetCategoryById;

public class GetCategoryByIdQueryResponse : IResponse
{
    public string Name { get; set; } 
    public string Description { get; set; } 
    //public List<ProductDto> Products { get; set; } 
}
