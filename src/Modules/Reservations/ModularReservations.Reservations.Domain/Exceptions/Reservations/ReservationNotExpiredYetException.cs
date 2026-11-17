using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Reservations;

public sealed class ReservationNotExpiredYetException(Guid id, DateTime expirationDate, DateTime now)
    : DomainException($"Reservation id: {id} have not expired yet, expire date {expirationDate:yyyy-MM-dd HH:mm:ss} UTC. Now: {now:yyyy-MM-dd HH:mm:ss}")
{
    public Guid Id { get; } = id;
    public DateTime ExpirationDate { get; } = expirationDate;
    public DateTime Now { get; } = now;
}
