using ModularReservations.Reservations.Domain.Aggregates;
using ModularReservations.Reservations.Domain.Events.Screenings;
using ModularReservations.Reservations.Domain.Repositories;
using ModularReservations.Shared.Interfaces.Dispatcher;
using ModularReservations.Shared.Interfaces.Event;

namespace ModularReservations.Reservations.Application.EventHandlers.SeatReserved;

public class SeatReservedEventHandler : IEventHandler<SeatReservedEvent>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly TimeProvider _timeProvider;
    private readonly IDispatcher _dispatcher;


    public SeatReservedEventHandler(IReservationRepository reservationRepository, TimeProvider timeProvider, IDispatcher dispatcher)
    {
        _reservationRepository = reservationRepository;
        _timeProvider = timeProvider;
        _dispatcher = dispatcher;
    }

    public async Task HandleAsync(SeatReservedEvent @event, CancellationToken cancellationToken = default)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var reservation = Reservation.Create(@event.ReservationId, now, @event.ScreeningId, @event.ReservationDate, @event.SeatId, @event.UserId);
        await _reservationRepository.AddAsync(reservation, cancellationToken);
        foreach (var reservationEvent in reservation.Events)
        {
            await _dispatcher.PublishAsync(reservationEvent, cancellationToken);
        }
        reservation.ClearEvents();

    }
}
