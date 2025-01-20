using MediatR;

namespace Ambev.Domain.Sales.Events
{
    public class SaleItemCancelEvent : INotification
    {
        public Guid SaleId { get; }
        public List<Guid> ItemIds { get; set; }
        public DateTime CancelledDate { get; }

        public SaleItemCancelEvent(Guid saleId, List<Guid> itemIds, DateTime cancelledDate)
        {
            SaleId = saleId;
            ItemIds = itemIds;
            CancelledDate = cancelledDate;
        }
    }
}
