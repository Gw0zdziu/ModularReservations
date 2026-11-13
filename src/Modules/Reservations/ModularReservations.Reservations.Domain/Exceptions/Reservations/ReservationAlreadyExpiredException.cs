using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Reservations;

public sealed class ReservationAlreadyExpiredException(DateTime expirationDate)
    : DomainException($"Reservation has already expired at {expirationDate:yyyy-MM-dd HH:mm:ss} UTC.")
{
    public DateTime ExpirationDate { get; } = expirationDate;
}
