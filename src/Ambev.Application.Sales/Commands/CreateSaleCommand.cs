using MediatR;

namespace Ambev.Application.Sales.Commands
{
    public class CreateSaleCommand : IRequest<Guid>
    {
        public Guid CustomerId { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.UtcNow;
        public string? Branch { get; set; }
        public List<CreateSaleItemCommand> Items { get; set; } = new();
    }
}
