using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Catalog.Core.SharedKernel;
using Catalog.Domain.DataContext;
using Catalog.Domain.Entities.ProductAggregate;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Products.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<UpdateProductResponse>>
{
    public readonly ICatalogDbContext _context;
    private readonly IValidator<UpdateProductCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(ICatalogDbContext context, IValidator<UpdateProductCommand> validator, IUnitOfWork unitOfWork)
    {
        _context = context;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UpdateProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<UpdateProductResponse>.Invalid(validationResult.AsErrors());
        }

        var product = await _context.Set<Product>().Where(p => p.Id == request.Id && p._isDeleted).SingleOrDefaultAsync(cancellationToken);
        if (product == null)
            return Result.NotFound($"No product found by Id: {request.Id}");

        product.Update(
            request.Name,
            request.Category,
            request.Price,
            request.StockQuantity,
            request.SKU,
            request.Brand
        );

        _context.Set<Product>().Update(product);

        await _unitOfWork.SaveChangesAsync();

        var response = new UpdateProductResponse(product.Id);
        return Result<UpdateProductResponse>.Success(response, "Product created successfully.");
    }

}
