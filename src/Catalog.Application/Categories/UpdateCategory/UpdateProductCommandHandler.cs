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

namespace Catalog.Application.Categories.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<UpdateCategoryResponse>>
{
    public readonly ICatalogDbContext _context;
    private readonly IValidator<UpdateCategoryCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(ICatalogDbContext context, IValidator<UpdateCategoryCommand> validator, IUnitOfWork unitOfWork)
    {
        _context = context;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UpdateCategoryResponse>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<UpdateCategoryResponse>.Invalid(validationResult.AsErrors());
        }

        var category = await _context.Set<Category>().Where(p => p.Id == request.Id && p._isDeleted).SingleOrDefaultAsync(cancellationToken);
        if (category == null)
            return Result.NotFound($"No Category found by Id: {request.Id}");

        category.Update(
            request.Name,
            request.Description
        );

        _context.Set<Category>().Update(category);

        await _unitOfWork.SaveChangesAsync();

        var response = new UpdateCategoryResponse(category.Id);
        return Result<UpdateCategoryResponse>.Success(response, "Category created successfully.");
    }

}
