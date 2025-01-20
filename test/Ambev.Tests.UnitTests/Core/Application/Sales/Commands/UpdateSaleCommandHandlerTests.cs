using Ambev.Application.Sales.Commands;
using Ambev.Domain.Sales.Creators;
using Ambev.Domain.Sales.Entities;
using Ambev.Domain.Sales.Events;
using Ambev.Domain.Sales.Interfaces.Repositories;
using Bogus;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.Tests.UnitTests.Core.Application.Sales.Commands
{
    public class UpdateSaleCommandHandlerTests
    {
        private readonly ISaleQueryRepository _saleQueryRepository;
        private readonly ISaleCommandRepository _saleCommandRepository;
        private readonly SaleItemFactory _saleItemFactory;
        private readonly IMediator _mediator;
        private readonly Serilog.ILogger _logger;
        private readonly Faker<UpdateSaleCommand> _fakerCommand;
        private readonly UpdateSaleCommandHandler _handler;

        public UpdateSaleCommandHandlerTests()
        {
            _saleQueryRepository = Substitute.For<ISaleQueryRepository>();
            _saleCommandRepository = Substitute.For<ISaleCommandRepository>();
            _saleItemFactory = Substitute.For<SaleItemFactory>();
            _mediator = Substitute.For<IMediator>();
            _logger = Substitute.For<Serilog.ILogger>();

            _fakerCommand = new Faker<UpdateSaleCommand>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Branch, f => f.Address.City())
                .RuleFor(c => c.Items, f => new List<UpdateSaleItemCommand>
                {
                    new UpdateSaleItemCommand { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 10.0m, Discount = 0.1m },
                    new UpdateSaleItemCommand { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 5.0m, Discount = 0.2m }
                });

            _handler = new UpdateSaleCommandHandler(_saleQueryRepository, _saleCommandRepository, _saleItemFactory, _mediator, _logger);
        }


        [Fact]
        public async Task Handle_ShouldThrowKeyNotFoundException_WhenSaleDoesNotExist()
        {
            // Arrange
            var command = _fakerCommand.Generate();
            _saleQueryRepository.GetByIdWithItemsAsync(command.Id).Returns((SaleEntity)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal($"Sale with ID {command.Id} not found.", exception.Message);
            _logger.Received(1).Warning("Sale with ID: {SaleId} not found.", command.Id);
        }

        [Fact]
        public async Task Handle_ShouldLogAndThrow_WhenUpdateSaleFails()
        {
            // Arrange
            var command = _fakerCommand.Generate();
            var existingSale = new SaleEntity { Id = command.Id, CustomerId = Guid.NewGuid() };
            _saleQueryRepository.GetByIdWithItemsAsync(command.Id).Returns(existingSale);
            _saleCommandRepository.UpdateSaleAsync(existingSale).Throws(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
            _logger.Received(1).Error(Arg.Any<Exception>(), "An error occurred while updating sale with ID: {SaleId}.", command.Id);
        }

        [Fact]
        public async Task Handle_ShouldLogAndThrow_WhenPublishingEventFails()
        {
            // Arrange
            var command = _fakerCommand.Generate();
            var existingSale = new SaleEntity { Id = command.Id, CustomerId = Guid.NewGuid() };
            _saleQueryRepository.GetByIdWithItemsAsync(command.Id).Returns(existingSale);
            _mediator.Publish(Arg.Any<SaleModifiedEvent>(), Arg.Any<CancellationToken>()).Throws(new Exception("Event error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Event error", exception.Message);
            _logger.Received(1).Error(Arg.Any<Exception>(), "An error occurred while publishing SaleModifiedEvent for sale with ID: {SaleId}.", command.Id);
        }
    }
}
