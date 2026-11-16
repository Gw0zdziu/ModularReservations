using ModularReservations.Shared.Interfaces.Command;
using ModularReservations.Shared.Interfaces.Event;

namespace ModularReservations.Shared.Interfaces.Dispatcher;

public interface IDispatcher
{
    Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand;
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
}
