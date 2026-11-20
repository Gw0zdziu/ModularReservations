using ModularReservations.Reservations.Application.Exceptions;
using ModularReservations.Reservations.Application.Interfaces;
using ModularReservations.Reservations.Domain.Interfaces.Repositories;
using ModularReservations.Shared.Interfaces.Command;
using ModularReservations.Shared.Interfaces.Dispatcher;

namespace ModularReservations.Reservations.Application.Commands.CancelReservation;

public sealed class CancelReservationCommandHandler : ICommandHandler<CancelReservationCommand>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IScreeningRepository _screeningRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDispatcher _dispatcher;
    private readonly TimeProvider _timeProvider;


    public CancelReservationCommandHandler(IReservationRepository reservationRepository, IDispatcher dispatcher, IScreeningRepository screeningRepository, TimeProvider timeProvider, IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _dispatcher = dispatcher;
        _screeningRepository = screeningRepository;
        _timeProvider = timeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(CancelReservationCommand command, CancellationToken cancellationToken = default)
    {
        var reservation = await _reservationRepository.GetByIdAsync(command.ReservationId, cancellationToken);
        if (reservation is null)
        {
            throw new ReservationNotFoundException(command.ReservationId);
        }
        var screening = await _screeningRepository.GetByIdAsync(reservation.ScreeningId, cancellationToken);
        if (screening is null)
        {
            throw new ScreeningNotFoundException(reservation.ScreeningId);
        }
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        reservation.CancelReservation(command.UserId, screening.ScreeningDate, now);
        _reservationRepository.Update(reservation);
        await _unitOfWork.CommitAsync(cancellationToken);
        foreach (var @event in reservation.Events)
        {
            await _dispatcher.PublishAsync(@event, cancellationToken);
        }
        reservation.ClearEvents();
    }
}
