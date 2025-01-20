namespace Ambev.Application.Sales.Commands
{
    public class CreateSaleItemDocumentCommand
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required Guid SaleId { get; set; }
        public required Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount => Quantity * UnitPrice * (1 - Discount);
    }
}
