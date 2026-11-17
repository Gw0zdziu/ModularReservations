using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Reservations;

public sealed class CancellationDeadlinePassedException(Guid reservationId, DateTime cancellationDeadline, DateTime now)
    : DomainException( $"Cannot cancel reservation {reservationId}: cancellation deadline passed at {cancellationDeadline:yyyy-MM-dd HH:mm} UTC. Now: {now:yyyy-MM-dd HH:mm} UTC.")
{
    public Guid ReservationId { get; } = reservationId;
    public DateTime CancellationDeadline { get; } = cancellationDeadline;
    public DateTime Now { get; } = now;
}
