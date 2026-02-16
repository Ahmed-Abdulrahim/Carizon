namespace Carizon.Application.Validation
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(r => r.UserName).NotEmpty().WithMessage("user Name must not be Empty")
                .MaximumLength(15).MinimumLength(4).WithMessage("userName must between 4 and 15");

            RuleFor(r => r.Email).NotEmpty().WithMessage("Email must not be Empty")
                .EmailAddress().WithMessage("Invalid Email format")
                .MaximumLength(256).WithMessage("Email Cannot Exceed 100 Character ");

            RuleFor(r => r.Password).NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).MaximumLength(100).WithMessage("Password must be between 8 and 100 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number and one special character");

            RuleFor(r => r.ConfirmPassword).NotEmpty().WithMessage("Confirm Password is required")
                .Equal(r => r.Password).WithMessage("Password and confirmation password do not match");
        }
    }
}
