using System;
using Ardalis.Result;
using MediatR;

namespace Catalog.Application.Categories.UpdateCategory;

public class UpdateCategoryCommand : IRequest<Result<UpdateCategoryResponse>>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}