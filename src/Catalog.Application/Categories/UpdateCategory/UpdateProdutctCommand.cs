using System;
using Ardalis.Result;
using MediatR;

namespace Catalog.Application.Categories.UpdateCategory;

public class UpdateCategoryCommand : IRequest<Result<UpdateCategoryResponse>>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string SKU { get; set; }
    public string Brand { get; set; }
}