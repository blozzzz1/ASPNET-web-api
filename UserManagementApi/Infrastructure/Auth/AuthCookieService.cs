using Microsoft.Extensions.Options;

namespace UserManagementApi.Infrastructure.Auth;

public interface IAuthCookieService
{
    void AppendAccessToken(HttpResponse response, string token);
    void DeleteAccessToken(HttpResponse response);
}

public class AuthCookieService(
    IOptions<JwtOptions> jwtOptions,
    IHostEnvironment environment) : IAuthCookieService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public void AppendAccessToken(HttpResponse response, string token)
    {
        response.Cookies.Append(_jwtOptions.CookieName, token, CreateCookieOptions(delete: false));
    }

    public void DeleteAccessToken(HttpResponse response)
    {
        response.Cookies.Append(_jwtOptions.CookieName, string.Empty, CreateCookieOptions(delete: true));
    }

    private CookieOptions CreateCookieOptions(bool delete)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = environment.IsProduction(),
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = delete
                ? DateTimeOffset.UnixEpoch
                : DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
            IsEssential = true
        };
    }
}
