using FluentValidation;

namespace Catalog.Application.Products.GetAllProducts;
public class GetAllProductsQueryValidator : AbstractValidator<GetAllProductsQuery>
{
    public GetAllProductsQueryValidator()
    {
        //RuleFor(command => command.Id)
        //   .NotEmpty();
    }
}
