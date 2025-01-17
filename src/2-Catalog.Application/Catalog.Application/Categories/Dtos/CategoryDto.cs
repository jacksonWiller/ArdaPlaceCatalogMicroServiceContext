using System;
using System.Collections.Generic;

namespace Catalog.Application.Categories.Dtos;
public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    //public List<ProductDto> Products { get; set; }
}
