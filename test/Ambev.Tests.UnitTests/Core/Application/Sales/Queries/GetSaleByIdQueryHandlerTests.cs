using Ambev.Application.Sales.DTOs;
using Ambev.Application.Sales.Queries;
using Ambev.Domain.Sales.Entities;
using Ambev.Domain.Sales.Interfaces.Repositories;
using AutoMapper;
using Bogus;
using NSubstitute;

namespace Ambev.Tests.UnitTests.Core.Application.Sales.Queries
{
    public class GetSaleByIdQueryHandlerTests
    {
        private readonly ISaleQueryRepository _saleQueryRepository;
        private readonly IMapper _mapper;
        private readonly Faker<SaleEntity> _fakerSaleEntity;
        private readonly GetSaleByIdQueryHandler _handler;

        public GetSaleByIdQueryHandlerTests()
        {
            _saleQueryRepository = Substitute.For<ISaleQueryRepository>();
            _mapper = Substitute.For<IMapper>();

            _fakerSaleEntity = new Faker<SaleEntity>()
                .RuleFor(s => s.Id, f => Guid.NewGuid())
                .RuleFor(s => s.CustomerId, f => Guid.NewGuid())
                .RuleFor(s => s.SaleDate, f => f.Date.Past())
                .RuleFor(s => s.Branch, f => f.Company.CompanyName())
                .RuleFor(s => s.Items, f => new List<SaleItemEntity>
                {
                    new SaleItemEntity
                    {
                        Id = Guid.NewGuid(),
                        SaleId = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        Quantity = f.Random.Int(1, 10),
                        UnitPrice = f.Random.Decimal(1, 100),
                        Discount = f.Random.Decimal(0, 0.2m)
                    }
                });

            _handler = new GetSaleByIdQueryHandler(_saleQueryRepository, _mapper);
        }

        [Fact]
        public async Task Handle_ShouldReturnSaleWithItems_WhenWithItemsIsTrue()
        {
            // Arrange
            var query = new GetSaleByIdQuery { Id = Guid.NewGuid(), WithItems = true };
            var saleEntity = _fakerSaleEntity.Generate();
            var saleDto = new SaleResponseDto
            {
                Id = saleEntity.Id,
                CustomerId = saleEntity.CustomerId,
                SaleDate = saleEntity.SaleDate,
                Branch = saleEntity.Branch,
                Items = saleEntity.Items.Select(i => new SaleItemResponseDto
                {
                    Id = i.Id,
                    SaleId = Guid.NewGuid(),
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Discount = i.Discount
                }).ToList()
            };

            _saleQueryRepository.GetByIdWithItemsAsync(query.Id).Returns(saleEntity);
            _mapper.Map<SaleResponseDto>(saleEntity).Returns(saleDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(saleDto.Id, result.Id);
            Assert.Equal(saleDto.CustomerId, result.CustomerId);
            Assert.Equal(saleDto.Items.Count, result.Items.Count);

            await _saleQueryRepository.Received(1).GetByIdWithItemsAsync(query.Id);
            _mapper.Received(1).Map<SaleResponseDto>(saleEntity);
        }

        [Fact]
        public async Task Handle_ShouldReturnSaleWithoutItems_WhenWithItemsIsFalse()
        {
            // Arrange
            var query = new GetSaleByIdQuery { Id = Guid.NewGuid(), WithItems = false };
            var saleEntity = _fakerSaleEntity.Generate();
            var saleDto = new SaleResponseDto
            {
                Id = saleEntity.Id,
                CustomerId = saleEntity.CustomerId,
                SaleDate = saleEntity.SaleDate,
                Branch = saleEntity.Branch
            };

            _saleQueryRepository.GetByIdAsync("Sales", query.Id).Returns(saleEntity);
            _mapper.Map<SaleResponseDto>(saleEntity).Returns(saleDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(saleDto.Id, result.Id);
            Assert.Equal(saleDto.CustomerId, result.CustomerId);

            await _saleQueryRepository.Received(1).GetByIdAsync("Sales", query.Id);
            _mapper.Received(1).Map<SaleResponseDto>(saleEntity);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenSaleNotFound()
        {
            // Arrange
            var query = new GetSaleByIdQuery { Id = Guid.NewGuid(), WithItems = true };
            _saleQueryRepository.GetByIdWithItemsAsync(query.Id).Returns((SaleEntity?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            await _saleQueryRepository.Received(1).GetByIdWithItemsAsync(query.Id);
            _mapper.DidNotReceive().Map<SaleResponseDto>(Arg.Any<SaleEntity>());
        }
    }
}
