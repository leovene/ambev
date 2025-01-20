using Ambev.Application.Sales.Commands;
using Ambev.Domain.Sales.Entities;
using Ambev.Domain.Sales.Events;
using Ambev.Domain.Sales.Interfaces.Repositories;
using Bogus;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Ambev.Tests.UnitTests.Core.Application.Sales.Commands
{
    public class CancelSaleItemsCommandHandlerTests
    {
        private readonly ISaleQueryRepository _saleQueryRepository;
        private readonly ISaleCommandRepository _saleCommandRepository;
        private readonly IMediator _mediator;
        private readonly Serilog.ILogger _logger;
        private readonly Faker<SaleEntity> _fakerSale;

        private readonly CancelSaleItemsCommandHandler _handler;

        public CancelSaleItemsCommandHandlerTests()
        {
            _saleQueryRepository = Substitute.For<ISaleQueryRepository>();
            _saleCommandRepository = Substitute.For<ISaleCommandRepository>();
            _mediator = Substitute.For<IMediator>();
            _logger = Substitute.For<Serilog.ILogger>();

            _fakerSale = new Faker<SaleEntity>()
                .RuleFor(s => s.Id, f => Guid.NewGuid())
                .RuleFor(s => s.CustomerId, f => Guid.NewGuid())
                .RuleFor(s => s.SaleDate, f => f.Date.Past())
                .RuleFor(s => s.IsCancelled, f => false)
                .RuleFor(s => s.Items, f => new List<SaleItemEntity>
                {
                    new SaleItemEntity { Id = Guid.NewGuid(), SaleId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 10.0m, Discount = 0.1m, IsCancelled = false },
                    new SaleItemEntity { Id = Guid.NewGuid(), SaleId = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 15.0m, Discount = 0.2m, IsCancelled = false }
                });

            _handler = new CancelSaleItemsCommandHandler(_saleQueryRepository, _saleCommandRepository, _mediator, _logger);
        }

        [Fact]
        public async Task Handle_ShouldCancelSaleItems_WhenItemsExist()
        {
            // Arrange
            var sale = _fakerSale.Generate();
            var itemToCancel = sale.Items.First();
            var command = new CancelSaleItemsCommand
            {
                SaleId = sale.Id,
                ItemIds = new List<Guid> { itemToCancel.Id }
            };

            _saleQueryRepository.GetByIdWithItemsAsync(sale.Id).Returns(sale);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(itemToCancel.IsCancelled);
            await _saleCommandRepository.Received(1).UpdateSaleAsync(sale);
            await _mediator.Received(1).Publish(Arg.Is<SaleItemCancelEvent>(e =>
                e.SaleId == sale.Id && e.ItemIds.Contains(itemToCancel.Id)), Arg.Any<CancellationToken>());
            _logger.Received(1).Information("Sale with ID: {SaleId} successfully updated.", sale.Id);
        }

        [Fact]
        public async Task Handle_ShouldThrowKeyNotFoundException_WhenSaleDoesNotExist()
        {
            // Arrange
            var command = new CancelSaleItemsCommand
            {
                SaleId = Guid.NewGuid(),
                ItemIds = new List<Guid> { Guid.NewGuid() }
            };

            _saleQueryRepository.GetByIdWithItemsAsync(command.SaleId).Returns((SaleEntity?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal($"Sale with ID {command.SaleId} not found.", exception.Message);
            _logger.Received(1).Warning("Sale with ID: {SaleId} not found.", command.SaleId);
        }

        [Fact]
        public async Task Handle_ShouldThrowKeyNotFoundException_WhenItemDoesNotExistInSale()
        {
            // Arrange
            var sale = _fakerSale.Generate();
            var command = new CancelSaleItemsCommand
            {
                SaleId = sale.Id,
                ItemIds = new List<Guid> { Guid.NewGuid() } // Non-existent item ID
            };

            _saleQueryRepository.GetByIdWithItemsAsync(sale.Id).Returns(sale);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.StartsWith("Item with ID", exception.Message);
            _logger.Received(1).Warning("Item with ID: {ItemId} not found in sale {SaleId}.", Arg.Any<Guid>(), sale.Id);
        }

        [Fact]
        public async Task Handle_ShouldLogError_WhenRepositoryUpdateFails()
        {
            // Arrange
            var sale = _fakerSale.Generate();
            var itemToCancel = sale.Items.First();
            var command = new CancelSaleItemsCommand
            {
                SaleId = sale.Id,
                ItemIds = new List<Guid> { itemToCancel.Id }
            };

            _saleQueryRepository.GetByIdWithItemsAsync(sale.Id).Returns(sale);
            _saleCommandRepository.UpdateSaleAsync(sale).Throws(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
            _logger.Received(1).Error(Arg.Any<Exception>(), "An error occurred while updating sale with ID: {SaleId}.", sale.Id);
        }

        [Fact]
        public async Task Handle_ShouldLogError_WhenPublishingEventFails()
        {
            // Arrange
            var sale = _fakerSale.Generate();
            var itemToCancel = sale.Items.First();
            var command = new CancelSaleItemsCommand
            {
                SaleId = sale.Id,
                ItemIds = new List<Guid> { itemToCancel.Id }
            };

            _saleQueryRepository.GetByIdWithItemsAsync(sale.Id).Returns(sale);
            _mediator.Publish(Arg.Any<SaleItemCancelEvent>(), Arg.Any<CancellationToken>()).Throws(new Exception("Event error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Event error", exception.Message);
            _logger.Received(1).Error(Arg.Any<Exception>(), "An error occurred while publishing SaleItemCancelEvent for sale with ID: {SaleId}.", sale.Id);
        }
    }
}
