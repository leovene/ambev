using MediatR;

namespace Ambev.Application.Sales.Commands
{
    public class CancelSaleItemsCommand : IRequest
    {
        public required Guid SaleId { get; set; }
        public required List<Guid> ItemIds { get; set; }
    }
}
