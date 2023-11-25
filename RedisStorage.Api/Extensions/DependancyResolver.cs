using RedisStorage.Application.Services;
using RedisStorage.Core.Abstractions;

namespace RedisStorage.Api.Extensions
{
    public static class DependancyResolver
    {
        public static void ResolveDependacy(this IServiceCollection services)
        {
            services.AddScoped<IRedisService, RedisService>();
        }
    }
}
