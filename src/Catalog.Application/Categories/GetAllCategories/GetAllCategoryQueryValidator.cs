using FluentValidation;

namespace Catalog.Application.Categories.GetAllCategorys;
public class GetAllCategorysQueryValidator : AbstractValidator<GetAllCategoriesQuery>
{
    public GetAllCategorysQueryValidator()
    {
        //RuleFor(command => command.Id)
        //   .NotEmpty();
    }
}
