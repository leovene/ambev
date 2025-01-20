using Ambev.Application.Sales.Commands;
using Ambev.Application.Sales.Events;
using Ambev.Application.Sales.Mappings;
using Ambev.Application.Sales.Queries;
using Ambev.Application.Sales.Validators;
using Ambev.Common.Configuration;
using Ambev.Domain.Common.Validation;
using Ambev.Domain.Sales.Creators;
using Ambev.Domain.Sales.Events;
using Ambev.IoC.Common.Interfaces;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.IoC.Sales.ModuleInitializers
{
    public class ApplicationModuleInitializer : IModuleInitializer
    {
        private readonly IConfiguration _configuration;

        public ApplicationModuleInitializer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<SaleItemFactory>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateSaleCommandHandler).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(UpdateSaleCommandHandler).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(DeleteSaleCommandHandler).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(GetAllSalesQueryHandler).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(GetSaleByIdQueryHandler).Assembly);
            });

            services.AddScoped<INotificationHandler<SaleCreatedEvent>, SaleEventHandler>();
            services.AddScoped<INotificationHandler<SaleModifiedEvent>, SaleEventHandler>();
            services.AddScoped<INotificationHandler<SaleDeleteEvent>, SaleEventHandler>();

            services.AddAutoMapper(typeof(SaleProfile).Assembly);

            services.AddValidatorsFromAssemblyContaining<CreateSaleCommandValidator>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssemblyContaining<UpdateSaleCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<DeleteSaleCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<CancelSaleCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<CancelSaleItemsCommandValidator>();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqSettings = _configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>();
                    cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                    {
                        h.Username(rabbitMqSettings.Username);
                        h.Password(rabbitMqSettings.Password);
                    });
                });
            });
        }
    }
}
