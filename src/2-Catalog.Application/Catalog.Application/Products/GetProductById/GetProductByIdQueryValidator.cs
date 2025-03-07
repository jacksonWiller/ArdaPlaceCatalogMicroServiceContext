using FluentValidation;

namespace Catalog.Application.Products.GetProductById;
public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator()
    {
        RuleFor(command => command.Id)
           .NotEmpty();
    }
}
