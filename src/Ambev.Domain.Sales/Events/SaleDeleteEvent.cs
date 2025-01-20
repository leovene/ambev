using MediatR;

namespace Ambev.Domain.Sales.Events
{
    public class SaleDeleteEvent : INotification
    {
        public Guid Id { get; }
        public DateTime DeletedDate { get; }

        public SaleDeleteEvent(Guid id, DateTime deletedDate)
        {
            Id = id;
            DeletedDate = deletedDate;
        }
    }
}
