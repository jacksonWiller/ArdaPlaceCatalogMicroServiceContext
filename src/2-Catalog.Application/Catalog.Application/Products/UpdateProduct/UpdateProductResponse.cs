using System;
using Catalog.Core.SharedKernel;

namespace Catalog.Application.Products.UpdateProduct;

public class UpdateProductResponse(Guid id) : IResponse
{
    public Guid Id { get; } = id;

}
