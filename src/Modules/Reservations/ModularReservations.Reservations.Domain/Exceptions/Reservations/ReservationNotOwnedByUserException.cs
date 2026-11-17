using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Reservations;

public sealed class ReservationNotOwnedByUserException(Guid reservationId, Guid userId) : DomainException($"Reservation {reservationId} does not belong to user {userId}.")
{
    public Guid ReservationId { get; } = reservationId;
    public Guid UserId { get; } =  userId;
}
