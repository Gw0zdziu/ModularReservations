using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Reservations;

public sealed class ReservationDateInFutureException(DateTime reservationDate, DateTime now)
    : DomainException($"Reservation date {reservationDate:yyyy-MM-dd HH:mm:ss} UTC cannot be later than current time {now:yyyy-MM-dd HH:mm:ss} UTC.")
{
    public DateTime ReservationDate { get; } = reservationDate;
    public DateTime Now { get; } = now;
}
