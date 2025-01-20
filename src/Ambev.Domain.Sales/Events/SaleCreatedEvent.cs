using MediatR;

namespace Ambev.Domain.Sales.Events
{
    public class SaleCreatedEvent : INotification
    {
        public Guid Id { get; }
        public string SaleNumber { get; }
        public DateTime SaleDate { get; }
        public Guid CustomerId { get; }

        public SaleCreatedEvent(Guid id, string saleNumber, DateTime saleDate, Guid customerId)
        {
            Id = id;
            SaleNumber = saleNumber;
            SaleDate = saleDate;
            CustomerId = customerId;
        }
    }
}
