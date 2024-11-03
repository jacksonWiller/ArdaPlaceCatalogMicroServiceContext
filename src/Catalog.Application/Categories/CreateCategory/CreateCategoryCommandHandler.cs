using System.Threading.Tasks;
using System.Threading;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Catalog.Core.SharedKernel;
using Catalog.Domain.DataContext;
using Catalog.Domain.Entities.ProductAggregate;
using FluentValidation;
using MediatR;

namespace Catalog.Application.Categories.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
{
    private readonly ICatalogDbContext _context;
    private readonly IValidator<CreateCategoryCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(
        ICatalogDbContext context,
        IValidator<CreateCategoryCommand> validator,
        IUnitOfWork unitOfWork
    )
    {
        _context = context;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateCategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Validação do comando
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<CreateCategoryResponse>.Invalid(validationResult.AsErrors());
        }

        var product = new Category(
            request.Name,
            request.Description
        );

        _context.Set<Category>().Add(product);
        await _unitOfWork.SaveChangesAsync();

        var response = new CreateCategoryResponse(product.Id);
        return Result<CreateCategoryResponse>.Success(response, "Product created successfully.");
    }
}