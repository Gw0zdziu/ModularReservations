using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions;

public sealed class ReservationDateRequiredException()
    : DomainException("Reservation date must be specified.");
