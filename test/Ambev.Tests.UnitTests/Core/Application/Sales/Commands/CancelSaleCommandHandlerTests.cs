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
    public class CancelSaleCommandHandlerTests
    {
        private readonly ISaleQueryRepository _saleQueryRepository;
        private readonly ISaleCommandRepository _saleCommandRepository;
        private readonly IMediator _mediator;
        private readonly Serilog.ILogger _logger;
        private readonly Faker<SaleEntity> _fakerSale;

        private readonly CancelSaleCommandHandler _handler;

        public CancelSaleCommandHandlerTests()
        {
            _saleQueryRepository = Substitute.For<ISaleQueryRepository>();
            _saleCommandRepository = Substitute.For<ISaleCommandRepository>();
            _mediator = Substitute.For<IMediator>();
            _logger = Substitute.For<Serilog.ILogger>();

            _fakerSale = new Faker<SaleEntity>()
                .RuleFor(s => s.Id, f => Guid.NewGuid())
                .RuleFor(s => s.CustomerId, f => Guid.NewGuid())
                .RuleFor(s => s.SaleDate, f => f.Date.Past())
                .RuleFor(s => s.IsCancelled, f => false);

            _handler = new CancelSaleCommandHandler(_saleQueryRepository, _saleCommandRepository, _mediator, _logger);
        }

        [Fact]
        public async Task Handle_ShouldCancelSale_WhenSaleExists()
        {
            // Arrange
            var sale = _fakerSale.Generate();
            var command = new CancelSaleCommand { Id = sale.Id };

            _saleQueryRepository.GetByIdAsync("Sales", sale.Id).Returns(sale);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(sale.IsCancelled);
            await _saleCommandRepository.Received(1).UpdateAsync(sale);
            await _mediator.Received(1).Publish(Arg.Is<SaleCancelEvent>(e => e.Id == sale.Id), Arg.Any<CancellationToken>());
            _logger.Received(1).Information("Sale with ID: {SaleId} successfully updated as cancelled.", sale.Id);
        }

        [Fact]
        public async Task Handle_ShouldThrowKeyNotFoundException_WhenSaleDoesNotExist()
        {
            // Arrange
            var command = new CancelSaleCommand { Id = Guid.NewGuid() };

            _saleQueryRepository.GetByIdAsync("Sales", command.Id).Returns((SaleEntity?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal($"Sale with ID {command.Id} not found.", exception.Message);
            _logger.Received(1).Warning("Sale with ID: {SaleId} not found.", command.Id);
        }

        [Fact]
        public async Task Handle_ShouldLogError_WhenRepositoryUpdateFails()
        {
            // Arrange
            var sale = _fakerSale.Generate();
            var command = new CancelSaleCommand { Id = sale.Id };

            _saleQueryRepository.GetByIdAsync("Sales", sale.Id).Returns(sale);
            _saleCommandRepository.UpdateAsync(sale).Throws(new Exception("Database error"));

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
            var command = new CancelSaleCommand { Id = sale.Id };

            _saleQueryRepository.GetByIdAsync("Sales", sale.Id).Returns(sale);
            _mediator.Publish(Arg.Any<SaleCancelEvent>(), Arg.Any<CancellationToken>()).Throws(new Exception("Event error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Event error", exception.Message);
            _logger.Received(1).Error(Arg.Any<Exception>(), "An error occurred while publishing SaleCancelEvent for sale with ID: {SaleId}.", sale.Id);
        }
    }
}
