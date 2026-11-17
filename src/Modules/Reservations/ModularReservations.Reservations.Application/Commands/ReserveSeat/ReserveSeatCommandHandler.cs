using ModularReservations.Reservations.Application.Exceptions;
using ModularReservations.Reservations.Domain.Repositories;
using ModularReservations.Shared.Interfaces.Command;
using ModularReservations.Shared.Interfaces.Dispatcher;

namespace ModularReservations.Reservations.Application.Commands.ReserveSeat;

public class ReserveSeatCommandHandler : ICommandHandler<ReserveSeatCommand>
{
    private readonly IScreeningRepository _screeningRepository;
    private readonly TimeProvider _timeProvider;
    private readonly IDispatcher _dispatcher;

    public ReserveSeatCommandHandler(IScreeningRepository screeningRepository, TimeProvider timeProvider, IDispatcher dispatcher)
    {
        _screeningRepository = screeningRepository;
        _timeProvider = timeProvider;
        _dispatcher = dispatcher;
    }

    public async Task HandleAsync(ReserveSeatCommand command, CancellationToken cancellationToken = default)
    {
        var screening = await _screeningRepository.GetByIdAsync(command.ScreeningId, cancellationToken);
        if (screening is null)
        {
            throw new ScreeningNotFoundException(command.ScreeningId);
        }
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        screening.ReserveSeat(command.SeatId, command.UserId, now);
        await _screeningRepository.UpdateAsync(screening, cancellationToken);
        foreach (var @event in screening.Events)
        {
            await _dispatcher.PublishAsync(@event, cancellationToken);
        }
        screening.ClearEvents();
    }
}


