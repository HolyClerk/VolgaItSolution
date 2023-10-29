using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Simbir.Infrastructure;

namespace Simbir.RestApi.Extensions;

public static class AuthExtensions
{
    /// <summary>
    /// Добавляет аутентификацию по JWT-токенам.
    /// Параметры валидации находятся в ParametersHelper (проект Simbir.Infrastructure)
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var auth = services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        auth.AddJwtBearer(options =>
        {
            options.TokenValidationParameters =
                ParametersHelper.GetTokenValidationParameters(builder.Configuration);
        });

        return services.AddAuthorization(options =>
        {
            options.DefaultPolicy
                = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
        });
    }
}
