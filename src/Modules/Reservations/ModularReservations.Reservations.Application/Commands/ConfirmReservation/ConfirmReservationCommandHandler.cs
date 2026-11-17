using ModularReservations.Reservations.Application.Exceptions;
using ModularReservations.Reservations.Domain.Repositories;
using ModularReservations.Shared.Interfaces.Command;
using ModularReservations.Shared.Interfaces.Dispatcher;

namespace ModularReservations.Reservations.Application.Commands.ConfirmReservation;

public class ConfirmReservationCommandHandler : ICommandHandler<ConfirmReservationCommand>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly  IDispatcher _dispatcher;

    public ConfirmReservationCommandHandler(IReservationRepository reservationRepository, IDispatcher dispatcher)
    {
        _reservationRepository = reservationRepository;
        _dispatcher = dispatcher;
    }

    public async Task HandleAsync(ConfirmReservationCommand command, CancellationToken cancellationToken = default)
    {
        var reservation = await _reservationRepository.GetByIdAsync(command.ReservationId, cancellationToken);
        if (reservation is null)
        {
            throw new ReservationNotFoundException(command.ReservationId);
        }
        reservation.ConfirmReservation();
        await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        foreach (var @event in reservation.Events)
        {
            await _dispatcher.PublishAsync(@event, cancellationToken);
        }
        reservation.ClearEvents();

    }
}
