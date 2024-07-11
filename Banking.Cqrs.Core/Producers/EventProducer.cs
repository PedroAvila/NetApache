using Banking.Cqrs.Core.Events;

namespace Banking.Cqrs.Core.Producers
{
    public interface EventProducer
    {
        void Produce(string topic, BaseEvent baseEvent);
    }
}
