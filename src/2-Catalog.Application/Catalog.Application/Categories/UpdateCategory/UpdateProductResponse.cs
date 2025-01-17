using System;
using Catalog.Core.SharedKernel;

namespace Catalog.Application.Categories.UpdateCategory;

public class UpdateCategoryResponse(Guid id) : IResponse
{
    public Guid Id { get; } = id;

}
