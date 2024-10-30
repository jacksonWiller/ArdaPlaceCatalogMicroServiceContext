using System.Collections.Generic;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Http;
namespace Catalog.Application.Products.CreateProduct;
public class CreateProductCommand : IRequest<Result<CreateProductResponse>>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string SKU { get; set; }
    public string Brand { get; set; }
    public List<IFormFile> Files { get; set; } = null;
}
