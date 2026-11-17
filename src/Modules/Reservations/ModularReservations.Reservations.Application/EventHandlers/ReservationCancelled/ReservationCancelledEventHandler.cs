using ModularReservations.Reservations.Application.Exceptions;
using ModularReservations.Reservations.Domain.Events.Reservations;
using ModularReservations.Reservations.Domain.Repositories;
using ModularReservations.Shared.Interfaces.Dispatcher;
using ModularReservations.Shared.Interfaces.Event;

namespace ModularReservations.Reservations.Application.EventHandlers.ReservationCancelled;

public class ReservationCancelledEventHandler : IEventHandler<ReservationCancelledEvent>
{
    private readonly IScreeningRepository _screeningRepository;
    private readonly IDispatcher _dispatcher;

    public ReservationCancelledEventHandler(IScreeningRepository screeningRepository, IDispatcher dispatcher)
    {
        _screeningRepository = screeningRepository;
        _dispatcher = dispatcher;
    }

    public async Task HandleAsync(ReservationCancelledEvent @event, CancellationToken cancellationToken = default)
    {
        var screening = await _screeningRepository.GetByIdAsync(@event.ScreeningId, cancellationToken);
        if (screening is null)
        {
            throw new ScreeningNotFoundException(@event.ScreeningId);
        }
        screening.ReleaseSeat(@event.SeatId);
        await _screeningRepository.UpdateAsync(screening, cancellationToken);
        foreach (var screeningEvent in screening.Events)
        {
            await _dispatcher.PublishAsync(screeningEvent, cancellationToken);
        }
        screening.ClearEvents();
    }
}
