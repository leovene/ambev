using MediatR;

namespace Ambev.Application.Sales.Commands
{
    public class UpdateSaleCommand : IRequest
    {
        public required Guid Id { get; set; }
        public string? Branch { get; set; }
        public List<UpdateSaleItemCommand> Items { get; set; } = new();
    }
}
