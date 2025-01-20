namespace Ambev.Application.Sales.DTOs
{
    public class SaleItemResponseDto
    {
        public Guid Id { get; set; } 
        public required Guid SaleId { get; set; }
        public required Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsCancelled { get; set; }
    }
}
