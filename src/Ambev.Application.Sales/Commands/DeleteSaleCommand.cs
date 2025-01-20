using MediatR;

namespace Ambev.Application.Sales.Commands
{
    public class DeleteSaleCommand : IRequest
    {
        public required Guid Id { get; set; }
    }
}
