namespace Carizon.Infrastructure.Services.AuthServices
{
    public class AuthService(UserManager<ApplicationUser> userManager, IMapper mapper, IOptions<EmailSettings> _emailSettings, IEmailService emailService, ITokenService tokenService, ILogger<AuthService> logger) : IAuthService
    {
        private readonly EmailSettings emailSettings = _emailSettings.Value;

        //Register User
        public async Task<ResultResponse<RegisterResponse>> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser is not null)
            {
                return ResultResponse<RegisterResponse>.Failure(new[] { "Email is Already Exist" });
            }
            var user = mapper.Map<ApplicationUser>(registerDto);
            var createUser = await userManager.CreateAsync(user, registerDto.Password);
            if (!createUser.Succeeded)
            {
                var errors = createUser.Errors.Select(e => e.Description);
                return ResultResponse<RegisterResponse>.Failure(errors);
            }
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"{emailSettings.BaseUrl}/api/authentication/confirm-email?email={UrlEncoder.Default.Encode(user.Email!)}&token={encodedToken}";

            try
            {
                await emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
            }

            logger.LogInformation("User {Email} registered successfully", user.Email);
            var registerResponse = new RegisterResponse
            {
                Email = user.Email!,
                Id = user.Id,
                RequiresEmailConfirmation = true,
                UserName = user.UserName!,
            };
            return ResultResponse<RegisterResponse>.Success(registerResponse, "Registration successful. Please check your email to confirm your account.");
        }

        //ConfirmEmail
        public async Task<ResultResponse<ConfirmEmailResponse>> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto)
        {
            var user = await userManager.FindByEmailAsync(confirmEmailDto.Email);
            if (user == null)
            {
                return ResultResponse<ConfirmEmailResponse>.Failure("User not found.");
            }

            if (user.EmailConfirmed)
            {
                return ResultResponse<ConfirmEmailResponse>.Success("Already Confirmed");
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmEmailDto.Token));
            var result = await userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return ResultResponse<ConfirmEmailResponse>.Failure(string.Join(", ", errors));
            }

            logger.LogInformation("Email confirmed for user {Email}", user.Email);
            return ResultResponse<ConfirmEmailResponse>.Success();
        }


    }
}
