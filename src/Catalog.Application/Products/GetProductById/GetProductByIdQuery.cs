using System;
using Ardalis.Result;
using MediatR;
namespace Catalog.Application.Products.GetProductById;
public class GetProductByIdQuery(Guid id) : IRequest<Result<GetProductByIdQueryResponse>>
{
    public Guid Id { get; set; } = id;
}
