using Banking.Cqrs.Core.Events;

namespace Banking.Cqrs.Core.Infrastructure
{
    public interface EventStore
    {
        Task SaveEvents(string aggregateId, IEnumerable<BaseEvent> events, int expectedVersion);
        Task<List<BaseEvent>> GetEvents(string aggregateId);
    }
}
