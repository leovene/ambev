using Ambev.Domain.Sales.Events;
using Ambev.Domain.Sales.Interfaces.Repositories;
using MediatR;

namespace Ambev.Application.Sales.Commands
{
    public class CancelSaleItemsCommandHandler : IRequestHandler<CancelSaleItemsCommand>
    {
        private readonly ISaleQueryRepository _saleQueryRepository;
        private readonly ISaleCommandRepository _saleCommandRepository;
        private readonly IMediator _mediator;
        private readonly Serilog.ILogger _logger;

        public CancelSaleItemsCommandHandler(ISaleQueryRepository saleQueryRepository, ISaleCommandRepository saleCommandRepository, IMediator mediator, Serilog.ILogger logger)
        {
            _saleQueryRepository = saleQueryRepository;
            _saleCommandRepository = saleCommandRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(CancelSaleItemsCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Received request to cancel items in sale with ID: {SaleId}. Items: {ItemIds}", request.SaleId, request.ItemIds);

            var sale = await _saleQueryRepository.GetByIdWithItemsAsync(request.SaleId);
            if (sale == null)
            {
                _logger.Warning("Sale with ID: {SaleId} not found.", request.SaleId);
                throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found.");
            }

            foreach (var itemId in request.ItemIds)
            {
                var item = sale.Items.FirstOrDefault(i => i.Id == itemId);
                if (item == null)
                {
                    _logger.Warning("Item with ID: {ItemId} not found in sale {SaleId}.", itemId, request.SaleId);
                    throw new KeyNotFoundException($"Item with ID {itemId} not found in sale {request.SaleId}.");
                }

                item.IsCancelled = true;
            }

            try
            {
                _logger.Information("Updating sale with ID: {SaleId} to reflect cancelled items.", request.SaleId);
                await _saleCommandRepository.UpdateSaleAsync(sale);
                _logger.Information("Sale with ID: {SaleId} successfully updated.", request.SaleId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating sale with ID: {SaleId}.", request.SaleId);
                throw;
            }

            var saleCancelledEvent = new SaleItemCancelEvent(sale.Id, request.ItemIds, DateTime.UtcNow);

            _logger.Information("Publishing SaleItemCancelEvent for sale with ID: {SaleId}. Items: {ItemIds}", sale.Id, request.ItemIds);
            try
            {
                await _mediator.Publish(saleCancelledEvent, cancellationToken);
                _logger.Information("SaleItemCancelEvent for sale with ID: {SaleId} successfully published.", sale.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while publishing SaleItemCancelEvent for sale with ID: {SaleId}.", sale.Id);
                throw;
            }
        }
    }
}
