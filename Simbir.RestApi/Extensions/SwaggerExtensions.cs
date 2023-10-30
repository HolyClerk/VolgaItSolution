using Microsoft.OpenApi.Models;

namespace Simbir.RestApi.Extensions;

public static class SwaggerExtensions
{
    /// <summary>
    /// Добавляет в Swagger возможность авторизации по JWT токену.
    /// </summary>
    public static IServiceCollection AddModifiedSwaggerGen(this IServiceCollection services)
    {
        return services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "JWTToken_Auth_API",
                Version = "v1"
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Сервис JWT авторизации использует Bearer схему. \r\n\r\n Введите 'Bearer' [пробел] и после введите ваш JWT-токен. \r\n\r\nПример: \"Bearer 1safsfsdfdfd\"",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}