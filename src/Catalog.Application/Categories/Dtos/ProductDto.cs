using System;

namespace Catalog.Application.Products.Dtos;
public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
