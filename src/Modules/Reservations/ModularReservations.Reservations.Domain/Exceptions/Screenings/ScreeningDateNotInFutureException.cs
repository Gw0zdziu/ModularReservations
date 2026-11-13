using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Screenings;

public sealed class ScreeningDateNotInFutureException(DateTime screeningDate, DateTime now)
    : DomainException($"Screening date {screeningDate:yyyy-MM-dd HH:mm} UTC must be later than current time {now:yyyy-MM-dd HH:mm} UTC.")
{
    public DateTime ScreeningDate { get; } = screeningDate;
    public DateTime Now { get; } = now;
}
