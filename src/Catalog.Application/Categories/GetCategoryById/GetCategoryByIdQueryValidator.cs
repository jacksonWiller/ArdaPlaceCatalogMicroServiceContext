using Catalog.Application.Categories.GetCategoryById;
using FluentValidation;

namespace Catalog.Application.Categories.GetCategoryById;
public class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdQueryValidator()
    {
        RuleFor(command => command.Id)
           .NotEmpty();
    }
}
