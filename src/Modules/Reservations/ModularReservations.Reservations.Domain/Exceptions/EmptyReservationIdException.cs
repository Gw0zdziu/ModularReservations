using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Reservations;

public sealed class EmptyReservationIdException() : DomainException("Reservation id cannot be empty.")
{
}
