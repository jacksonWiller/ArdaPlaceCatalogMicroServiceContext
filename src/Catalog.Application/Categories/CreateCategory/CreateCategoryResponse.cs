using System;
using Catalog.Core.SharedKernel;

namespace Catalog.Application.Categories.CreateCategory;

public class CreateCategoryResponse(Guid id) : IResponse
{
    public Guid Id { get; } = id;

}
