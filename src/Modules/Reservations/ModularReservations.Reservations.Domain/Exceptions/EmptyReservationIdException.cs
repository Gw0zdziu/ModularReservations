using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions;

public sealed class EmptyReservationIdException() : DomainException("Reservation id cannot be empty.")
{
}
