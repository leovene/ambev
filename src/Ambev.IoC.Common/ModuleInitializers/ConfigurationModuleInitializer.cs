using Ambev.Common.Configuration;
using Ambev.IoC.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace Ambev.IoC.Common.ModuleInitializers
{
    public class ConfigurationModuleInitializer : IModuleInitializer
    {
        private readonly IConfiguration _configuration;

        public ConfigurationModuleInitializer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Initialize(IServiceCollection services)
        {
            services.Configure<ConnectionStrings>(_configuration.GetSection(nameof(ConnectionStrings)));

            var seqUrl = _configuration.GetValue<string>("Serilog:WriteTo:1:Args:serverUrl");

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(seqUrl ?? "http://localhost:5341")
                .CreateLogger();

            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<ConnectionStrings>>().Value);

            services.AddSingleton(Log.Logger);
        }
    }
}
