namespace Carizon.Application.Interfaces.AuthInterface
{
    public interface ITokenService
    {
        Task<string> GenerateAccesToken(ApplicationUser applicationUser, IList<string> roles);
        string GenerateRefreshtoken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        DateTime GetAccessTokenExpiration();
        DateTime GetRefreshTokenExpiration();
    }
}
