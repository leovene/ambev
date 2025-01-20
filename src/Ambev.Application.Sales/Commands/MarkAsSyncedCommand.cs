using MediatR;

namespace Ambev.Application.Sales.Commands
{
    public class MarkAsSyncedCommand : IRequest
    {
        public required Guid Id { get; set; }
    }
}
