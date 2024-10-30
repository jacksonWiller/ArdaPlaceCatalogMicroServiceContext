using Catalog.Application.Products.Dtos;
using Catalog.Core.SharedKernel;
using Catalog.Domain.Entities.ProductAggregate;
using System.Collections.Generic;
using System.Linq;

namespace Catalog.Application.Products.GetProductById;

public class GetProductByIdQueryResponse(Product product) : IResponse
{
    public string Name { get; set; } = product.Name;
    public string Description { get; set; } = product.Description;
    public string Category { get; set; } = product.Category;
    public decimal Price { get; set; } = product.Price;
    public int StockQuantity { get; set; } = product.StockQuantity;
    public string SKU { get; set; } = product.SKU;
    public string Brand { get; set; } = product.Brand;
    public List<ImageDto> Imgages { get; set; } = product.Images.Select(i => new ImageDto { Url = i.Url }).ToList();
}
