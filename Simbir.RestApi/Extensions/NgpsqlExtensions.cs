using Microsoft.EntityFrameworkCore;
using Simbir.Application.Other;
using Simbir.Infrastructure.Context;

namespace Simbir.RestApi.Extensions;

public static class NpgsqlExtensions
{
    /// <summary>
    /// Добавляет возможность подключения к Postgres.
    /// Добавляет контекст БД.
    /// </summary>
    public static IServiceCollection AddPostgresDataBaseContext(this IServiceCollection services,
        WebApplicationBuilder builder)
    {
        var connection =
            builder.Configuration.GetConnectionString("PgConnection") ??
            throw new Exception("Невозможно найти строку \"PgConnection\" в appsettings.json");

        return services.AddDbContext<IDbContext, ApplicationContext>(options
            => options.UseNpgsql(connection));
    }
}
