using MediatR;

namespace Ambev.Domain.Sales.Events
{
    public class SaleModifiedEvent : INotification
    {
        public Guid Id { get; }
        public DateTime ModifiedDate { get; }

        public SaleModifiedEvent(Guid id, DateTime modifiedDate)
        {
            Id = id;
            ModifiedDate = modifiedDate;
        }
    }
}
