using Ambev.Domain.Sales.Events;
using Ambev.Domain.Sales.Interfaces.Repositories;
using MediatR;

namespace Ambev.Application.Sales.Commands
{
    public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand>
    {
        private readonly ISaleQueryRepository _saleQueryRepository;
        private readonly ISaleCommandRepository _saleCommandRepository;
        private readonly IMediator _mediator;
        private readonly Serilog.ILogger _logger;

        public CancelSaleCommandHandler(ISaleQueryRepository saleQueryRepository, ISaleCommandRepository saleCommandRepository, IMediator mediator, Serilog.ILogger logger)
        {
            _saleQueryRepository = saleQueryRepository;
            _saleCommandRepository = saleCommandRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(CancelSaleCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Received request to cancel sale with ID: {SaleId}", request.Id);

            var sale = await _saleQueryRepository.GetByIdAsync("Sales", request.Id);
            if (sale == null)
            {
                _logger.Warning("Sale with ID: {SaleId} not found.", request.Id);
                throw new KeyNotFoundException($"Sale with ID {request.Id} not found.");
            }

            _logger.Information("Marking sale with ID: {SaleId} as cancelled.", sale.Id);

            sale.IsCancelled = true;

            try
            {
                await _saleCommandRepository.UpdateAsync(sale);
                _logger.Information("Sale with ID: {SaleId} successfully updated as cancelled.", sale.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating sale with ID: {SaleId}.", sale.Id);
                throw;
            }

            var saleCancelledEvent = new SaleCancelEvent(sale.Id, DateTime.UtcNow);

            _logger.Information("Publishing SaleCancelEvent for sale with ID: {SaleId}.", sale.Id);
            try
            {
                await _mediator.Publish(saleCancelledEvent, cancellationToken);
                _logger.Information("SaleCancelEvent for sale with ID: {SaleId} successfully published.", sale.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while publishing SaleCancelEvent for sale with ID: {SaleId}.", sale.Id);
                throw;
            }
        }
    }
}
