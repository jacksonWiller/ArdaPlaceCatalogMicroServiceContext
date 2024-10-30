using System;
using System.Collections.Generic;

namespace Catalog.Application.Products.Dtos;
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string SKU { get; set; }
    public string Brand { get; set; }
    public List<ImageDto> Images { get; set; }
}
