namespace Ambev.Application.Sales.Commands
{
    public class UpdateSaleItemCommand
    {
        public required Guid Id { get; set; }
        public required Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
    }
}
