using Ambev.Application.Sales.Commands;
using Ambev.Domain.Sales.Creators;
using Ambev.Domain.Sales.Entities;
using Ambev.Domain.Sales.Events;
using Ambev.Domain.Sales.Interfaces.Repositories;
using Bogus;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Ambev.Tests.UnitTests.Core.Application.Sales.Commands
{
    public class CreateSaleCommandHandlerTests
    {
        private readonly ISaleCommandRepository _saleCommandRepository;
        private readonly SaleItemFactory _saleItemFactory;
        private readonly IMediator _mediator;
        private readonly Serilog.ILogger _logger;
        private readonly Faker<CreateSaleCommand> _fakerCommand;

        private readonly CreateSaleCommandHandler _handler;

        public CreateSaleCommandHandlerTests()
        {
            _saleCommandRepository = Substitute.For<ISaleCommandRepository>();
            _saleItemFactory = Substitute.For<SaleItemFactory>();
            _mediator = Substitute.For<IMediator>();
            _logger = Substitute.For<Serilog.ILogger>();

            _fakerCommand = new Faker<CreateSaleCommand>()
                .RuleFor(c => c.CustomerId, f => Guid.NewGuid())
                .RuleFor(c => c.SaleDate, f => f.Date.Past())
                .RuleFor(c => c.Items, f => new List<CreateSaleItemCommand>
                {
                    new CreateSaleItemCommand { ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 10.0m },
                    new CreateSaleItemCommand { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 5.0m }
                });

            _handler = new CreateSaleCommandHandler(_saleCommandRepository, _saleItemFactory, _mediator, _logger);
        }

        [Fact]
        public async Task Handle_ShouldCreateSale_WhenValidCommandIsProvided()
        {
            // Arrange
            var command = _fakerCommand.Generate();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            await _saleCommandRepository.Received(1).AddAsync(Arg.Is<SaleEntity>(sale =>
                sale.CustomerId == command.CustomerId &&
                sale.Items.Count == command.Items.Count));
            await _mediator.Received(1).Publish(Arg.Is<SaleCreatedEvent>(e =>
                e.Id == result && e.CustomerId == command.CustomerId), Arg.Any<CancellationToken>());
            _logger.Received(1).Information("Sale with ID {SaleId} successfully saved.", Arg.Any<Guid>());
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenNoItemsProvided()
        {
            // Arrange
            var command = _fakerCommand.Clone().RuleFor(c => c.Items, new List<CreateSaleItemCommand>()).Generate();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("At least one item must be provided to create a sale.", exception.Message);
            _logger.Received(1).Warning("No items found in CreateSaleCommand for sale {SaleId}.", Arg.Any<Guid>());
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenSaleCommandRepositoryFails()
        {
            // Arrange
            var command = _fakerCommand.Generate();
            _saleCommandRepository.AddAsync(Arg.Any<SaleEntity>()).Throws(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
            _logger.Received(1).Error(Arg.Any<Exception>(), "An error occurred while saving sale with ID {SaleId}.", Arg.Any<Guid>());
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenMediatorFails()
        {
            // Arrange
            var command = _fakerCommand.Generate();
            _mediator.Publish(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>()).Throws(new Exception("Event error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Event error", exception.Message);
            _logger.Received(1).Error(Arg.Any<Exception>(), "An error occurred while publishing SaleCreatedEvent for sale {SaleId}.", Arg.Any<Guid>());
        }
    }
}
