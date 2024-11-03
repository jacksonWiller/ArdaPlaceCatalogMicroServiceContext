using FluentValidation;

namespace Catalog.Application.Categories.CreateCategory;
public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        //RuleFor(command => command.Name).NotEmpty().WithMessage("Name is required.");
        //RuleFor(command => command.Description).NotEmpty().WithMessage("Description is required.");
        //RuleFor(command => command.Category).NotEmpty().WithMessage("Category is required.");
        //RuleFor(command => command.Price).GreaterThan(0).WithMessage("Price must be greater than 0.");
        //RuleFor(command => command.StockQuantity).GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");
        //RuleFor(command => command.SKU).NotEmpty().WithMessage("SKU is required.");
        //RuleFor(command => command.Brand).NotEmpty().WithMessage("Brand is required.");
    }
}
