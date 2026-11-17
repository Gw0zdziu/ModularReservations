using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Application.Exceptions;

public class ScreeningNotFoundException(Guid screeningId) : AppException($"Screening {screeningId} not found")
{
    public Guid ScreeningId { get; } = screeningId;
}
