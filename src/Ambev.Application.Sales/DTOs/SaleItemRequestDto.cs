namespace Ambev.Application.Sales.DTOs
{
    public class SaleItemRequestDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
