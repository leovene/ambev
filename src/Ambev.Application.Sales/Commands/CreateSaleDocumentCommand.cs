using Ambev.Domain.Sales.Entities;
using MediatR;

namespace Ambev.Application.Sales.Commands
{
    public class CreateSaleDocumentCommand : IRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required Guid CustomerId { get; set; }
        public string SaleNumber { get; set; } = Guid.NewGuid().ToString();
        public DateTime SaleDate { get; set; } = DateTime.UtcNow;
        public string? Branch { get; set; }
        public decimal TotalAmount => Items.Sum(i => i.TotalAmount);
        public List<CreateSaleItemDocumentCommand> Items { get; private set; } = new();
    }
}
