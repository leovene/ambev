using Ambev.Domain.Sales.Interfaces.Repositories;
using MediatR;

namespace Ambev.Application.Sales.Commands
{
    public class MarkAsSyncedCommandHandler : IRequestHandler<MarkAsSyncedCommand>
    {
        private readonly ISaleQueryRepository _saleQueryRepository;
        private readonly ISaleCommandRepository _saleCommandRepository;
        private readonly Serilog.ILogger _logger;

        public MarkAsSyncedCommandHandler(ISaleQueryRepository saleQueryRepository, ISaleCommandRepository saleCommandRepository, Serilog.ILogger logger)
        {
            _saleQueryRepository = saleQueryRepository;
            _saleCommandRepository = saleCommandRepository;
            _logger = logger;
        }

        public async Task Handle(MarkAsSyncedCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Received request to sync sale with ID: {SaleId}", request.Id);

            var sale = await _saleQueryRepository.GetByIdAsync("Sales", request.Id);
            if (sale == null)
            {
                _logger.Warning("Sale with ID: {SaleId} not found.", request.Id);
                throw new KeyNotFoundException($"Sale with ID {request.Id} not found.");
            }

            _logger.Information("Marking sale with ID: {SaleId} as synced.", sale.Id);

            sale.IsSynced = true;

            try
            {
                await _saleCommandRepository.UpdateAsync(sale);
                _logger.Information("Sale with ID: {SaleId} successfully updated as synced.", sale.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating sale with ID: {SaleId}.", sale.Id);
                throw;
            }
        }
    }
}
