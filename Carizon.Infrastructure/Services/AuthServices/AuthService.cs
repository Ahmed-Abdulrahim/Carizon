namespace Carizon.Infrastructure.Services.AuthServices
{
    public class AuthService(IUnitOfWork unitOfWork, SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager, IMapper mapper, IOptions<EmailSettings> _emailSettings,
        IEmailService emailService, ITokenService tokenService, ILogger<AuthService> logger, IAccountService accountService) : IAuthService
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
            var addRole = await userManager.AddToRoleAsync(user, "User");

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

        //Login User
        public async Task<ResultResponse<LoginResponse>> LoginAsync(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user is null) return ResultResponse<LoginResponse>.Failure("Invalid email or password.");

            if (!user.EmailConfirmed) return ResultResponse<LoginResponse>.Failure("Email is not confirmed.");

            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                return ResultResponse<LoginResponse>.Failure("Invalid email or password.");
            }

            return await GenerateAuthTokensAsync(user);
        }

        //Refresh Token
        public async Task<ResultResponse<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            // Check if Access Token Expire
            var principal = tokenService.GetPrincipalFromExpiredToken(refreshTokenDto.Token);
            if (principal is null) return ResultResponse<RefreshTokenResponse>.Failure("Invalid access token.");

            // Check if Token Related to User
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return ResultResponse<RefreshTokenResponse>.Failure("Invalid access token.");
            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return ResultResponse<RefreshTokenResponse>.Failure("User not found.");
            // get Validate Refresh Token From Db
            var specRefreshToken = new RefreshTokenSpecification(rt => rt.Token == refreshTokenDto.RefreshToken && rt.UserId == user.Id && rt.ExpiresAt >= DateTime.UtcNow && rt.IsRevoked == false);
            var getRefreshToken = await unitOfWork.Repository<RefreshToken>().GetByIdSpecTrackedAsync(specRefreshToken);
            if (getRefreshToken is null) return ResultResponse<RefreshTokenResponse>.Failure("Invalid refresh token.");
            var roles = await userManager.GetRolesAsync(user);

            //Generate new Access && Refresh Token
            var newAccessToken = await tokenService.GenerateAccesToken(user, roles);
            var newRefeshToken = tokenService.GenerateRefreshtoken();
            var refreshTokenrow = new RefreshToken
            {
                Token = newRefeshToken,
                ExpiresAt = tokenService.GetRefreshTokenExpiration(),
                UserId = user.Id,
            };
            //Revoke the Old RefreshToken
            getRefreshToken.IsRevoked = true;
            getRefreshToken.RevokedAt = DateTime.UtcNow;
            unitOfWork.Repository<RefreshToken>().Update(getRefreshToken);
            await unitOfWork.Repository<RefreshToken>().AddAsync(refreshTokenrow);
            await unitOfWork.CommitAsync();
            var refreshTokenResponse = new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefeshToken,
                AccessTokenExpiration = tokenService.GetAccessTokenExpiration(),
                RefreshTokenExpiration = tokenService.GetRefreshTokenExpiration(),

            };
            return ResultResponse<RefreshTokenResponse>.Success(refreshTokenResponse);
        }

        //Forget Password
        public async Task<ResultResponse<string>> ForgetPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user is null || !user.EmailConfirmed)
            {
                return ResultResponse<string>.Success();
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var resetLink = $"{emailSettings.BaseUrl}/api/Authentication/reset-password?email={UrlEncoder.Default.Encode(user.Email!)}&token={encodedToken}";

            try
            {
                await emailService.SendPasswordResetAsync(user.Email!, resetLink);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
            }
            return ResultResponse<string>.Success();
        }

        //ResetPassword
        public async Task<ResultResponse<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return ResultResponse<string>.Failure("Invalid request.");
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordDto.Token));
            var result = await userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return ResultResponse<string>.Failure(string.Join(", ", errors));
            }

            await accountService.LogoutAsync(user.Id.ToString());

            logger.LogInformation("Password reset successfully for user {Email}", user.Email);
            return ResultResponse<string>.Success();
        }


        //Private Methods
        private async Task<ResultResponse<LoginResponse>> GenerateAuthTokensAsync(ApplicationUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            var accessToken = await tokenService.GenerateAccesToken(user, roles);
            var refreshToken = tokenService.GenerateRefreshtoken();


            var refreshTokenrow = new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = tokenService.GetRefreshTokenExpiration(),
                UserId = user.Id,
            };
            await unitOfWork.Repository<RefreshToken>().AddAsync(refreshTokenrow);
            await unitOfWork.CommitAsync();

            var loginResponse = new LoginResponse
            {
                AccessToken = accessToken,
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RefreshToken = refreshToken,
                AccessTokenExpiration = tokenService.GetAccessTokenExpiration(),
                RefreshTokenExpiration = tokenService.GetRefreshTokenExpiration(),
                Roles = roles.ToList()

            };
            return ResultResponse<LoginResponse>.Success(loginResponse, "Login Successfully");
        }


    }
}
