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
    public class DeleteSaleCommandHandlerTests
    {
        private readonly ISaleQueryRepository _saleQueryRepository;
        private readonly ISaleCommandRepository _saleCommandRepository;
        private readonly IMediator _mediator;
        private readonly Serilog.ILogger _logger;
        private readonly Faker<DeleteSaleCommand> _fakerCommand;

        private readonly DeleteSaleCommandHandler _handler;

        public DeleteSaleCommandHandlerTests()
        {
            _saleQueryRepository = Substitute.For<ISaleQueryRepository>();
            _saleCommandRepository = Substitute.For<ISaleCommandRepository>();
            _mediator = Substitute.For<IMediator>();
            _logger = Substitute.For<Serilog.ILogger>();

            _fakerCommand = new Faker<DeleteSaleCommand>()
                .RuleFor(c => c.Id, f => Guid.NewGuid());

            _handler = new DeleteSaleCommandHandler(_saleQueryRepository, _saleCommandRepository, _mediator, _logger);
        }

        [Fact]
        public async Task Handle_ShouldDeleteSale_WhenSaleExists()
        {
            // Arrange
            var command = _fakerCommand.Generate();
            var sale = new SaleEntity { Id = command.Id, CustomerId = Guid.NewGuid() };

            _saleQueryRepository.GetByIdAsync("Sales", command.Id).Returns(sale);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            await _saleCommandRepository.Received(1).DeleteAsync(sale);
            await _mediator.Received(1).Publish(Arg.Is<SaleDeleteEvent>(e => e.Id == command.Id), Arg.Any<CancellationToken>());
            _logger.Received(1).Information("Sale with ID: {SaleId} successfully deleted.", command.Id);
        }

        [Fact]
        public async Task Handle_ShouldThrowKeyNotFoundException_WhenSaleDoesNotExist()
        {
            // Arrange
            var command = _fakerCommand.Generate();

            _saleQueryRepository.GetByIdAsync("Sales", command.Id).Returns((SaleEntity)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal($"Sale with ID {command.Id} not found.", exception.Message);
            _logger.Received(1).Warning("Sale with ID: {SaleId} not found.", command.Id);
        }

        [Fact]
        public async Task Handle_ShouldLogError_WhenDeleteFails()
        {
            // Arrange
            var command = _fakerCommand.Generate();
            var sale = new SaleEntity { Id = command.Id, CustomerId = Guid.NewGuid() };

            _saleQueryRepository.GetByIdAsync("Sales", command.Id).Returns(sale);
            _saleCommandRepository.DeleteAsync(sale).Throws(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
            _logger.Received(1).Error(Arg.Any<Exception>(), "An error occurred while deleting sale with ID: {SaleId}.", command.Id);
        }

        [Fact]
        public async Task Handle_ShouldLogError_WhenPublishFails()
        {
            // Arrange
            var command = _fakerCommand.Generate();
            var sale = new SaleEntity { Id = command.Id, CustomerId = Guid.NewGuid() };

            _saleQueryRepository.GetByIdAsync("Sales", command.Id).Returns(sale);
            _mediator.Publish(Arg.Any<SaleDeleteEvent>(), Arg.Any<CancellationToken>()).Throws(new Exception("Event error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Event error", exception.Message);
            _logger.Received(1).Error(Arg.Any<Exception>(), "An error occurred while publishing SaleDeleteEvent for sale with ID: {SaleId}.", command.Id);
        }
    }
}
