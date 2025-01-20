using MediatR;

namespace Ambev.Domain.Sales.Events
{
    public class SaleCancelEvent : INotification
    {
        public Guid Id { get; }
        public DateTime CancelledDate { get; }

        public SaleCancelEvent(Guid id, DateTime cancelledDate)
        {
            Id = id;
            CancelledDate = cancelledDate;
        }
    }
}
