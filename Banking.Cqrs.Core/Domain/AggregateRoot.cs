using Banking.Cqrs.Core.Events;
using System.Windows.Markup;

namespace Banking.Cqrs.Core.Domain
{
    public abstract class AggregateRoot
    {
        public string Id { get; set; } = string.Empty;

        private int version = -1;

        List<BaseEvent> changes = new();

        public int GetVersion()
        {
            return version;
        }

        public void SetVersion(int version)
        {
            this.version = version;
        }

        public List<BaseEvent> GetUncommitedChanges()
        {
            return changes;
        }

        public void MarkChangesAsCommited()
        {
            changes.Clear();
        }

        public void ApplyChanges(BaseEvent baseEvent, bool isNewEvent) 
        {
            try
            {
                var claseDeEvento = baseEvent.GetType();
                var method = GetType().GetMethod("Apply", new[] { claseDeEvento });
                method.Invoke(this, new object[] { baseEvent });
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (isNewEvent) 
                    changes.Add(baseEvent);

            }
        }

        public void RaiseEvent(BaseEvent baseEvent)
        {
            ApplyChanges(baseEvent, true);
        }

        public void ReplayEvents(IEnumerable<BaseEvent> events)
        {
            foreach (var evt in events)
            {
                ApplyChanges(evt, false);
            }
        }
    }
}
