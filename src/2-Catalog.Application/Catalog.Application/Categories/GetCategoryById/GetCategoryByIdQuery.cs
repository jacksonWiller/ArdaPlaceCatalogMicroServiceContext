using System;
using Ardalis.Result;
using MediatR;
namespace Catalog.Application.Categories.GetCategoryById;
public class GetCategoryByIdQuery(Guid id) : IRequest<Result<GetCategoryByIdQueryResponse>>
{
    public Guid Id { get; set; } = id;
}
