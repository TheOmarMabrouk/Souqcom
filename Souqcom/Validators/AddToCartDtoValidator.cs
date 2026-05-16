using FluentValidation;

public class AddToCartDtoValidator : AbstractValidator<int> // بنعمل فاليديشن للـ ProductId
{
    public AddToCartDtoValidator()
    {
        RuleFor(id => id)
            .GreaterThan(0).WithMessage("رقم المنتج غير صحيح");
    }
}