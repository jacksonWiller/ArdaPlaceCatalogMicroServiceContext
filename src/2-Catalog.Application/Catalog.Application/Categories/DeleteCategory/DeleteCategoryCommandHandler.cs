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

namespace Catalog.Application.Categories.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<DeleteCategoryResponse>>
{
    public readonly ICatalogDbContext _context;
    private readonly IValidator<DeleteCategoryCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(ICatalogDbContext context, IValidator<DeleteCategoryCommand> validator, IUnitOfWork unitOfWork)
    {
        _context = context;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DeleteCategoryResponse>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<DeleteCategoryResponse>.Invalid(validationResult.AsErrors());
        }

        var category = await _context.Set<Category>().Where(p => p.Id == request.Id && p._isDeleted).SingleOrDefaultAsync(cancellationToken);
        if (category == null)
            return Result.NotFound($"No Category found by Id: {request.Id}");

        category.Delete(); 

        _context.Set<Category>().Update(category);

        await _unitOfWork.SaveChangesAsync();

        var response = new DeleteCategoryResponse(category.Id);
        return Result<DeleteCategoryResponse>.Success(response, "Category created successfully.");
    }

}
