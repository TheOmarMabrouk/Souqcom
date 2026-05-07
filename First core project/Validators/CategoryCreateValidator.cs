using FluentValidation;
using First_core_project.DTOs.API;

namespace First_core_project.Validators
{
    public class CategoryCreateValidator : AbstractValidator<CategoryCreateDto>
    {
        public CategoryCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("اسم القسم مطلوب")
                .MinimumLength(3).WithMessage("اسم القسم لازم يكون أكتر من 3 حروف")
                .MaximumLength(50).WithMessage("اسم القسم كبير جداً");

            RuleFor(x => x.Description)
                .MaximumLength(200).WithMessage("الوصف ما ينفعش يزيد عن 200 حرف");
        }
    }
}