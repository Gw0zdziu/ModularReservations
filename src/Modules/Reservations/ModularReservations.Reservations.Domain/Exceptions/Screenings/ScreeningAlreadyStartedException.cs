using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Screenings;

public sealed class ScreeningAlreadyStartedException(Guid screeningId, DateTime screeningDate)
    : DomainException($"Cannot reserve a seat for screening {screeningId}: it started at {screeningDate:yyyy-MM-dd HH:mm} UTC.")
{
    public Guid ScreeningId { get; } = screeningId;
    public DateTime ScreeningDate { get; } = screeningDate;
}
