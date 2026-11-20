using ModularReservations.Reservations.Application.Exceptions;
using ModularReservations.Reservations.Application.Interfaces;
using ModularReservations.Reservations.Domain.Interfaces.Repositories;
using ModularReservations.Shared.Interfaces.Command;
using ModularReservations.Shared.Interfaces.Dispatcher;

namespace ModularReservations.Reservations.Application.Commands.ConfirmReservation;

public sealed class ConfirmReservationCommandHandler : ICommandHandler<ConfirmReservationCommand>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDispatcher _dispatcher;

    public ConfirmReservationCommandHandler(IReservationRepository reservationRepository, IDispatcher dispatcher, IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _dispatcher = dispatcher;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(ConfirmReservationCommand command, CancellationToken cancellationToken = default)
    {
        var reservation = await _reservationRepository.GetByIdAsync(command.ReservationId, cancellationToken);
        if (reservation is null)
        {
            throw new ReservationNotFoundException(command.ReservationId);
        }
        reservation.ConfirmReservation();
        _reservationRepository.Update(reservation);
        await _unitOfWork.CommitAsync(cancellationToken);
        foreach (var @event in reservation.Events)
        {
            await _dispatcher.PublishAsync(@event, cancellationToken);
        }
        reservation.ClearEvents();

    }
}
