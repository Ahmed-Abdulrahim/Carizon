namespace Carizon.Application.Interfaces.AuthInterface
{
    public interface IAuthService
    {
        Task<ResultResponse<RegisterResponse>> RegisterAsync(RegisterDto registerDto);
        Task<ResultResponse<ConfirmEmailResponse>> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto);
    }
}
