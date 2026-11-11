using ModularReservations.Shared.Interfaces;

namespace ModularReservations.Shared.Aggregates;

public abstract class AggregateRoot
{
    private readonly List<IEvent> _events = [];
    public IReadOnlyCollection<IEvent> Events => _events.AsReadOnly();

    protected void AddEvent(IEvent @event)
    {
        _events.Add(@event);
    }

    public void ClearEvents()
    {
        _events.Clear();
    }
}
