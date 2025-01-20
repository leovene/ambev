using Ambev.Application.Sales.Commands;
using Ambev.Application.Sales.Queries;
using AutoMapper;
using MediatR;

namespace Ambev.Worker.Sales
{
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Worker> _logger;

        public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Starting sync process...");

                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                var querySales = new GetUnsyncedSalesQuery();

                var querySalesResult = await mediator.Send(querySales, stoppingToken);

                foreach (var sale in querySalesResult)
                {
                    try
                    {
                        var markAsSyncedCommand = new MarkAsSyncedCommand { Id = sale.Id };

                        if (sale.IsCancelled)
                        {
                            await mediator.Send(markAsSyncedCommand, stoppingToken);
                            continue;
                        }

                        var saleDocument = mapper.Map<CreateSaleDocumentCommand>(sale);

                        await mediator.Send(saleDocument, stoppingToken);
                        await mediator.Send(markAsSyncedCommand, stoppingToken);

                        _logger.LogInformation("Sale with ID {SaleId} synced successfully.", sale.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error syncing sale with ID {SaleId}.", sale.Id);
                    }
                }

                _logger.LogInformation("Sync process completed. Retrying in 5 minutes...");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
