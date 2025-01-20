using Ambev.Domain.Sales.Entities;

namespace Ambev.Application.Sales.DTOs
{
    public class SaleResponseDto
    {
        public Guid Id { get; set; }
        public required Guid CustomerId { get; set; }
        public string? SaleNumber { get; set; } 
        public DateTime SaleDate { get; set; }
        public string? Branch { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsCancelled { get; set; }
        public List<SaleItemResponseDto> Items { get; set; } = new();
    }
}
