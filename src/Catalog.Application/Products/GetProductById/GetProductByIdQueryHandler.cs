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

namespace Catalog.Application.Products.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<GetProductByIdQueryResponse>>
{
    public readonly ICatalogDbContext _context;
    private readonly IValidator<GetProductByIdQuery> _validator;

    public GetProductByIdQueryHandler(ICatalogDbContext context, IValidator<GetProductByIdQuery> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<GetProductByIdQueryResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<GetProductByIdQueryResponse>.Invalid(validationResult.AsErrors());
        }

        var product = await _context.Set<Product>().Where(p => p.Id == request.Id && p._isDeleted == false).SingleOrDefaultAsync(cancellationToken);
        if (product == null)
            return Result.NotFound($"No product found by Id: {request.Id}");

        var response = new GetProductByIdQueryResponse(product);
        return Result<GetProductByIdQueryResponse>.Success(response, "Product created successfully.");
    }

}