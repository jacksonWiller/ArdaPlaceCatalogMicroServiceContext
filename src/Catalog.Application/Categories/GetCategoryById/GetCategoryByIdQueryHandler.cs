using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Catalog.Domain.DataContext;
using Catalog.Domain.Entities.ProductAggregate;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Categories.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<GetCategoryByIdQueryResponse>>
{
    public readonly ICatalogDbContext _context;
    private readonly IValidator<GetCategoryByIdQuery> _validator;

    public GetCategoryByIdQueryHandler(ICatalogDbContext context, IValidator<GetCategoryByIdQuery> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<GetCategoryByIdQueryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<GetCategoryByIdQueryResponse>.Invalid(validationResult.AsErrors());
        }

        var category = await _context.Set<Category>().Where(p => p.Id == request.Id && p._isDeleted == false).SingleOrDefaultAsync(cancellationToken);
        if (category == null)
            return Result.NotFound($"No Category found by Id: {request.Id}");

        var response = new GetCategoryByIdQueryResponse(category);
        return Result<GetCategoryByIdQueryResponse>.Success(response, "Category created successfully.");
    }

}