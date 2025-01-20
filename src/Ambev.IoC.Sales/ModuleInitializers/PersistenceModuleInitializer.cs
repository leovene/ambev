using Ambev.Common.Configuration;
using Ambev.Domain.Sales.Interfaces.Repositories;
using Ambev.IoC.Common.Interfaces;
using Ambev.Persistence.Common.Contexts;
using Ambev.Persistence.Sales.Commands;
using Ambev.Persistence.Sales.Contexts;
using Ambev.Persistence.Sales.Documents;
using Ambev.Persistence.Sales.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Npgsql;
using System.Data;

namespace Ambev.IoC.Sales.ModuleInitializers
{
    public class PersistenceModuleInitializer : IModuleInitializer
    {
        private readonly IConfiguration _configuration;

        public PersistenceModuleInitializer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Initialize(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var connectionStrings = serviceProvider.GetRequiredService<ConnectionStrings>();

            services.AddScoped<IDbConnection>(provider =>
                new NpgsqlConnection(connectionStrings.DefaultConnection));

            services.AddDbContext<AppDbContext, SalesDbContext>(options =>
                options.UseNpgsql(connectionStrings.DefaultConnection));

            services.AddScoped<ISaleCommandRepository, SaleCommandRepository>();
            services.AddScoped<ISaleQueryRepository, SaleQueryRepository>();

            var mongoConnectionString = _configuration.GetSection("MongoSettings:ConnectionString").Value;
            var mongoDatabaseName = _configuration.GetSection("MongoSettings:DatabaseName").Value;

            var mongoClient = new MongoClient(mongoConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);

            services.AddSingleton<IMongoDatabase>(mongoDatabase);
            services.AddScoped<ISaleDocumentRepository, SaleDocumentRepository>();
        }
    }
}
