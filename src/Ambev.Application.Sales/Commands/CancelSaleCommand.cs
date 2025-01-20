using MediatR;

namespace Ambev.Application.Sales.Commands
{
    public class CancelSaleCommand : IRequest
    {
        public required Guid Id { get; set; }
    }
}
