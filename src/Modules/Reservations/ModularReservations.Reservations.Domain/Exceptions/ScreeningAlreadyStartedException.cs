using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions;

public sealed class ScreeningAlreadyStartedException(Guid screeningId, DateTime screeningDate)
    : DomainException($"Screening {screeningId} already started at {screeningDate:yyyy-MM-dd HH:mm} UTC.")
{
    public Guid ScreeningId { get; } = screeningId;
    public DateTime ScreeningDate { get; } = screeningDate;
}
