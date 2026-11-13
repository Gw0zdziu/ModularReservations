using ModularReservations.Reservations.Domain.Statuses;
using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Reservations;

public sealed class InvalidReservationStatusException(Guid reservationId, ReservationStatus actual, ReservationStatus expected)
    : DomainException($"Reservation {reservationId} must be in status {expected}, but is {actual}.")
{
    public Guid ReservationId { get; } = reservationId;
    public ReservationStatus Actual { get; } = actual;
    public ReservationStatus Expected { get; } = expected;
}
