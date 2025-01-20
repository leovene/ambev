using Ambev.Domain.Sales.Creators;
using Ambev.Domain.Sales.Entities;
using Ambev.Domain.Sales.Events;
using Ambev.Domain.Sales.Interfaces.Repositories;
using MediatR;

namespace Ambev.Application.Sales.Commands
{
    public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Guid>
    {
        private readonly ISaleCommandRepository _saleCommandRepository;
        private readonly SaleItemFactory _saleItemFactory;
        private readonly IMediator _mediator;
        private readonly Serilog.ILogger _logger;

        public CreateSaleCommandHandler(ISaleCommandRepository saleCommandRepository, SaleItemFactory saleItemFactory, IMediator mediator, Serilog.ILogger logger)
        {
            _saleCommandRepository = saleCommandRepository;
            _saleItemFactory = saleItemFactory;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Received CreateSaleCommand for customer {CustomerId} with {ItemCount} items.", request.CustomerId, request.Items?.Count);

            var sale = new SaleEntity
            {
                Id = Guid.NewGuid(),
                Branch = request.Branch,
                SaleDate = request.SaleDate,
                CustomerId = request.CustomerId
            };

            _logger.Information("Creating sale with ID {SaleId} for customer {CustomerId}.", sale.Id, sale.CustomerId);

            if (request.Items == null || !request.Items.Any())
            {
                _logger.Warning("No items found in CreateSaleCommand for sale {SaleId}.", sale.Id);
                throw new ArgumentException("At least one item must be provided to create a sale.");
            }

            foreach (var item in request.Items)
            {
                _logger.Information("Adding item with ProductId {ProductId}, Quantity {Quantity}, UnitPrice {UnitPrice} to sale {SaleId}.",
                    item.ProductId, item.Quantity, item.UnitPrice, sale.Id);

                var saleItem = _saleItemFactory.CreateSaleItem(sale.Id, item.ProductId, item.Quantity, item.UnitPrice);
                sale.AddItem(saleItem);
            }

            try
            {
                _logger.Information("Saving sale with ID {SaleId} to the database.", sale.Id);
                await _saleCommandRepository.AddAsync(sale);
                _logger.Information("Sale with ID {SaleId} successfully saved.", sale.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while saving sale with ID {SaleId}.", sale.Id);
                throw;
            }

            var saleCreatedEvent = new SaleCreatedEvent(sale.Id, sale.SaleNumber, sale.SaleDate, sale.CustomerId);
            _logger.Information("Publishing SaleCreatedEvent for sale {SaleId}.", sale.Id);

            try
            {
                await _mediator.Publish(saleCreatedEvent, cancellationToken);
                _logger.Information("SaleCreatedEvent for sale {SaleId} successfully published.", sale.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while publishing SaleCreatedEvent for sale {SaleId}.", sale.Id);
                throw;
            }

            return sale.Id;
        }
    }
}
