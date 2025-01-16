using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Catalog.Application.Categories.Dtos;
using Catalog.Domain.DataContext;
using Catalog.Domain.Entities.ProductAggregate;
using FluentValidation;
using Fop;
using Fop.FopExpression;
using MediatR;
using ProductDto = Catalog.Application.Categories.Dtos.ProductDto;

namespace Catalog.Application.Categories.GetAllCategorys;

public class GetAllCategorysQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<GetAllCategorysQueryResponse>>
{
    public readonly ICatalogDbContext _context;
    private readonly IValidator<GetAllCategoriesQuery> _validator;

    public GetAllCategorysQueryHandler(ICatalogDbContext context, IValidator<GetAllCategoriesQuery> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<GetAllCategorysQueryResponse>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<GetAllCategorysQueryResponse>.Invalid(validationResult.AsErrors());
        }
        var fopRequest = FopExpressionBuilder<Category>.Build(request.Filter, request.Order, request.PageNumber, request.PageSize);
 
        var (filteredCategorys, totalCount) = _context.Set<Category>().ApplyFop(fopRequest) ;

        var pagedInfo = new PagedInfo(
                                        request.PageNumber,
                                        request.PageSize,
                                        (int)Math.Ceiling((double)totalCount / request.PageSize),
                                        totalCount
                                        );

        var response = new GetAllCategorysQueryResponse
        {
            PagedInfo = pagedInfo,
            Categorys = [.. filteredCategorys.Select(p => new CategoryDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                //Products = p.Products.Select(p => new ProductDto
                //{
                //    Id = p.Id,
                //    Name = p.Name,
                //}).ToList()
            })]
        };

        return Result<GetAllCategorysQueryResponse>.Success(response, "Categorys retrieved successfully.");
    }

    public class Paged<T>(PagedInfo pagedInfo, T value)
    {
        [JsonInclude]
        public PagedInfo PagedInfo { get; init; } = pagedInfo;
        public T Value { get; init; } = value;
    }
}