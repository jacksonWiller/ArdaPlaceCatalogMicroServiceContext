using System;
using Catalog.Core.SharedKernel;

namespace Catalog.Application.Categories.DeleteCategory;

public class DeleteCategoryResponse(Guid id) : IResponse
{
    public Guid Id { get; } = id;

}
