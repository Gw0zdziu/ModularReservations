using ModularReservations.Shared.Exceptions;

namespace ModularReservations.Reservations.Domain.Exceptions.Reservations;

public sealed class EmptySeatIdException()
    : DomainException("Seat id cannot be empty.");
