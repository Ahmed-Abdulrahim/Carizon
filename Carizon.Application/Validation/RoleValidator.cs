namespace Carizon.Application.Validation
{
    public class RoleValidator : AbstractValidator<RoleDto>
    {
        public RoleValidator()
        {
            RuleFor(c => c.Name).NotEmpty().WithMessage("Rol is Required")
                .MinimumLength(2).MaximumLength(50).WithMessage("Role name must be between 2 and 50 characters.");

            RuleFor(c => c.Description).NotEmpty().WithMessage("Descritpion is Required")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
