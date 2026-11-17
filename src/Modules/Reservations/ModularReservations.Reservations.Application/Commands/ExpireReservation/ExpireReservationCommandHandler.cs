using ModularReservations.Reservations.Application.Exceptions;
using ModularReservations.Reservations.Domain.Repositories;
using ModularReservations.Shared.Interfaces.Command;
using ModularReservations.Shared.Interfaces.Dispatcher;

namespace ModularReservations.Reservations.Application.Commands.ExpireReservation;

public class ExpireReservationCommandHandler : ICommandHandler<ExpireReservationCommand>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IDispatcher _dispatcher;
    private readonly TimeProvider _timeProvider;

    public ExpireReservationCommandHandler(IReservationRepository reservationRepository, IDispatcher dispatcher, TimeProvider timeProvider)
    {
        _reservationRepository = reservationRepository;
        _dispatcher = dispatcher;
        _timeProvider = timeProvider;
    }

    public async Task HandleAsync(ExpireReservationCommand command, CancellationToken cancellationToken = default)
    {
        var reservation = await _reservationRepository.GetByIdAsync(command.ReservationId, cancellationToken);
        if (reservation is null)
        {
            throw new ReservationNotFoundException(command.ReservationId);
        }
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        reservation.ExpireReservation(now);
        await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        foreach (var @event in reservation.Events)
        {
            await _dispatcher.PublishAsync(@event, cancellationToken);
        }
        reservation.ClearEvents();
    }
}
