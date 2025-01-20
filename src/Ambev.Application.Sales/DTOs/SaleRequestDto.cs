namespace Ambev.Application.Sales.DTOs
{
    public class SaleRequestDto
    {
        public Guid CustomerId { get; set; }
        public string? Branch { get; set; }
        public List<SaleItemRequestDto> Items { get; set; } = new();
    }
}
