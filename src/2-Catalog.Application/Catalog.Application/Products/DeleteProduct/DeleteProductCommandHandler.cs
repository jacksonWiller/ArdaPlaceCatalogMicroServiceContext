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

namespace Catalog.Application.Products.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<DeleteProductResponse>>
{
    public readonly ICatalogDbContext _context;
    private readonly IValidator<DeleteProductCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(ICatalogDbContext context, IValidator<DeleteProductCommand> validator, IUnitOfWork unitOfWork)
    {
        _context = context;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DeleteProductResponse>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<DeleteProductResponse>.Invalid(validationResult.AsErrors());
        }

        var product = await _context.Set<Product>().Where(p => p.Id == request.Id && p._isDeleted).SingleOrDefaultAsync(cancellationToken);
        if (product == null)
            return Result.NotFound($"No product found by Id: {request.Id}");

        product.Delete(); 

        _context.Set<Product>().Update(product);

        await _unitOfWork.SaveChangesAsync();

        var response = new DeleteProductResponse(product.Id);
        return Result<DeleteProductResponse>.Success(response, "Product created successfully.");
    }

}
