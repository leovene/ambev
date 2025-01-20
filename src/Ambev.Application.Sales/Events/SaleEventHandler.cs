using Ambev.Domain.Sales.Events;
using MassTransit;
using MediatR;

namespace Ambev.Application.Sales.Events
{
    public class SaleEventHandler : INotificationHandler<SaleCreatedEvent>, INotificationHandler<SaleModifiedEvent>, INotificationHandler<SaleDeleteEvent>, INotificationHandler<SaleCancelEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly Serilog.ILogger _logger;

        public SaleEventHandler(IPublishEndpoint publishEndpoint, Serilog.ILogger logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.Information("Publishing SaleCreatedEvent for SaleNumber: {SaleNumber}", notification.SaleNumber);

            await _publishEndpoint.Publish(notification, cancellationToken);

            _logger.Information("SaleCreatedEvent for SaleNumber: {SaleNumber} published successfully", notification.SaleNumber);
        }

        public async Task Handle(SaleDeleteEvent notification, CancellationToken cancellationToken)
        {
            _logger.Information("Publishing SaleDeleteEvent for SaleId: {SaleId}", notification.Id);

            await _publishEndpoint.Publish(notification, cancellationToken);

            _logger.Information("SaleDeleteEvent for SaleId: {SaleId} published successfully", notification.Id);
        }

        public async Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
        {
            _logger.Information("Publishing SaleModifiedEvent for SaleId: {SaleId}", notification.Id);

            await _publishEndpoint.Publish(notification, cancellationToken);

            _logger.Information("SaleModifiedEvent for SaleId: {SaleId} published successfully", notification.Id);
        }

        public async Task Handle(SaleCancelEvent notification, CancellationToken cancellationToken)
        {
            _logger.Information("Publishing SaleCancelEvent for SaleId: {SaleId}", notification.Id);

            await _publishEndpoint.Publish(notification, cancellationToken);

            _logger.Information("SaleCancelEvent for SaleId: {SaleId} published successfully", notification.Id);
        }
    }
}
