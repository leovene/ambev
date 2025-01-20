using Ambev.Domain.Sales.Creators;
using Ambev.Domain.Sales.Events;
using Ambev.Domain.Sales.Interfaces.Repositories;
using MediatR;

namespace Ambev.Application.Sales.Commands
{
    public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand>
    {
        private readonly ISaleQueryRepository _saleQueryRepository;
        private readonly ISaleCommandRepository _saleCommandRepository;
        private readonly SaleItemFactory _saleItemFactory;
        private readonly IMediator _mediator;
        private readonly Serilog.ILogger _logger;

        public UpdateSaleCommandHandler(ISaleQueryRepository saleQueryRepository, ISaleCommandRepository saleCommandRepository, SaleItemFactory saleItemFactory, IMediator mediator, Serilog.ILogger logger)
        {
            _saleQueryRepository = saleQueryRepository;
            _saleCommandRepository = saleCommandRepository;
            _saleItemFactory = saleItemFactory;
            _mediator = mediator;
            _logger = logger;
        }
        public async Task Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Received request to update sale with ID: {SaleId}", request.Id);

            var existingSale = await _saleQueryRepository.GetByIdWithItemsAsync(request.Id); 
            if (existingSale == null)
            {
                _logger.Warning("Sale with ID: {SaleId} not found.", request.Id);
                throw new KeyNotFoundException($"Sale with ID {request.Id} not found.");
            }

            _logger.Information("Updating details for sale with ID: {SaleId}.", request.Id);

            existingSale.Branch = request.Branch;

            foreach (var itemDto in request.Items)
            {
                var existingItem = existingSale.Items.FirstOrDefault(i => i.Id == itemDto.Id);

                if (existingItem != null)
                {
                    _logger.Information("Updating item with ID: {ItemId} in sale with ID: {SaleId}.", existingItem.Id, request.Id);
                    existingItem.Quantity = itemDto.Quantity;
                    existingItem.UnitPrice = itemDto.UnitPrice;
                    existingItem.Discount = itemDto.Discount;
                }
                else
                {
                    _logger.Information("Adding new item to sale with ID: {SaleId}.", request.Id);
                    var newItem = _saleItemFactory.CreateSaleItem(
                        existingSale.Id,
                        itemDto.ProductId,
                        itemDto.Quantity,
                        itemDto.UnitPrice
                    );
                    existingSale.AddItem(newItem);
                }
            }

            var itemsToRemove = existingSale.Items
                .Where(existingItem => request.Items.All(i => i.Id != existingItem.Id))
                .ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                existingSale.Items.Remove(itemToRemove);
            }

            try
            {
                _logger.Information("Saving updates for sale with ID: {SaleId}.", request.Id);
                await _saleCommandRepository.UpdateSaleAsync(existingSale);
                _logger.Information("Sale with ID: {SaleId} successfully updated.", request.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating sale with ID: {SaleId}.", request.Id);
                throw;
            }

            var saleModifiedEvent = new SaleModifiedEvent(existingSale.Id, DateTime.UtcNow);

            try
            {
                await _mediator.Publish(saleModifiedEvent, cancellationToken);
                _logger.Information("SaleModifiedEvent for sale with ID: {SaleId} successfully published.", request.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while publishing SaleModifiedEvent for sale with ID: {SaleId}.", request.Id);
                throw;
            }
        }
    }
}
