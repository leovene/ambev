using Ambev.Domain.Sales.Events;
using Ambev.Domain.Sales.Interfaces.Repositories;
using MediatR;

namespace Ambev.Application.Sales.Commands
{
    public class DeleteSaleCommandHandler : IRequestHandler<DeleteSaleCommand>
    {
        private readonly ISaleQueryRepository _saleQueryRepository;
        private readonly ISaleCommandRepository _saleCommandRepository;
        private readonly IMediator _mediator;
        private readonly Serilog.ILogger _logger;

        public DeleteSaleCommandHandler(ISaleQueryRepository saleQueryRepository, ISaleCommandRepository saleCommandRepository, IMediator mediator, Serilog.ILogger logger)
        {
            _saleQueryRepository = saleQueryRepository;
            _saleCommandRepository = saleCommandRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Received request to delete sale with ID: {SaleId}", request.Id);

            var sale = await _saleQueryRepository.GetByIdAsync("Sales", request.Id);
            if (sale == null)
            {
                _logger.Warning("Sale with ID: {SaleId} not found.", request.Id);
                throw new KeyNotFoundException($"Sale with ID {request.Id} not found.");
            }

            _logger.Information("Deleting sale with ID: {SaleId}.", sale.Id);

            try
            {
                await _saleCommandRepository.DeleteAsync(sale);
                _logger.Information("Sale with ID: {SaleId} successfully deleted.", sale.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while deleting sale with ID: {SaleId}.", sale.Id);
                throw;
            }

            var saleDeletedEvent = new SaleDeleteEvent(sale.Id, DateTime.UtcNow);

            try
            {
                await _mediator.Publish(saleDeletedEvent, cancellationToken);
                _logger.Information("SaleDeleteEvent for sale with ID: {SaleId} successfully published.", sale.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while publishing SaleDeleteEvent for sale with ID: {SaleId}.", sale.Id);
                throw;
            }
        }
    }
}
