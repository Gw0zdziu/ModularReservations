using Microsoft.Extensions.DependencyInjection;
using ModularReservations.Shared.Interfaces.Command;
using ModularReservations.Shared.Interfaces.Dispatcher;
using ModularReservations.Shared.Interfaces.Event;

namespace ModularReservations.Shared.Infrastructure.Dispatcher;

public sealed class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public Dispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
    {
        var commandHandler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        await commandHandler.HandleAsync(command, cancellationToken);

    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
        var handleMethod = handlerType.GetMethod(nameof(IEventHandler<>.HandleAsync))!;
        foreach (var handler in _serviceProvider.GetServices(handlerType))
        {
            await (Task)handleMethod.Invoke(handler, [@event, cancellationToken])!;
        }
    }
}
