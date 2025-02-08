using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Catalog.Application.Products.Dtos;
using Catalog.Domain.DataContext;
using Catalog.Domain.Entities.ProductAggregate;
using FluentValidation;
using Fop;
using Fop.FopExpression;
using MediatR;

namespace Catalog.Application.Products.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<GetAllProductsQueryResponse>>
{
    public readonly ICatalogDbContext _context;
    private readonly IValidator<GetAllProductsQuery> _validator;

    public GetAllProductsQueryHandler(ICatalogDbContext context, IValidator<GetAllProductsQuery> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<GetAllProductsQueryResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<GetAllProductsQueryResponse>.Invalid(validationResult.AsErrors());
        }

        var fopRequest = FopExpressionBuilder<Product>.Build(request.Filter, request.Order, request.PageNumber, request.PageSize);

        var (filteredProducts, totalCount) = _context.Set<Product>().ApplyFop(fopRequest);

        var pagedInfo = new PagedInfo(
                                        request.PageNumber,
                                        request.PageSize,
                                        (int)Math.Ceiling((double)totalCount / request.PageSize),
                                        totalCount
                                        );

        var response = new GetAllProductsQueryResponse
        {
            PagedInfo = pagedInfo,
            Products = [.. filteredProducts.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                SKU = p.SKU,
                Brand = p.Brand,
                Images = p.Images.Select(img => new ImageDto { Url = img.Url }).ToList()
            })]
        };

        return Result<GetAllProductsQueryResponse>.Success(response, "Products retrieved successfully.");
    }

    public class Paged<T>(PagedInfo pagedInfo, T value)
    {
        [JsonInclude]
        public PagedInfo PagedInfo { get; init; } = pagedInfo;
        public T Value { get; init; } = value;
    }
}