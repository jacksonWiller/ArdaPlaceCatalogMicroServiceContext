using System;
using Ardalis.Result;
using MediatR;

namespace Catalog.Application.Products.DeleteProduct;

public class DeleteProductCommand(Guid id) : IRequest<Result<DeleteProductResponse>>
{
    public Guid Id { get; set; } = id;
}