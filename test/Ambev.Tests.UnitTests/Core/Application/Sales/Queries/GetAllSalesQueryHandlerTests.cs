using Ambev.Application.Sales.DTOs;
using Ambev.Application.Sales.Queries;
using Ambev.Domain.Sales.Entities;
using Ambev.Domain.Sales.Interfaces.Repositories;
using AutoMapper;
using Bogus;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.Tests.UnitTests.Core.Application.Sales.Queries
{
    public class GetAllSalesQueryHandlerTests
    {
        private readonly ISaleQueryRepository _saleQueryRepository;
        private readonly IMapper _mapper;
        private readonly Faker<SaleEntity> _fakerSaleEntity;
        private readonly GetAllSalesQueryHandler _handler;

        public GetAllSalesQueryHandlerTests()
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

            _handler = new GetAllSalesQueryHandler(_saleQueryRepository, _mapper);
        }

        [Fact]
        public async Task Handle_ShouldReturnSalesWithItems_WhenWithItemsIsTrue()
        {
            // Arrange
            var sales = _fakerSaleEntity.Generate(3);
            var saleDtos = sales.Select(s => new SaleResponseDto
            {
                Id = s.Id,
                CustomerId = s.CustomerId,
                SaleDate = s.SaleDate,
                Branch = s.Branch,
                Items = s.Items.Select(i => new SaleItemResponseDto
                {
                    Id = i.Id,
                    SaleId = Guid.NewGuid(),
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Discount = i.Discount
                }).ToList()
            }).ToList();

            _saleQueryRepository.GetAllWithItemsAsync().Returns(sales);
            _mapper.Map<SaleResponseDto>(Arg.Any<SaleEntity>()).Returns(call =>
            {
                var entity = call.Arg<SaleEntity>();
                return saleDtos.FirstOrDefault(dto => dto.Id == entity.Id);
            });

            var query = new GetAllSalesQuery { WithItems = true };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(sales.Count, result.Count);
            await _saleQueryRepository.Received(1).GetAllWithItemsAsync();
            _mapper.Received(sales.Count).Map<SaleResponseDto>(Arg.Any<SaleEntity>());
        }

        [Fact]
        public async Task Handle_ShouldReturnSalesWithoutItems_WhenWithItemsIsFalse()
        {
            // Arrange
            var sales = _fakerSaleEntity.Generate(3);
            var saleDtos = sales.Select(s => new SaleResponseDto
            {
                Id = s.Id,
                CustomerId = s.CustomerId,
                SaleDate = s.SaleDate,
                Branch = s.Branch
            }).ToList();

            _saleQueryRepository.GetAllAsync("Sales").Returns(sales);
            _mapper.Map<SaleResponseDto>(Arg.Any<SaleEntity>()).Returns(call =>
            {
                var entity = call.Arg<SaleEntity>();
                return saleDtos.FirstOrDefault(dto => dto.Id == entity.Id);
            });

            var query = new GetAllSalesQuery { WithItems = false };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(sales.Count, result.Count);
            await _saleQueryRepository.Received(1).GetAllAsync("Sales");
            _mapper.Received(sales.Count).Map<SaleResponseDto>(Arg.Any<SaleEntity>());
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoSalesAreFound()
        {
            // Arrange
            _saleQueryRepository.GetAllWithItemsAsync().Returns(new List<SaleEntity>());
            var query = new GetAllSalesQuery { WithItems = true };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            await _saleQueryRepository.Received(1).GetAllWithItemsAsync();
            _mapper.DidNotReceive().Map<SaleResponseDto>(Arg.Any<SaleEntity>());
        }
    }
}
