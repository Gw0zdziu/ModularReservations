using ModularReservations.Reservations.Application.Exceptions;
using ModularReservations.Reservations.Application.Interfaces;
using ModularReservations.Reservations.Domain.Events.Reservations;
using ModularReservations.Reservations.Domain.Interfaces.Repositories;
using ModularReservations.Shared.Interfaces.Dispatcher;
using ModularReservations.Shared.Interfaces.Event;

namespace ModularReservations.Reservations.Application.EventHandlers.ReservationCancelled;

public sealed class ReservationCancelledEventHandler : IEventHandler<ReservationCancelledEvent>
{
    private readonly IScreeningRepository _screeningRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDispatcher _dispatcher;

    public ReservationCancelledEventHandler(IScreeningRepository screeningRepository, IDispatcher dispatcher, IUnitOfWork unitOfWork)
    {
        _screeningRepository = screeningRepository;
        _dispatcher = dispatcher;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(ReservationCancelledEvent @event, CancellationToken cancellationToken = default)
    {
        var screening = await _screeningRepository.GetByIdAsync(@event.ScreeningId, cancellationToken);
        if (screening is null)
        {
            throw new ScreeningNotFoundException(@event.ScreeningId);
        }
        screening.ReleaseSeat(@event.SeatId);
        _screeningRepository.Update(screening);
        await _unitOfWork.CommitAsync(cancellationToken);
        foreach (var screeningEvent in screening.Events)
        {
            await _dispatcher.PublishAsync(screeningEvent, cancellationToken);
        }
        screening.ClearEvents();
    }
}
