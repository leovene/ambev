using Ambev.Application.Sales.DTOs;
using MediatR;

namespace Ambev.Application.Sales.Queries
{
    public class GetAllSalesQuery : IRequest<List<SaleResponseDto>>
    {
        public bool WithItems { get; set; }
    }
}
