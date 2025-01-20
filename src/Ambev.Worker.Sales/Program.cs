using Ambev.IoC.Common.Interfaces;
using Ambev.IoC.Common.ModuleInitializers;
using Ambev.IoC.Sales.ModuleInitializers;
using Ambev.Worker.Sales;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var initializers = new IModuleInitializer[]
{
    new ConfigurationModuleInitializer(builder.Configuration),
    new PersistenceModuleInitializer(builder.Configuration),
    new ApplicationModuleInitializer(builder.Configuration)
};

foreach (var initializer in initializers)
{
    initializer.Initialize(builder.Services);
}

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

var host = builder.Build();

host.Run();
