using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Screenings;

public sealed class SeatPositionRequiredException()
    : DomainException("Seat position must be specified.");
