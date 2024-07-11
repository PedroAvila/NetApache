using Banking.Account.Command.Application.Aggregates;
using Banking.Account.Command.Application.Contracts.Persistence;
using Banking.Account.Command.Domain;
using Banking.Cqrs.Core.Events;
using Banking.Cqrs.Core.Infrastructure;
using Banking.Cqrs.Core.Producers;

namespace Banking.Account.Command.Infrastructure.KafkaEvents
{
    public class AccountEventStore : EventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly EventProducer _eventProducer;

        public AccountEventStore(IEventStoreRepository eventStoreRepository, EventProducer eventProducer)
        {
            _eventStoreRepository = eventStoreRepository ?? throw new ArgumentNullException(nameof(eventStoreRepository));
            _eventProducer = eventProducer ?? throw new ArgumentNullException(nameof(eventProducer));
        }

        public async Task<List<BaseEvent>> GetEvents(string aggregateId)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateIdentifier(aggregateId);
            if (eventStream is null || !eventStream.Any())
                throw new Exception("La cuenta bancaria no existe");

            return eventStream.Select(x => x.EventData).ToList();
        }

        public async Task SaveEvents(string aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateIdentifier(aggregateId);
            if (expectedVersion != -1 && eventStream.ElementAt(eventStream.Count() - 1).Version != expectedVersion)
            {
                throw new Exception("Errores de concurrencia");
            }

            var version = expectedVersion;
            foreach(var evt  in events)
            {
                version++;
                evt.Version = version;

                var eventModel = new EventModel
                {
                    Timestamp = DateTime.Now,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(AccountAggregate),
                    Version = version,
                    EventType = evt.GetType().Name,
                    EventData = evt
                };

                await _eventStoreRepository.InsertDocumentAsync(eventModel);
                _eventProducer.Produce(evt.GetType().Name, evt);
            }
        }
    }
}
