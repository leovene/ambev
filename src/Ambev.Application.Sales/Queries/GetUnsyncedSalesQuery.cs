using Ambev.Application.Sales.DTOs;
using MediatR;

namespace Ambev.Application.Sales.Queries
{
    public class GetUnsyncedSalesQuery : IRequest<List<SaleResponseDto>>
    {
    }
}
