using System;
using Catalog.Core.SharedKernel;

namespace Catalog.Application.Products.CreateProduct;

public class CreateProductResponse(Guid id) : IResponse
{
    public Guid Id { get; } = id;

}
