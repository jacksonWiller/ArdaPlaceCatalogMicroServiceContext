using System;
using Catalog.Core.SharedKernel;

namespace Catalog.Application.Products.DeleteProduct;

public class DeleteProductResponse(Guid id) : IResponse
{
    public Guid Id { get; } = id;

}
