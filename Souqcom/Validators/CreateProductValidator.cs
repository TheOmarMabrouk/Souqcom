using First_core_project.Data;
using First_core_project.DTOs.API;
using FluentValidation;

public class CreateProductValidator : AbstractValidator<ProductCreateDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MinimumLength(3);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");

        RuleFor(x => x.Catid)
            .GreaterThan(0).WithMessage("Category is required");
    }
}