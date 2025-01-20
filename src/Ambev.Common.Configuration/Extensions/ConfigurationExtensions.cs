using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ambev.Common.Configuration.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConnectionStrings>(configuration);

            services.AddSingleton(sp =>
                sp.GetRequiredService<IOptions<ConnectionStrings>>().Value);

            return services;
        }
    }
}
