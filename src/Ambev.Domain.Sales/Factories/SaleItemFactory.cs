using Ambev.Domain.Sales.Entities;

namespace Ambev.Domain.Sales.Creators
{
    public class SaleItemFactory
    {
        public SaleItemEntity CreateSaleItem(Guid saleId, Guid productId, int quantity, decimal unitPrice)
        {
            if (quantity < 1) throw new ArgumentException("Quantity must be greater than 0.");
            if (quantity > 20) throw new ArgumentException("Cannot sell more than 20 identical items.");

            var discount = CalculateDiscount(quantity);

            return new SaleItemEntity
            {
                SaleId = saleId,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = unitPrice,
                Discount = discount
            };
        }

        private decimal CalculateDiscount(int quantity)
        {
            if (quantity >= 10) return 0.20m;
            if (quantity >= 4) return 0.10m;
            return 0m;
        }
    }
}
