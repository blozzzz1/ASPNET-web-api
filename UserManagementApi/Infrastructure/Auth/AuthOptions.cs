namespace UserManagementApi.Infrastructure.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "UserManagementApi";
    public string Audience { get; set; } = "UserManagementApi";
    public string Key { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
    public string CookieName { get; set; } = "access_token";
}

public class CorsOptions
{
    public const string SectionName = "Cors";

    public string[] AllowedOrigins { get; set; } =
    [
        "http://localhost:3000",
        "http://localhost:5173",
        "http://localhost:4200"
    ];
}
