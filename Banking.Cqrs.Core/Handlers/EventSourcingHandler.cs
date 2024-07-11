using Banking.Cqrs.Core.Domain;

namespace Banking.Cqrs.Core.Handlers
{
    public interface EventSourcingHandler<T>
    {
        Task Save(AggregateRoot aggregate);
        Task<T> GetById(string id);
    }
}
