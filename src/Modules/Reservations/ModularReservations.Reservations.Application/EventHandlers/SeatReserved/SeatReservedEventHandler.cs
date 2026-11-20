using ModularReservations.Reservations.Application.Interfaces;
using ModularReservations.Reservations.Domain.Aggregates;
using ModularReservations.Reservations.Domain.Events.Screenings;
using ModularReservations.Reservations.Domain.Interfaces.Repositories;
using ModularReservations.Shared.Interfaces.Dispatcher;
using ModularReservations.Shared.Interfaces.Event;

namespace ModularReservations.Reservations.Application.EventHandlers.SeatReserved;

public sealed class SeatReservedEventHandler : IEventHandler<SeatReservedEvent>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly IDispatcher _dispatcher;


    public SeatReservedEventHandler(IReservationRepository reservationRepository, TimeProvider timeProvider, IDispatcher dispatcher, IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _timeProvider = timeProvider;
        _dispatcher = dispatcher;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(SeatReservedEvent @event, CancellationToken cancellationToken = default)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var reservation = Reservation.Create(@event.ReservationId, now, @event.ScreeningId, @event.ReservationDate, @event.SeatId, @event.UserId);
        await _reservationRepository.AddAsync(reservation, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        foreach (var reservationEvent in reservation.Events)
        {
            await _dispatcher.PublishAsync(reservationEvent, cancellationToken);
        }
        reservation.ClearEvents();

    }
}
