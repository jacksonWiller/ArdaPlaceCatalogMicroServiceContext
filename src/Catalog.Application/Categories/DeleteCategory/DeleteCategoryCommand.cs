using System;
using Ardalis.Result;
using MediatR;

namespace Catalog.Application.Categories.DeleteCategory;

public class DeleteCategoryCommand(Guid id) : IRequest<Result<DeleteCategoryResponse>>
{
    public Guid Id { get; set; } = id;
}