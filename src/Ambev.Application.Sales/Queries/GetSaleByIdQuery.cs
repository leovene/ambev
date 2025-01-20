using Ambev.Application.Sales.DTOs;
using MediatR;

namespace Ambev.Application.Sales.Queries
{
    public class GetSaleByIdQuery : IRequest<SaleResponseDto?>
    {
        public required Guid Id { get; set; }
        public bool WithItems { get; set; }
    }
}
